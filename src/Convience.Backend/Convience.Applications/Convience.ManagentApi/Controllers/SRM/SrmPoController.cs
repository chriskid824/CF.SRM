using Convience.Entity.Entity.SRM;
using Convience.JwtAuthentication;
using Convience.ManagentApi.Infrastructure.Authorization;
using Convience.ManagentApi.Infrastructure.Logs;
using Convience.Model.Models.ContentManage;
using Convience.Model.Models.SRM;
using Convience.Service.SRM;
using Convience.Util.Extension;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Convience.ManagentApi.Controllers.SRM
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SrmPoController : ControllerBase
    {
        private readonly ISrmPoService _srmPoService;
        private readonly ISrmDeliveryService _srmDeliveryService;

        public SrmPoController(ISrmPoService srmPoService, ISrmDeliveryService srmDeliveryService)
        {
            _srmPoService = srmPoService;
            _srmDeliveryService = srmDeliveryService;
        }

        [HttpPost("GetPo")]
        public string GetPo(JObject query)
        {
            UserClaims user = User.GetUserClaims();
            if (query == null)
            {
                return JsonConvert.SerializeObject(_srmPoService.GetAll(), Formatting.None,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
            }
            QueryPoList q = new QueryPoList();
            //var aaa = query.Property("poNum");
            q.poNum = query["poNum"].ToString();
            q.status = (int)query["status"];
            q.buyer = query["buyer"].ToString();
            q.user = User;
            var aaa = _srmPoService.GetAll(q);

            return JsonConvert.SerializeObject(aaa, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
        }
        [HttpPost("GetPoL")]
        public string GetPoL(JObject query)
        {
            if (query == null)
            {
                return JsonConvert.SerializeObject(_srmPoService.GetAll(), Formatting.None,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
            }
            QueryPoList q = new QueryPoList();
            q.poNum = query["poNum"] == null ? null : query["poNum"].ToString();
            q.poLId = query["poLId"] == null ? 0 : (int)query["poLId"];
            q.status = query["status"] == null ? 0 : (int)query["status"];
            q.replyDeliveryDate_s = (query["replyDeliveryDate_s"] == null || !query["replyDeliveryDate_s"].HasValues) ? null : Convert.ToDateTime(query["replyDeliveryDate_s"]);
            q.replyDeliveryDate_e = (query["replyDeliveryDate_e"] == null || !query["replyDeliveryDate_e"].HasValues) ? null : Convert.ToDateTime(query["replyDeliveryDate_e"]);
            q.user = User;
            var aaa = _srmPoService.GetPoL(q);

            return JsonConvert.SerializeObject(aaa, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
        }
        [HttpPost("UpdateReplyDeliveryDate")]
        public IActionResult UpdateReplyDeliveryDate(JObject rfq)
        {
            SrmPoL data = rfq.ToObject<SrmPoL>();
            _srmDeliveryService.UpdateReplyDeliveryDate(data);
            if (_srmPoService.CheckAllReply(data.PoId))
            {
                _srmPoService.UpdateStatus(data.PoId, 15);
            }
            return Ok();
        }
        [HttpGet("UpdateStatus")]
        public IActionResult UpdateStatus(int id)
        {
            //int data= id.ToObject<int>();
            _srmPoService.UpdateStatus(id, 11);
            return Ok();            
        }
    }
}
