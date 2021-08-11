using Convience.Entity.Entity.SRM;
using Convience.Mail;
using Convience.ManagentApi.Infrastructure.Authorization;
using Convience.ManagentApi.Infrastructure.Logs;
using Convience.Model.Models.ContentManage;
using Convience.Model.Models.SRM;
using Convience.Model.Models.SystemManage;
using Convience.Service.SRM;
using Convience.Service.SystemManage;
using Convience.Util.Extension;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Convience.ManagentApi.Controllers.SRM
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SrmRfqController : ControllerBase
    {
        private readonly ISrmMatnrService _srmMatnrService;
        private readonly ISrmVendorService _srmVendorService;
        private readonly ISrmRfqHService _srmRfqHService;
        private readonly ISrmRfqMService _srmRfqMService;
        private readonly ISrmRfqVService _srmRfqVService;
        private readonly ISrmQotService _srmQotHService;
        private readonly IUserService _userService;

        public SrmRfqController(ISrmMatnrService srmMatnrService,
            ISrmVendorService srmVendorService,
            ISrmRfqHService srmRfqHService, 
            ISrmRfqMService srmRfqMService,
            ISrmRfqVService srmRfqVService,
            ISrmQotService srmQotHService,
            IUserService userService)
        {
            _srmMatnrService = srmMatnrService;
            _srmVendorService = srmVendorService;
            _srmRfqHService = srmRfqHService;
            _srmRfqMService = srmRfqMService;
            _srmRfqVService = srmRfqVService;
            _srmQotHService = srmQotHService;
            _userService = userService;
        }

        [HttpPost("GetMatnr")]
        public IActionResult GetMatnr(QueryMatnrModel matnrQuery)
        {
            return Ok(_srmMatnrService.GetMatnr(matnrQuery));
        }

        [HttpPost("GetVendor")]
        public IActionResult GetVendor(QueryVendorModel vendorQuery) {
            return Ok(_srmVendorService.GetVendor(vendorQuery));
        }
        [HttpPost("Save")]
        public IActionResult Save(JObject rfq) {
            SrmRfqH h = rfq["h"].ToObject<SrmRfqH>();
            SrmRfqM[] ms = rfq["m"].ToObject<SrmRfqM[]>();
            SrmRfqV[] vs = rfq["v"].ToObject<SrmRfqV[]>();
            DateTime now = DateTime.Now;
            h.LastUpdateDate = now;
            _srmRfqHService.Save(h, ms, vs);
            return Ok();
        }

        [HttpGet("GetRfqData")]
        public IActionResult GetRfqData(int id) {
            ViewSrmRfqH h = _srmRfqHService.GetDataByRfqId(id);
            h.sourcerName = _userService.GetUsers(new UserQueryModel() { UserName = h.Sourcer,Page=1,Size=1 }).Data[0].Name;
            System.Linq.IQueryable m = _srmRfqMService.GetDataByRfqId(id);
            ViewSrmRfqV[] v = _srmRfqVService.GetDataByRfqId(id);
            Newtonsoft.Json.JsonSerializer js = new Newtonsoft.Json.JsonSerializer();
            js.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            //JObject rfq = new JObject() {
            //    { "h",JObject.FromObject(h,js)},
            //    { "m",JArray.FromObject(m,js)},
            //    { "v",JArray.FromObject(v,js)},
            //};
            ResultRfqModel rfq = new ResultRfqModel() {
                h = h,
                m = m,
                v = v
            };
            return Ok(rfq);
        }

        [HttpPost("GetRfqList")]
        public IActionResult GetRfqList(JObject query)
        {
            QueryRfqList q = new QueryRfqList();
            q.rfqNum = query["rfqNum"].ToString();
            q.status = (int)query["status"];
            q.name = query["name"].ToString();
            q.werks = Array.ConvertAll(query["werks"].ToString().Split(","), s => int.Parse(s));
            int page = (int)query["page"];
            int size = (int)query["size"];
            var h = _srmRfqHService.GetRfqList(q,page,size);
            return Ok(h);
        }
        [HttpPost("StartUp")]
        public IActionResult StartUp(JObject rfq) {
            SrmRfqH h = rfq["h"].ToObject<SrmRfqH>();
            SrmRfqM[] matnrs = rfq["m"].ToObject<SrmRfqM[]>();
            SrmRfqV[] vendors = rfq["v"].ToObject<SrmRfqV[]>();
            DateTime now = DateTime.Now.Date;

            if (h.Deadline < now) {
                return BadRequest("截止日期已過");
            }

            using (var transaction = new System.Transactions.TransactionScope()) {
                try
                {
                    _srmRfqHService.Save(h, matnrs, vendors);
                    h.LastUpdateDate = DateTime.Now;
                    _srmRfqHService.UpdateStatus((int)Status.啟動, h);
                    List<SrmQotH> qots = new List<SrmQotH>();
                    foreach (var matnr in matnrs)
                    {
                        foreach (var vendor in vendors)
                        {
                            SrmQotH qot = new SrmQotH();
                            qot.MatnrId = matnr.MatnrId;
                            qot.VendorId = vendor.VendorId;
                            qot.RfqId = h.RfqId;
                            qot.Status = (int)Status.初始;
                            qot.CreateDate = now;
                            qot.CreateBy = h.LastUpdateBy;
                            qot.LastUpdateDate = now;
                            qot.LastUpdateBy = h.LastUpdateBy;
                            qots.Add(qot);
                        }
                    }
                    _srmQotHService.Add(qots.ToArray());
                    //db.SaveChanges();
                    //db.Database.CommitTransaction();
                    transaction.Complete();
                    return Ok();
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    return BadRequest(ex.Message);
                }
            }
        }
        //[HttpPost("GetSourcer")]
        //public IActionResult GetSourcer(UserQueryModel userQuery) {
        //    return Ok(_userService.GetUsers(userQuery));
        //}
        [HttpPost("Cancel")]
        public IActionResult Cancel(SrmRfqH rfqH) {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    rfqH.LastUpdateDate = DateTime.Now;
                    var rfq = _srmRfqHService.UpdateStatus(((int)Status.作廢), rfqH);
                    _srmQotHService.UpdateStatus((int)Status.作廢, rfqH);
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("leon.jcg@chenfull.com.tw");
                    mail.To.Add("leon.jcg@chenfull.com.tw");
                    var temp = _srmRfqVService.GetDataByRfqId(rfqH.RfqId);
                    //string a = JsonConvert.SerializeObject(_srmRfqVService.GetDataByRfqId(rfqH.RfqId));
                    ViewSrmRfqV[] vendors = JsonConvert.DeserializeObject<ViewSrmRfqV[]>(JsonConvert.SerializeObject(_srmRfqVService.GetDataByRfqId(rfqH.RfqId)));
                    StringBuilder sb = new StringBuilder();
                    StringBuilder qotNums = new StringBuilder();
                    sb.AppendLine("收件者:");
                    foreach (var vendor in vendors) {
                        sb.AppendLine(vendor.Mail);
                    }
                    SrmQotH[] qots = _srmQotHService.Get(new QueryQot() { rfqId = rfqH.RfqId });
                    foreach (var qot in qots) {
                        qotNums.AppendLine(qot.QotNum);
                    }


                    string body = $@"親愛的供應商夥伴您好,
因本公司內部原因，
詢價單：{rfq.RfqNum}
以下報價單：
{qotNums.ToString()}
進行取消詢價作業，
造成貴公司困擾深感抱歉，
謝謝!

此信為系統發送,
 如有任何問題，
 請回覆此E-MAIL致採購窗口或致電協調。";

                        //_srmVendorService.GetUsers(new UserQueryModel() { UserName = rfq.Sourcer,Page=1,Size=1 });
                    mail.Body = body;
                    mail.Subject = "詢價單作廢通知";
                    using (System.Net.Mail.SmtpClient MySMTP = new System.Net.Mail.SmtpClient("mail.chenfull.com.tw", 25))
                    {
                        MySMTP.Send(mail);
                        MySMTP.Dispose();
                    }
                    transaction.Complete();
                    return Ok();
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    return BadRequest(ex.Message);
                }
            }
        }
        [HttpPost("Delete")]
        public IActionResult Delete(SrmRfqH rfqH)
        {
            try
            {
                rfqH.LastUpdateDate = DateTime.Now;
                _srmRfqHService.UpdateStatus(((int)Status.刪除), rfqH);
                return Ok();
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("GetSourcerList")]
        public IActionResult GetSourcerList(JObject jobj) {
            string name = jobj["name"].ToString();
            int[] werks = Array.ConvertAll(jobj["werks"].ToString().Split(","), s => int.Parse(s));
            int page = (int)jobj["page"];
            int size = (int)jobj["size"];
            var users = _srmRfqHService.GetSourcer(name, werks, size,page);
            return Ok(users);
            //return Ok(_srmRfqHService.GetSourcer(users));
        }
        [HttpPost("GetRfq")]
        public IActionResult GetRfq(QueryRfq query) {
            return Ok(_srmRfqHService.GetRfq(query));
        }
        [HttpPost("AsyncSourcer")]
        public IActionResult AsyncSourcer() {
            _srmRfqHService.AsyncSourcer();
            return Ok();
        }
    }
}
