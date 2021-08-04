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
    public class SrmPriceController : ControllerBase
    {
        private readonly ISrmPriceService _srmPriceService;
        private readonly ISrmQotService _srmQotService;
        private readonly ISrmRfqMService _srmRfqMService;

        public SrmPriceController(ISrmPriceService srmPriceService, ISrmQotService srmQotService, ISrmRfqMService srmRfqMService)
        {
            _srmQotService = srmQotService;
            _srmPriceService = srmPriceService;
            _srmRfqMService = srmRfqMService;
        }
        [HttpPost("GetQotDetail")]
        public IActionResult GetQotDetail(QueryQot query)
        {
            var qots = (_srmQotService.Get(query));
            ViewSrmPriceDetail detail = _srmPriceService.GetDetail(qots);
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
