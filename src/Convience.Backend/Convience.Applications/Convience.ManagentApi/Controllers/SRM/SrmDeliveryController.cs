using Convience.Entity.Entity.SRM;
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
    public class SrmDeliveryController : ControllerBase
    {
        private readonly ISrmPoService _srmPoService;

        public SrmDeliveryController(ISrmPoService srmPoService)
        {
            _srmPoService = srmPoService;
        }

        [HttpPost("AddDelivery")]
        public IActionResult AddDelivery(List<ViewSrmPoL> pols)
        {
            if (_srmPoService.AddDelivery(pols)) return Ok();
            return BadRequest("出貨單生成失敗");
        }

        [HttpPost("GetDelivery")]
        public string GetDelivery(JObject query)
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
            //var aaa = query.Property("poNum");
            q.deliveryNum = query["deliveryNum"].ToString();
            q.status = (int)query["status"];
            q.host = Request.Host.ToString();
            var aaa = _srmPoService.GetDelivery(q);

            return JsonConvert.SerializeObject(aaa, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
        }

        [HttpGet("GetDeliveryL")]
        public string GetDeliveryL(int deliveryLId, string deliveryNum)
        {
            //var url=HttpContext.Current.Request.Url;
            if ((string.IsNullOrEmpty(deliveryNum) || deliveryNum == "0") && deliveryLId == 0) return null;
            QueryPoList q = new QueryPoList();
            //var aaa = query.Property("poNum");
            q.deliveryNum = deliveryNum;
            q.deliveryLId = deliveryLId;
            q.host = Request.Host.ToString();
            var aaa = _srmPoService.GetDelivery(q);

            return JsonConvert.SerializeObject(aaa, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
        }
    }
}