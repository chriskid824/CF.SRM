using Convience.Entity.Entity.SRM;
using Convience.Filestorage.Abstraction;
using Convience.JwtAuthentication;
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
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Convience.ManagentApi.Controllers.SRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SrmRfqController : ControllerBase
    {
        private readonly ISrmMatnrService _srmMatnrService;
        private readonly ISrmVendorService _srmVendorService;
        private readonly ISrmRfqHService _srmRfqHService;
        private readonly ISrmRfqMService _srmRfqMService;
        private readonly ISrmRfqVService _srmRfqVService;
        private readonly ISrmQotService _srmQotHService;
        private readonly IUserService _userService;
        private readonly ISrmSupplierService _srmSupplierService;
        private readonly appSettings _appSettingsService;
        private readonly ISrmMaterialService _srmMaterialService;
        private readonly ISrmFileService _srmFileService;

        public SrmRfqController(ISrmMatnrService srmMatnrService,
            ISrmVendorService srmVendorService,
            ISrmRfqHService srmRfqHService, 
            ISrmRfqMService srmRfqMService,
            ISrmRfqVService srmRfqVService,
            ISrmQotService srmQotHService,
            IUserService userService,
            ISrmSupplierService srmSupplierService,
            IOptions<appSettings> appSettingsOption,
            ISrmMaterialService srmMaterialService,
            ISrmFileService srmFileService
            )
        {
            _srmMatnrService = srmMatnrService;
            _srmVendorService = srmVendorService;
            _srmRfqHService = srmRfqHService;
            _srmRfqMService = srmRfqMService;
            _srmRfqVService = srmRfqVService;
            _srmQotHService = srmQotHService;
            _userService = userService;
            _srmSupplierService = srmSupplierService;
            _appSettingsService = appSettingsOption.Value;
            _srmMaterialService = srmMaterialService;
            _srmFileService = srmFileService;
        }

        [HttpPost("GetMatnr")]
        [Permission("rfq")]
        public IActionResult GetMatnr(QueryMatnrModel matnrQuery)
        {
            UserClaims user = User.GetUserClaims();
            matnrQuery.Werks = user.Werks;
            return Ok(_srmMatnrService.GetMatnr(matnrQuery));
        }

        [HttpPost("GetVendor")]
        [Permission("rfq")]
        public IActionResult GetVendor(QueryVendorModel vendorQuery) {
            UserClaims user = User.GetUserClaims();
            vendorQuery.Werks = user.Werks;
            return Ok(_srmVendorService.GetVendor(vendorQuery));
        }
        [HttpPost("Save")]
        [Permission("rfq")]
        public IActionResult Save(JObject rfq) {
            try
            {
                SrmRfqH h = rfq["h"].ToObject<SrmRfqH>();
                SrmRfqM[] ms = rfq["m"].ToObject<SrmRfqM[]>();
                SrmRfqV[] vs = rfq["v"].ToObject<SrmRfqV[]>();

                DateTime now = DateTime.Now;
                UserClaims user = User.GetUserClaims();
                if (user.UserName != h.Sourcer)
                {
                    return this.BadRequestResult("非詢價本人");
                }
                h.LastUpdateBy = user.UserName;
                h.LastUpdateDate = now;
                h.Werks = user.Werks[0];
                string rfqNum= _srmRfqHService.Save(h, ms, vs);
                string result = updateFileNumber(rfqNum, rfq);
                if (!string.IsNullOrEmpty(result))
                {
                    return this.BadRequestResult(result);
                }
                return Ok();
            }
            catch (Exception ex) {
                return this.BadRequestResult(ex.Message);
            }
        }

        [HttpGet("GetRfqData")]
        [Permission("rfq")]
        public IActionResult GetRfqData(int id) {
            ViewSrmRfqH h = _srmRfqHService.GetDataByRfqId(id);
            h.sourcerName = _userService.GetUsers(new UserQueryModel() { UserName = h.Sourcer,Page=1,Size=1 }).Data[0].Name;
            h.C_by = _userService.GetUsers(new UserQueryModel() { UserName = h.CreateBy, Page = 1, Size = 1 }).Data[0].Name;
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
        [Permission("rfq-manage")]
        public IActionResult GetRfqList(JObject query)
        {
            QueryRfqList q = new QueryRfqList();
            q.rfqNum = query["rfqNum"].ToString();
            q.status = (int)query["status"];
            q.name = query["name"].ToString();
            UserClaims user = User.GetUserClaims();
            q.werks = user.Werks;
            int page = (int)query["page"];
            int size = (int)query["size"];
            var h = _srmRfqHService.GetRfqList(q,page,size);
            return Ok(h);
        }
        [HttpPost("StartUp")]
        [Permission("rfq")]
        public IActionResult StartUp(JObject rfq) {
            SrmRfqH h = rfq["h"].ToObject<SrmRfqH>();
            SrmRfqM[] matnrs = rfq["m"].ToObject<SrmRfqM[]>();
            SrmRfqV[] vendors = rfq["v"].ToObject<SrmRfqV[]>();
            DateTime now = DateTime.Now.Date;

            if (h.Deadline < now) {
                return this.BadRequestResult("截止日期已過");
            }

            using (var transaction = new System.Transactions.TransactionScope()) {
                try
                {
                    UserClaims user = User.GetUserClaims();
                    if (user.UserName != h.Sourcer)
                    {
                        return this.BadRequestResult("非詢價本人");
                    }
                    h.LastUpdateBy = user.UserName;
                    h.LastUpdateDate = DateTime.Now;
                    h.Werks = user.Werks[0];                    
                    string rfqNum = _srmRfqHService.Save(h, matnrs, vendors);
                    string result = updateFileNumber(rfqNum, rfq);
                    if (!string.IsNullOrEmpty(result))
                    {
                        return this.BadRequestResult(result);
                    }
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

                    ViewSrmRfqV[] Vvendors = JsonConvert.DeserializeObject<ViewSrmRfqV[]>(JsonConvert.SerializeObject(_srmRfqVService.GetDataByRfqId(h.RfqId)));
                    //StringBuilder sb = new StringBuilder();
                    //MailMessage mail = new MailMessage();
                    //mail.From = new MailAddress("leon.jcg@chenfull.com.tw");
                    //mail.To.Add("leon.jcg@chenfull.com.tw");
                    //mail.To.Add("leo.lai@chenfull.com.tw");
                    //                    sb.AppendLine("收件者:<br />");
                    //                    foreach (var vendor in Vvendors)
                    //                    {
                    //                        sb.AppendLine(vendor.Mail+ "<br />");
                    //                    }

                    //                    string body = $@"親愛的供應商夥伴您好,<br />
                    //此信為系統發送,<br />
                    //詢價單{h.RfqNum}已發送，請協助於詢價截止日期前{h.Deadline.Value.ToString("yyyy/MM/dd")}回覆報價資訊，謝謝！<br />
                    // 如有任何問題，<br />
                    // 請回覆此E-MAIL致採購窗口或致電協調。<br />
                    //<a href='http://10.88.1.28/account/login'>SRM入口</a><br />
                    //{ sb.ToString()}";

                    //                    mail.Body = body;
                    //                    mail.Subject = "詢價單啟動通知";
                    //                    mail.IsBodyHtml = true;
                    //                    using (System.Net.Mail.SmtpClient MySMTP = new System.Net.Mail.SmtpClient("mail.chenfull.com.tw", 25))
                    //                    {
                    //                        MySMTP.Send(mail);
                    //                        MySMTP.Dispose();
                    //                    }

                    sendRespective(Vvendors, h, "啟動");
                    //db.SaveChanges();
                    //db.Database.CommitTransaction();
                    transaction.Complete();
                    return Ok();
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    return this.BadRequestResult(ex.Message);
                }
            }
        }
        //[HttpPost("GetSourcer")]
        //public IActionResult GetSourcer(UserQueryModel userQuery) {
        //    return Ok(_userService.GetUsers(userQuery));
        //}
        [HttpPost("Cancel")]
        [Permission("rfq")]
        public IActionResult Cancel(SrmRfqH rfqH) {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    UserClaims user = User.GetUserClaims();
                    if (user.UserName != rfqH.Sourcer)
                    {
                        return this.BadRequestResult("非詢價本人");
                    }
                    rfqH.EndBy = user.UserName;
                    rfqH.LastUpdateBy = user.UserName;
                    rfqH.LastUpdateDate = DateTime.Now;
                    var rfq = _srmRfqHService.UpdateStatus(((int)Status.作廢), rfqH);
                    _srmQotHService.UpdateStatus((int)Status.作廢, rfqH);
                    var temp = _srmRfqVService.GetDataByRfqId(rfqH.RfqId);
                    //string a = JsonConvert.SerializeObject(_srmRfqVService.GetDataByRfqId(rfqH.RfqId));
                    ViewSrmRfqV[] vendors = JsonConvert.DeserializeObject<ViewSrmRfqV[]>(JsonConvert.SerializeObject(_srmRfqVService.GetDataByRfqId(rfqH.RfqId)));
                    #region 單封
                    //MailMessage mail = new MailMessage();
                    //mail.From = new MailAddress("leon.jcg@chenfull.com.tw");
                    //mail.To.Add("leon.jcg@chenfull.com.tw");
                    //StringBuilder sb = new StringBuilder();
                    //StringBuilder qotNums = new StringBuilder();
                    //                    sb.AppendLine("收件者:<br />");
                    //                    foreach (var vendor in vendors) {
                    //                        sb.AppendLine(vendor.Mail);
                    //                    }
                    //                    SrmQotH[] qots = _srmQotHService.Get(new QueryQot() { rfqId = rfqH.RfqId });
                    //                    foreach (var qot in qots) {
                    //                        qotNums.AppendLine(qot.QotNum);
                    //                    }

                    //                    string body = $@"親愛的供應商夥伴您好,<br />
                    //因本公司內部原因，<br />
                    //詢價單：{rfq.RfqNum}<br />
                    //以下報價單：<br />
                    //{qotNums.ToString()}<br />
                    //進行取消詢價作業，<br />
                    //造成貴公司困擾深感抱歉，<br />
                    //謝謝!<br />
                    //<br />
                    //此信為系統發送,<br />
                    // 如有任何問題，<br />
                    // 請回覆此E-MAIL致採購窗口或致電協調。<br />
                    //<a href='http://10.88.1.28/account/login'>SRM入口</a><br />
                    //{sb.ToString()}";

                    //mail.Body = body;
                    //mail.Subject = "詢價單作廢通知";
                    //mail.IsBodyHtml = true;
                    //using (System.Net.Mail.SmtpClient MySMTP = new System.Net.Mail.SmtpClient("mail.chenfull.com.tw", 25))
                    //{
                    //    MySMTP.Send(mail);
                    //    MySMTP.Dispose();
                    //}
                    #endregion 單封
                    sendRespective(vendors, rfq, "作廢");
                    transaction.Complete();
                    return Ok();
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    return this.BadRequestResult(ex.Message);
                }
            }
        }

        private void sendRespective(ViewSrmRfqV[]  vendors, SrmRfqH rfq, string type) {
            StringBuilder sb = new StringBuilder();
            StringBuilder qotNums = new StringBuilder();
            foreach (var vendor in vendors)
            {
                try
                {
                    MailMessage mail = new MailMessage();
                    //mail.From = new MailAddress("mis@chenfull.com.tw");
                    mail.To.Add(vendor.Mail);
                    mail.CC.Add("leo.lai@chenfull.com.tw");
                    var sourcer = _userService.GetUsers(new UserQueryModel { UserName = rfq.CreateBy, Page = 1, Size = 1 });
                    if (!string.IsNullOrWhiteSpace(sourcer.Data[0].Email)) { mail.CC.Add(sourcer.Data[0].Email); }
                    //mail.To.Add("leo.lai@chenfull.com.tw");
                    SrmQotH[] qots = _srmQotHService.Get(new QueryQot() { rfqId = rfq.RfqId, vendorId = vendor.VendorId });
                    sb.Clear();
                    //sb.AppendLine("收件者:<br />");
                    //sb.AppendLine(vendor.Mail+ "<br />");
                    qotNums.Clear();
                    foreach (var qot in qots)
                    {
                        qotNums.AppendLine(qot.QotNum + "<br />");
                    }
                    string body = "";
                    string subject = "";
                    string bodyVendor = $"{vendor.SrmVendor1} {vendor.VendorName}";
                    if (type == "作廢")
                    {
                        body = $@"親愛的{bodyVendor}夥伴您好,<br />
                    因本公司內部原因，<br />
                    詢價單：{rfq.RfqNum}<br />
                    以下報價單：<br />
                    {qotNums.ToString()}<br />
                    進行取消詢價作業，<br />
                    造成貴公司困擾深感抱歉，<br />
                    謝謝!<br />
                    <br />
                    此信為系統發送,<br />
                     如有任何問題，<br />
                     請回覆此E-MAIL致採購窗口或致電協調。<br />
                    <a href='http://10.88.1.28/account/login'>SRM入口</a><br />
                    {sb.ToString()}";
                        subject = $"千附-{rfq.RfqNum} 詢價單作廢通知";
                    }
                    else if (type == "啟動")
                    {
                        body = $@"親愛的{bodyVendor}夥伴您好,<br />
此信為系統發送,<br />
詢價單{rfq.RfqNum}已發送，請協助於詢價截止日期前{rfq.Deadline.Value.ToString("yyyy/MM/dd")}回覆報價資訊，謝謝！<br />
 如有任何問題，<br />
 請回覆此E-MAIL致採購窗口或致電協調。<br />
<a href='http://10.88.1.28/account/login'>SRM入口</a><br />
{ sb.ToString()}";
                        subject = $"千附-{rfq.RfqNum} 詢價單啟動通知";
                    }

                    mail.Body = body;
                    mail.Subject = subject;
                    mail.IsBodyHtml = true;
                    sendMail(mail);
                }
                catch (Exception ex) {
                    MailMessage mail = new MailMessage();
                    mail.To.Add("leon.jcg@chenfull.com.tw");
                    mail.Body = $"寄信異常，供應商:{vendor.VendorName}，EX:{ex.Message}";
                    mail.Subject = "寄信異常";
                    sendMailMIS(mail);
                }
            }
        }

        private void sendMail(MailMessage mail) {
            #region test
            if (_appSettingsService.Environment == "dev")
            {
                mail.To.Clear();
                mail.CC.Clear();
                mail.Bcc.Clear();
                mail.To.Add("leon.jcg@chenfull.com.tw");
            }
            #endregion test

            using (System.Net.Mail.SmtpClient MySMTP = new System.Net.Mail.SmtpClient("cfp-ex01.cfprec.com.tw", 25))
            {
                mail.From = new MailAddress("purchasing@cfprec.com.tw", "千附-採購部");
                MySMTP.Credentials = new System.Net.NetworkCredential("purchasing@cfprec.com.tw", "Ab1234567");
                MySMTP.Send(mail);
                MySMTP.Dispose();
            }
        }

        private void sendMailMIS(MailMessage mail)
        {
            #region test
            if (_appSettingsService.Environment == "dev")
            {
                mail.To.Clear();
                mail.CC.Clear();
                mail.Bcc.Clear();
                mail.To.Add("leon.jcg@chenfull.com.tw");
            }
            #endregion test

            using (System.Net.Mail.SmtpClient MySMTP = new System.Net.Mail.SmtpClient("mail.chenfull.com.tw", 25))
            {
                mail.From = new MailAddress("mis@chenfull.com.tw");
                MySMTP.Credentials = new System.Net.NetworkCredential("mis@chenfull.com.tw", "Chen@full");
                MySMTP.Send(mail);
                MySMTP.Dispose();
            }
        }


        [HttpPost("Delete")]
        [Permission("rfq")]
        public IActionResult Delete(SrmRfqH rfqH)
        {
            try
            {
                UserClaims user = User.GetUserClaims();
                if (user.UserName != rfqH.Sourcer)
                {
                    return this.BadRequestResult("非詢價本人");
                }
                rfqH.EndBy = user.UserName;
                rfqH.LastUpdateBy = user.UserName;
                rfqH.LastUpdateDate = DateTime.Now;
                _srmRfqHService.UpdateStatus(((int)Status.刪除), rfqH);
                return Ok();
            }catch (Exception ex)
            {
                return this.BadRequestResult(ex.Message);
            }
        }
        [HttpPost("GetSourcerList")]
        public IActionResult GetSourcerList(JObject jobj) {
            string name = jobj["name"].ToString();
            UserClaims user = User.GetUserClaims();
            int[] werks = user.Werks;
            int page = (int)jobj["page"];
            int size = (int)jobj["size"];
            var users = _srmRfqHService.GetSourcer(name, werks, size,page);
            return Ok(users);
            //return Ok(_srmRfqHService.GetSourcer(users));
        }
        [HttpPost("GetRfq")]
        [Permission("price")]
        [Permission("rfq")]
        public IActionResult GetRfq(QueryRfq query) {
            UserClaims user = User.GetUserClaims();
            query.werks = user.Werks;
            return Ok(_srmRfqHService.GetRfq(query));
        }
        [HttpPost("AsyncSourcer")]
        public IActionResult AsyncSourcer() {
            _srmRfqHService.AsyncSourcer();
            return Ok();
        }

        private string Convert_DataRowToJson(DataRow datarow)
        {
            var dict = new Dictionary<string, object>();
            foreach (DataColumn col in datarow.Table.Columns)
            {
                dict.Add(col.ColumnName, datarow[col]);
            }
            return JsonConvert.SerializeObject(dict);
        }

        [HttpPost("BatchUpload")]
        public IActionResult BatchUpload([FromForm] Model.Models.SRM.FileUploadViewModel_RFQ fileUploadModel) {
            UserClaims user = User.GetUserClaims();
            fileUploadModel.CreateBy = user.UserName;
            fileUploadModel.CurrentDirectory = _appSettingsService.CurrentDirectory + fileUploadModel.CurrentDirectory;
            string path = "";
            string RfqNum = "";
            try
            {
                if (user.Werks.Count() > 1) {
                    //throw new Exception("只限單一工廠人員申請");
                }
                path = _srmRfqHService.Upload(fileUploadModel);
                DataTable data_m = _srmRfqHService.ReadExcel_Matnr(path,user);
                if (data_m.Rows.Count == 0)
                {
                    throw new Exception("料號至少需一筆");
                }
                DataTable data_v = _srmRfqHService.ReadExcel_Vendor(path,user);
                if (data_v.Rows.Count == 0) {
                    throw new Exception("供應商至少需一筆");
                }
                string errTitle = "";
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    try
                    {
                        SrmRfqM[] m = JsonConvert.DeserializeObject<SrmRfqM[]>(JsonConvert.SerializeObject(data_m));
                        for (int i = 0; i < data_m.Rows.Count; i++)
                        {
                            DataRow dr_m = data_m.Rows[i];
                            errTitle = $"料號:{dr_m["SrmMatnr1"].ToString()}，";
                            if (!Convert.ToBoolean(dr_m["IsExists"].ToString()))
                            {
                                ViewSrmMatnr1 temp = JsonConvert.DeserializeObject<ViewSrmMatnr1>(Convert_DataRowToJson(dr_m));
                                temp.User = user.UserName;
                                m[i].MatnrId = _srmMaterialService.AddMatnr(temp).MatnrId;
                            }
                            else
                            {
                                var temp_matnr = _srmMatnrService.GetMatnr(new QueryMatnrModel() { MatnrEquals = dr_m["SrmMatnr1"].ToString(), Werks = user.Werks, withoutStatus = new int[] { (int)Status.失效 }, Page = 1, Size = 1 }).Data[0];
                                m[i] = JsonConvert.DeserializeObject<SrmRfqM>(JsonConvert.SerializeObject(temp_matnr));
                                m[i].Qty = Convert.ToDouble(data_m.Rows[i]["Qty"].ToString());
                                m[i].EstDeliveryDate = Convert.ToDateTime(data_m.Rows[i]["EstDeliveryDate"].ToString()).Date;
                                m[i].Unit = Convert.ToInt32(data_m.Rows[i]["Unit"].ToString());
                                //m[i].MatnrId = _srmMatnrService.GetMatnr(new QueryMatnrModel() { MatnrEquals = data_m.Rows[i]["SrmMatnr1"].ToString(),Werks=user.Werks, withoutStatus = new int[] { (int)Status.失效 }, Page = 1, Size = 1 }).Data[0].MatnrId;
                            }
                        }

                        SrmRfqV[] v = JsonConvert.DeserializeObject<SrmRfqV[]>(JsonConvert.SerializeObject(data_v));
                        for (int i = 0; i < data_v.Rows.Count; i++)
                        {
                            DataRow dr_v = data_v.Rows[i];
                            errTitle = $"供應商:{dr_v["SrmVendor1"].ToString()}，";
                            if (!Convert.ToBoolean(dr_v["IsExists"].ToString()))
                            {
                                ViewSrmSupplier temp = JsonConvert.DeserializeObject<ViewSrmSupplier>(Convert_DataRowToJson(dr_v));
                                temp.User = user.UserName;
                                v[i].VendorId = _srmSupplierService.AddVendor(temp).VendorId;
                            }
                            else
                            {
                                v[i].VendorId = _srmVendorService.GetVendor(new QueryVendorModel() { VendorEquals = dr_v["SrmVendor1"].ToString(), Werks = user.Werks, withoutStatus = new int[] { (int)Status.失效 }, Page = 1, Size = 1 }).Data[0].VendorId;
                            }
                        }
                        errTitle = "";
                        //foreach (DataRow dr_m in data_m.Rows)
                        //{
                        //    if (!Convert.ToBoolean(dr_m["IsExists"].ToString()))
                        //    {
                        //        ViewSrmMatnr1 temp = JsonConvert.DeserializeObject<ViewSrmMatnr1>(JsonConvert.SerializeObject(dr_m));
                        //        temp.User = user.Name;
                        //        _srmMaterialService.AddMatnr(temp);
                        //    }
                        //}

                        //foreach (DataRow dr_v in data_v.Rows)
                        //{
                        //    if (!Convert.ToBoolean(dr_v["IsExists"].ToString()))
                        //    {
                        //        ViewSrmSupplier temp = JsonConvert.DeserializeObject<ViewSrmSupplier>(JsonConvert.SerializeObject(dr_v));
                        //        temp.User = user.Name;
                        //        _srmSupplierService.AddVendor(temp);
                        //    }
                        //}
                        SrmRfqH h = new SrmRfqH();
                        h.Sourcer = user.UserName;
                        h.Werks = user.Werks[0];
                        h.LastUpdateBy = user.UserName;
                        h.Status = 1;
                        //SrmRfqM[] m = JsonConvert.DeserializeObject<SrmRfqM[]>(JsonConvert.SerializeObject(data_m));
                        //SrmRfqV[] v = JsonConvert.DeserializeObject<SrmRfqV[]>(JsonConvert.SerializeObject(data_v));

                        _srmRfqHService.Save(h, m, v);

                        transaction.Complete();
                        RfqNum = h.RfqNum;
                    }
                    catch (Exception ex)
                    {
                        transaction.Dispose();
                        throw new Exception(errTitle + ex.Message);
                    }
                    finally {
                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            _srmRfqHService.Delete(path);
                        }
                    }
                }
                var j = new JObject();
                j.Add("RfqNum", RfqNum);
                return Ok(j);
            }
            catch (Exception ex) {
                return this.BadRequestResult(ex.Message);
            }
        }
        private string updateFileNumber(string rfqNum, JObject rfq)
        {
            string guid = null;
            //有guid才替換
            if (rfq.Property("guid") != null)
            {
                guid = rfq["guid"].ToString();
                //有guid跟rfqnum才替換
                if (!string.IsNullOrEmpty(rfqNum) && !string.IsNullOrEmpty(guid))
                {
                    //有樣板才替換(user的werks跟functionid跟場內場外
                    UserClaims user = User.GetUserClaims();
                    if (_srmFileService.HasTemplate(user.Werks[0], 3,1))
                    {
                        string result = _srmFileService.UpdateNumber(rfqNum, guid);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }
                    }

                }
            }
            return string.Empty;
        }
    }
}
