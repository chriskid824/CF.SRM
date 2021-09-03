using Convience.Entity.Entity.SRM;
using Convience.ManagentApi.Infrastructure.Authorization;
using Convience.ManagentApi.Infrastructure.Logs;
using Convience.Model.Models.ContentManage;
using Convience.Model.Models.SRM;
using Convience.Service.SRM;
using Convience.Util.Extension;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Headers;
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
        private readonly ISrmDeliveryService _srmDeliveryService;

        public SrmDeliveryController(ISrmDeliveryService srmDeliveryService)
        {
            _srmDeliveryService = srmDeliveryService;
        }

        [HttpPost("AddDelivery")]
        public IActionResult AddDelivery(List<ViewSrmPoL> pols)
        {
            if (_srmDeliveryService.AddDelivery(pols)) return Ok();
            return BadRequest("出貨單生成失敗");
        }
        [HttpPost("UpdateDeliveryL")]
        public IActionResult UpdateDeliveryL(ViewSrmDeliveryL dls)
        {
            if (_srmDeliveryService.UpdateDeliveryL(dls)) return Ok();
            return BadRequest("項次 新增/修改 失敗");
        }
        [HttpPost("DeleteDeliveryL")]
        public IActionResult DeleteDeliveryL(ViewSrmDeliveryL dls)
        {
            if (_srmDeliveryService.DeleteDeliveryL(dls)) return Ok();
            return BadRequest("項次 新增/修改 失敗");
        }
        [HttpPost("GetDelivery")]
        public string GetDelivery(JObject query)
        {
            if (query == null)
            {
                return null;
            }
            QueryPoList q = new QueryPoList();
            //var aaa = query.Property("poNum");
            q.deliveryNum = query["deliveryNum"].ToString();
            q.status = (int)query["status"];
            q.host = Request.Headers["Referer"].ToString() + "srm/deliveryreceive";
            q.user = User;
            var aaa = _srmDeliveryService.GetDelivery(q);

            return JsonConvert.SerializeObject(aaa, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
        }

        [HttpPost("GetDeliveryL")]
        public string GetDeliveryL(JObject query)
        {
            //var url=HttpContext.Current.Request.Url;
            if (query == null) return null;
            QueryPoList q = new QueryPoList();
            //var aaa = query.Property("poNum");
            q.deliveryNum = query["deliveryNum"].ToString();
            q.deliveryLId = string.IsNullOrWhiteSpace(query["deliveryLId"].ToString()) ? 0 : (int)query["deliveryLId"];
            q.user = User;
            if (string.IsNullOrWhiteSpace(q.deliveryNum) && q.deliveryLId == 0) return null;
            var aaa = _srmDeliveryService.GetDelivery(q);

            return JsonConvert.SerializeObject(aaa, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
        }
    }
}