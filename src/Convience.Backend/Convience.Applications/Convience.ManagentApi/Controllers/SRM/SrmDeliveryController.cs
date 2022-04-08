using Convience.Entity.Entity.SRM;
using Convience.JwtAuthentication;
using Convience.ManagentApi.Infrastructure.Authorization;
using Convience.ManagentApi.Infrastructure.Logs;
using Convience.Model.Models.ContentManage;
using Convience.Model.Models.SRM;
using Convience.Service.SRM;
using Convience.Service.SystemManage;
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
        private readonly IUserService _userService;

        public SrmDeliveryController(ISrmDeliveryService srmDeliveryService, IUserService userService)
        {
            _srmDeliveryService = srmDeliveryService;
            _userService = userService;
        }

        [HttpPost("AddDelivery")]
        public IActionResult AddDelivery(AddDeliveryModel data)
        {
            bool success = true;
            List<ViewSrmPoL> list1 = data.data.FindAll(p => p.Storage == "05Z1");
            List<ViewSrmPoL> list2 = data.data.FindAll(p => p.Storage != "05Z1");
            string deliveryNum = null;
            if (list1.Count > 0)
            {
                deliveryNum = _srmDeliveryService.AddDelivery(new AddDeliveryModel() { data = list1, date = data.date, vendorsn = data.vendorsn ,manager = data.manager});
                if (deliveryNum == null)
                {
                    success = false;
                }
            }
            if (list2.Count > 0)
            {
                deliveryNum = _srmDeliveryService.AddDelivery(new AddDeliveryModel() { data = list2, date = data.date, vendorsn = data.vendorsn, manager = data.manager });
                if (deliveryNum == null)
                {
                    success = false;
                }
            }
            if (success)
            {
                return Ok(deliveryNum);
            }
            //if (_srmDeliveryService.AddDelivery(data)) return Ok();
            return BadRequest("出貨單生成失敗");
        }
        [HttpPost("UpdateDeliveryL")]
        public IActionResult UpdateDeliveryL(ViewSrmDeliveryL dls)
        {
            if (_srmDeliveryService.UpdateDeliveryL(dls)) return Ok();
            return BadRequest("項次 新增/修改 失敗");
        }
        [HttpPost("DeleteDeliveryH")]
        public IActionResult DeleteDeliveryL(ViewSrmDeliveryH dh)
        {
            if (_srmDeliveryService.DeleteDeliveryH(dh)) return Ok();
            return BadRequest("出貨單 : "+dh.DeliveryNum.ToString()+" 刪除失敗!");
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
            string UserName = User.GetUserName();
            foreach (var item in dls)
            {
                item.LastUpdateBy = _userService.GetUserSapiId(UserName);
            }
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = JsonConvert.SerializeObject(dls);
                    HttpContent httpContent = new StringContent(json,
                                    Encoding.UTF8,
                                    "application/json");
                    HttpResponseMessage response = await client.PostAsync(Request.Scheme + "://" + Request.Host.Host + ":64779/api/srm/T_RECEIPT", httpContent);// + Query.RequestUri.Query);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = response.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrWhiteSpace(result) && result != "null")
                        {
                            return BadRequest(response.Content.ReadAsStringAsync().Result);
                        }
                        else
                        {
                            if (dls != null && dls.Count > 0)
                            {
                                string result_sql = _srmDeliveryService.ReceiveDeliveryL(dls);
                                if (string.IsNullOrEmpty(result_sql))
                                { }
                                else
                                { return BadRequest(result_sql); }

                            }
                            return Ok();
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

        [HttpPost("GetDeliveryExcel")]
        public IActionResult GetDeliveryExcel(AddDeliveryModel data)
        {
            //var stream = await _srmDeliveryService.DownloadAsync(viewModel);
            //return File(stream, "application/octet-stream", viewModel.Name);
            return BadRequest("出貨單生成失敗");
        }
        [HttpGet("SapDelivery")]
        public IActionResult SapDelivery()
        {
           string result= _srmDeliveryService.SapDelivery(User.GetUserName());
            if (string.IsNullOrWhiteSpace(result)) return Ok();
            return BadRequest(result);
        }
    }
}