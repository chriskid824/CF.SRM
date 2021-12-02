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
    public class SrmQotController : ControllerBase
    {
        private readonly ISrmQotService _srmQotService;
        private readonly ISrmRfqMService _srmRfqMService;

        public SrmQotController(ISrmQotService srmQotService, ISrmRfqMService srmRfqMService)
        {
            _srmQotService = srmQotService;
            _srmRfqMService = srmRfqMService;
        }
        [HttpPost("GetQotList")]
        public string GetQotList(JObject query)
        {
            if (query == null)
            {
                return JsonConvert.SerializeObject(_srmQotService.GetQotList(), Formatting.None,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
            }
            QueryQotList qot = new QueryQotList();
            //var aaa = query.Property("poNum");
            if (query.Property("rfqId") != null)
            {
                qot.rfqId = (int)query["rfqId"];
            }
            var rfqno = (query["RFQ_NUM"] != null) ? query["RFQ_NUM"].ToString() : null;
            var matnr = (query["MATNR"] != null) ? query["MATNR"].ToString() : null;
            var status = (query["STATUS"] != null) ? (int)query["STATUS"] : 0;
            var vendor = (string)query["vendor"];
            qot.rfqno = rfqno;
            qot.status = status;//(int)query["status"];
            qot.matnr = matnr;
            qot.vendor = vendor;
            var result =  _srmQotService.GetQotList(qot);
            //if (query["vendor"].ToString().IndexOf("admin") != -1)
            //{
            //    result = _srmQotService.GetQotListByAdmin(qot);
            //}
            
            //var result = _srmQotService.GetQotListByAdmin(qot); 

            return JsonConvert.SerializeObject(result, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
        }
        //[HttpGet("GetQotData")]
        //public IActionResult GetQotData(int id)
        //{
        //    //ViewQot qot = (ViewQot)_srmQotService.GetDataBQotId(id);          
        //    //return this.BadRequestResult("12334");
        //    return Ok(
        //        );
        //}

        [HttpGet("GetQotData")]
        public IActionResult GetQotData(int id, int rfqid, int vendorid)
        {
            //for tree?
            //ViewSrmRfqH h = _srmRfqHService.GetDataByRfqId(id);
            //h.sourcerName = _userService.GetUsers(new UserQueryModel() { UserName = h.Sourcer, Page = 1, Size = 1 }).Data[0].Name;
            //h.C_by = _userService.GetUsers(new UserQueryModel() { UserName = h.CreateBy, Page = 1, Size = 1 }).Data[0].Name;
            
            
            System.Linq.IQueryable QotV = _srmQotService.GetQotData(rfqid, vendorid,id); //表單欄位用
            System.Linq.IQueryable matnr = _srmQotService.GetMatnrData(rfqid, vendorid); //表單欄位用

            //ViewSrmRfqV[] v = _srmRfqVService.GetDataByRfqId(id);
            //Newtonsoft.Json.JsonSerializer js = new Newtonsoft.Json.JsonSerializer();
            //js.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            ////JObject rfq = new JObject() {
            ////    { "h",JObject.FromObject(h,js)},
            ////    { "m",JArray.FromObject(m,js)},
            ////    { "v",JArray.FromObject(v,js)},
            ////};
            ResultQotModel qot = new ResultQotModel()
            {
                m = matnr,
                //h = h,
                q = QotV,
                //v = v
            };
            //return this.BadRequestResult("12334");
            return Ok(qot);
        }
        [HttpPost("GetQotInfo")]
        public IActionResult GetQotDetail(QueryQot query)
        {
            var qots = (_srmQotService.GetByVendor(query));
            //var qots = (_srmQotService.Get(query)); //因admin需看到全部
            ViewQotResult detail = _srmQotService.GetDetail(query);
            SrmRfqM m = new SrmRfqM()
            {
                RfqId = query.rfqId,
                MatnrId = query.matnrId
            };
            detail.matnr = _srmRfqMService.GetRfqMData(m);
            detail.qot = _srmQotService.GetQot( query);
            return Ok(detail);
        }

        [HttpPost("Save")]
        //[Permission("rfq")]//???
        public IActionResult Save(JObject qot)
        {
            SrmQotH q = qot["q"].ToObject<SrmQotH>();
            SrmQotMaterial[] m = qot["material"].ToObject<SrmQotMaterial[]>();
            SrmQotSurface[] s = qot["surface"].ToObject<SrmQotSurface[]>();
            SrmQotProcess[] p = qot["process"].ToObject<SrmQotProcess[]>();
            SrmQotOther[] o = qot["other"].ToObject<SrmQotOther[]>();
            DateTime now = DateTime.Now;
            q.LastUpdateDate = now;
            _srmQotService.Save(q, m, s, p, o);
            return Ok();
        }

        
        [HttpPost("Reject")]
        //[Permission("rfq")]
        public IActionResult Reject(JObject qot)
        {
            try
            {
                SrmQotUpdateMaterial q = qot["q"].ToObject<SrmQotUpdateMaterial>();           
                q.LastUpdateDate = DateTime.Now;
                _srmQotService.UpdateQotStatus(((int)Status.拒絕), q);
                _srmQotService.InsertRejectReason(q);
                //確認所有qot
                bool IfUpdateRfq = _srmQotService.CheckAllQot(q);
                //更新rfq
                if (IfUpdateRfq)
                {                   
                    _srmQotService.UpdateRfqStatus(((int)Status.確認), q);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return this.BadRequestResult(ex.Message);
            }
        }

        

        
        [HttpPost("GetRowNum")]
        //[Permission("rfq")]//???
        public int GetRowNum(JObject qot)
        {
            int index = 0;

            SrmQotH q = qot["q"].ToObject<SrmQotH>();
            index = _srmQotService.GetRowNum(q);
            return index;
        }

        [HttpPost("GetProcess")]
        public IActionResult GetProcess()
        {
            return Ok(_srmQotService.GetProcess());
        }
        [HttpPost("GetMaterial")]
        public IActionResult GetMaterial()
        {
            return Ok(_srmQotService.GetMaterial());
        }
        [HttpPost("GetProcessByNum")]
        public IActionResult GetProcessByNum(int num)
        {
            return Ok(_srmQotService.GetProcessByNum(num));
        }
        [HttpPost("Send")]
        //[Permission("rfq")]//???
        public IActionResult Send(JObject qot)
        {
            SrmQotH q = qot["q"].ToObject<SrmQotH>();
            SrmQotMaterial[] m = qot["material"].ToObject<SrmQotMaterial[]>();
            SrmQotSurface[] s = qot["surface"].ToObject<SrmQotSurface[]>();
            SrmQotProcess[] p = qot["process"].ToObject<SrmQotProcess[]>();
            SrmQotOther[] o = qot["other"].ToObject<SrmQotOther[]>();
            DateTime now = DateTime.Now;
            q.LastUpdateDate = now;
            _srmQotService.Save(q, m, s, p, o);
            SrmQotUpdateMaterial qu = qot["q"].ToObject<SrmQotUpdateMaterial>();
            _srmQotService.UpdateQotStatus(((int)Status.確認), qu);
            //確認所有qot
            bool IfUpdateRfq = _srmQotService.CheckAllQot(q);
            //更新rfq
            if (IfUpdateRfq)
            {
                _srmQotService.UpdateRfqStatus(((int)Status.確認), qu);
            }
            return Ok();
        }
        /*[HttpPost("SendAllQot")]
        //[Permission("rfq")]//???
        public string SendAllQot(JObject qot)
        {
            //int vendorid = _srmQotService.GetVendorId(qot);
            //string msg = _srmQotService.SendAll(int.Parse(qot["q"]["RfqId"].ToString()), int.Parse(qot["q"]["vendorId"].ToString()));
            //無錯誤訊息則變更狀態:初始 -> 
            if (string.IsNullOrWhiteSpace(msg)) 
            {
                //SrmQotUpdateMaterial qu = qot["q"].ToObject<SrmQotUpdateMaterial>();
                //string aa = qot["q"][""].ToString()
                //_srmQotService.UpdateQotStatus(((int)Status.確認), qu);
                ////確認所有qot
                //bool IfUpdateRfq = _srmQotService.CheckAllQot(q);
                ////更新rfq
                //if (IfUpdateRfq)
                //{
                //    _srmQotService.UpdateRfqStatus(((int)Status.確認), qu);
                //}
            }
            return msg;
        }*/
    }
}
