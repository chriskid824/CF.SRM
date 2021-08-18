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
            var rfqno = (query["RFQ_NUM"] != null) ? query["RFQ_NUM"].ToString() : null;
            var matnr = (query["MATNR"] != null) ? query["MATNR"].ToString() : null;
            var status = (query["STATUS"] != null) ? (int)query["STATUS"] : 0;
            var vendor = "2";// (int)query["VENDOR"];  //???
            qot.rfqno = rfqno;
            qot.status = status;//(int)query["status"];
            qot.matnr = matnr;
            qot.vendor = vendor;

            var result = _srmQotService.GetQotList(qot);

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
        public IActionResult GetQotData(int id,int rfqid,int vendorid)
        {
            //for tree?
            //ViewSrmRfqH h = _srmRfqHService.GetDataByRfqId(id);
            //h.sourcerName = _userService.GetUsers(new UserQueryModel() { UserName = h.Sourcer, Page = 1, Size = 1 }).Data[0].Name;
            //h.C_by = _userService.GetUsers(new UserQueryModel() { UserName = h.CreateBy, Page = 1, Size = 1 }).Data[0].Name;
            System.Linq.IQueryable QotV = _srmQotService.GetQotData(id); //表單欄位用
            System.Linq.IQueryable matnr = _srmQotService.GetMatnrData(rfqid,vendorid); //表單欄位用

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
                m= matnr,
                //h = h,
                q = QotV,
                //v = v
            };
            //return this.BadRequestResult("12334");
            return Ok(qot);
        }
        [HttpPost("GetQotDetail")]
        //[Permission("price")]
        public IActionResult GetQotDetail(QueryQot query)
        {
            var qots = (_srmQotService.Get(query));
            ViewSrmPriceDetail detail = _srmQotService.GetDetail(qots);
            SrmRfqM m = new SrmRfqM()
            {
                RfqId = query.rfqId,
                MatnrId = query.matnrId
            };
            detail.matnr = _srmRfqMService.GetRfqMData(m);
            return Ok(detail);
        }
    }
}
