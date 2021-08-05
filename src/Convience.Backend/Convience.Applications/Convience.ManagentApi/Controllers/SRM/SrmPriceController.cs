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
        private readonly ISrmRfqHService _srmRfqHService;

        public SrmPriceController(ISrmPriceService srmPriceService, ISrmQotService srmQotService, ISrmRfqHService srmRfqHService, ISrmRfqMService srmRfqMService)
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
        [HttpPost("Start")]
        public IActionResult Start(JObject jobj) {
            viewSrmInfoRecord[] infos = jobj["infos"].ToObject<viewSrmInfoRecord[]>();
            string logonid = jobj["logonid"].ToString();
            DateTime now = DateTime.Now;
            foreach (viewSrmInfoRecord info in infos)
            {
                    info.Status = (int)Status.初始;
                    info.CreateDate = now;
                    info.CreateBy = logonid;
                    info.LastUpdateDate = now;
                    info.LastUpdateBy = logonid;
            }
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    int? rfqId = _srmPriceService.Start(infos);
                    if (rfqId.HasValue) { _srmRfqHService.UpdateStatus((int)Status.簽核中, new SrmRfqH { RfqId = rfqId.Value, LastUpdateDate = now, LastUpdateBy = logonid }); }
                    transaction.Complete();
                    return Ok();
                }
                catch (Exception ex) {
                    transaction.Dispose();
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}
