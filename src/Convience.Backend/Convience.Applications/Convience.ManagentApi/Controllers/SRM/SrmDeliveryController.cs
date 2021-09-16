using Convience.Entity.Entity.SRM;
using Convience.JwtAuthentication;
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
        [HttpPost("ReceiveDeliveryL")]
        public async Task<IActionResult> ReceiveDeliveryLAsync(List<ViewSrmDeliveryL> dls)
        {
            string Name = User.GetName();
            foreach (var item in dls)
            {
                item.LastUpdateBy = Name;
            }
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = JsonConvert.SerializeObject(dls);
                    HttpContent httpContent = new StringContent(json,
                                    Encoding.UTF8,
                                    "application/json");
                    HttpResponseMessage response = await client.PostAsync("http://10.88.1.28:64779/api/srm/T_RECEIPT", httpContent);// + Query.RequestUri.Query);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = response.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrWhiteSpace(result) && result != "null")
                        {
                            return BadRequest(response.Content.ReadAsStringAsync().Result);
                        }
                    }
                    else
                    {
                        return BadRequest("修改資料在sap階段失敗 請聯絡工程師調整");
                    }
                }
            }
            catch (Exception e)
            {
                //throw e;
                return BadRequest("修改資料在sap階段失敗 請聯絡工程師調整");
            }
            if (dls != null && dls.Count > 0)
            {
                string result = _srmDeliveryService.ReceiveDeliveryL(dls);
                if (string.IsNullOrEmpty(result)) return Ok();
                return BadRequest(result);
            }
            //if (_srmDeliveryService.DeleteDeliveryL(dls)) return Ok();
            return BadRequest("項次 新增/修改 失敗");
        }
        [HttpPost("GetDelivery")]
        public string GetDelivery(JObject query)
        {
            //SapDeliveryService sc = new SapDeliveryService();
            //sc.test();
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