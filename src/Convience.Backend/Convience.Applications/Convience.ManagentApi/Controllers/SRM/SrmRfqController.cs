using Convience.Entity.Entity.SRM;
using Convience.ManagentApi.Infrastructure.Authorization;
using Convience.ManagentApi.Infrastructure.Logs;
using Convience.Model.Models.ContentManage;
using Convience.Service.SRM;
using Convience.Util.Extension;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
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

        public SrmRfqController(ISrmMatnrService srmMatnrService, ISrmVendorService srmVendorService, ISrmRfqHService srmRfqHService, ISrmRfqMService srmRfqMService, ISrmRfqVService srmRfqVService)
        {
            _srmMatnrService = srmMatnrService;
            _srmVendorService = srmVendorService;
            _srmRfqHService = srmRfqHService;
            _srmRfqMService = srmRfqMService;
            _srmRfqVService = srmRfqVService;
        }

        [HttpGet("GetMatnr")]
        public IActionResult GetMatnr(string matnr = "")
        {
            return Ok(_srmMatnrService.GetMatnr(matnr));
        }

        [HttpGet("test")]
        public System.Collections.Generic.IEnumerable<SrmMatnr> GetMatnr2(string matnr = "")
        {
            var a = _srmMatnrService.GetMatnr(matnr);
            return a;
        }

        [HttpGet("GetVendor")]
        public IActionResult GetVendor(string vendor = "") {
            return Ok(_srmVendorService.GetVendor(vendor));
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
        //public class rfq {
        //    public SrmRfqH h { get; set; }
        //    public System.Collections.Generic.IEnumerable<SrmRfqM> m { get; set; }
        //    public System.Collections.Generic.IEnumerable<SrmRfqV> v { get; set; }
        //};
        [HttpGet("GetRfqData")]
        public IActionResult GetRfqData(int id) {
            SrmRfqH h = _srmRfqHService.GetDataByRfqId(id);
            System.Linq.IQueryable m = _srmRfqMService.GetDataByRfqId(id);
            System.Linq.IQueryable v = _srmRfqVService.GetDataByRfqId(id);
            Newtonsoft.Json.JsonSerializer js = new Newtonsoft.Json.JsonSerializer();
            js.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            JObject rfq = new JObject() {
                { "h",JObject.FromObject(h,js)},
                { "m",JArray.FromObject(m,js)},
                { "v",JArray.FromObject(v,js)},
            };
            return Ok(rfq);
        }
    }
}
