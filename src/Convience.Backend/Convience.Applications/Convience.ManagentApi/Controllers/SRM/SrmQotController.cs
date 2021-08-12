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
            var rfqno = (query["rfqno"] != null) ? query["rfqno"].ToString() : null;
            var matnr = (query["matnr"] != null) ? query["matnr"].ToString() : null;
            var status = (query["status"] != null) ? (int)query["status"] : 0;
            var vendor = 2;// (int)query["vendor"]; 
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
    }
}
