using Convience.Entity.Entity.SRM;
using Convience.JwtAuthentication;
using Convience.ManagentApi.Infrastructure.Authorization;
using Convience.ManagentApi.Infrastructure.Logs;
using Convience.Model.Models.ContentManage;
using Convience.Model.Models.SRM;
using Convience.Service.ContentManage;
using Convience.Service.SRM;
using Convience.Util.Extension;
using DMSAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Text;
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
        private readonly ISrmVendorService _srmVendorService;
        private readonly ISrmFileService _srmFileService;
        private readonly appSettings _appSettingsService;

        public SrmPoController(ISrmPoService srmPoService, ISrmDeliveryService srmDeliveryService, ISrmVendorService srmVendorService, ISrmFileService srmFileService, IOptions<appSettings> appSettingsOption)
        {
            _srmPoService = srmPoService;
            _srmDeliveryService = srmDeliveryService;
            _srmVendorService = srmVendorService;
            _srmFileService = srmFileService;
            _appSettingsService = appSettingsOption.Value;
        }

        [HttpPost("GetPo")]
        public string GetPo(JObject query)
        {
            //var bbb= await _srmVendorService.AddUserFromVendor();
            //UserClaims user = User.GetUserClaims();
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
            if (query.Property("poId")!=null)
            {
                q.poId = (int)query["poId"];
            }

            q.poNum = query["poNum"].ToString();
            q.status = (int)query["status"];
            q.dataStatus = query["dataStatus"] == null ? 0 : (int)query["dataStatus"];
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
            q.dataStatus = query["dataStatus"] == null ? 0 : (int)query["dataStatus"];
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
            if (rfq.Property("ReplyDeliveryDate") == null) return Ok();
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
        [HttpPost("Sap_GetPoData")]
        public async Task<IActionResult> Sap_GetPoData(JObject data)
        {
            SapResultList returndata = new SapResultList();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = JsonConvert.SerializeObject(data);
                    HttpContent httpContent = new StringContent(json,
                                    Encoding.UTF8,
                                    "application/json");
                    HttpResponseMessage response = await client.PostAsync(Request.Scheme+"://"+ Request.Host.Host+":64779/api/srm/GetPoData", httpContent);// + Query.RequestUri.Query);
                    string result = response.Content.ReadAsStringAsync().Result;
                    
                    if (response.IsSuccessStatusCode)
                    {

                        if (!string.IsNullOrWhiteSpace(result) && result != "null")
                        {
                            SapPoData dataSet = JsonConvert.DeserializeObject<SapPoData>(result);
                            returndata.List = _srmPoService.UpdateSapData(dataSet, User.GetUserName());
                            return Ok(returndata);
                        }
                    }
                    else
                    {
                        returndata.err = result;
                        return BadRequest(returndata);
                    }
                }
            }
            catch (Exception e)
            {
                //throw e;'
                returndata.err = "修改資料在sap階段失敗 請聯絡工程師調整";
                return BadRequest(returndata);
            }
            //if (dls != null && dls.Count > 0)
            //{
            //    string result = _srmDeliveryService.ReceiveDeliveryL(dls);
            //    if (string.IsNullOrEmpty(result)) return Ok();
            //    return BadRequest(result);
            //}
            //if (_srmDeliveryService.DeleteDeliveryL(dls)) return Ok();
            returndata.err = "項次 新增/修改 失敗";
            return BadRequest(returndata);
        }

        [HttpPost("Sap_GetPoDoc")]
        public async Task<IActionResult> Sap_GetPoDoc(JObject data)
        {
            //SapResultList returndata = new SapResultList();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = JsonConvert.SerializeObject(data);
                    HttpContent httpContent = new StringContent(json,
                                    Encoding.UTF8,
                                    "application/json");
                    HttpResponseMessage response = await client.PostAsync(Request.Scheme + "://" + Request.Host.Host + ":64779/api/srm/RFC_GET_MATNR_DOC", httpContent);// + Query.RequestUri.Query);
                    string result = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {

                        if (!string.IsNullOrWhiteSpace(result) && result != "null")
                        {
                            List<T_DRAD> dataSet = JsonConvert.DeserializeObject<List<T_DRAD>>(JObject.Parse(result)["T_DRAD"].ToString());
                            //returndata.List = _srmPoService.UpdateSapData(dataSet, User.GetUserName());
                            return Ok(dataSet);
                        }
                    }
                    else
                    {
                        return BadRequest(result);
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest("Sap取得資料失敗");
            }
            //if (dls != null && dls.Count > 0)
            //{
            //    string result = _srmDeliveryService.ReceiveDeliveryL(dls);
            //    if (string.IsNullOrEmpty(result)) return Ok();
            //    return BadRequest(result);
            //}
            //if (_srmDeliveryService.DeleteDeliveryL(dls)) return Ok();
            return BadRequest("Sap取得資料失敗");
        }

        public void GetDoc()
        {
            
        }

        [HttpPost("GetPoPoL")]
        [Permission("po-examine")]
        public IActionResult GetPoPoL(JObject query)
        {
            QueryPoList q = new QueryPoList();
            q.poNum = query["poNum"] == null ? null : query["poNum"].ToString();
            q.status = query["status"] == null ? 0 : (int)query["status"];
            q.buyer = query["buyer"].ToString();
            q.user = User;
            int page = (int)query["page"];
            int size = (int)query["size"];
            var h = _srmPoService.GetPoPoL(q, page, size);
            return Ok(h);
        }
        [HttpGet("DownloadFileUrl")]
        public async Task<IActionResult> DownloadFileUrl(string file_name)
        {
            //string file_name = viewModel.ToString();
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 20000000;
            binding.MaxBufferSize = 20000000;
            binding.MaxBufferPoolSize = 20000000;
            binding.AllowCookies = true;
            EndpointAddress address = new EndpointAddress("http://10.1.1.180/CF.DMS.Service/API.asmx");
            if (_appSettingsService.Environment == "dev")
            {
                address = new EndpointAddress("http://10.1.1.180/CF.DMS.Service/API.asmx");
            }
            else
            {
                address = new EndpointAddress("http://10.1.1.180/CF.DMS.Service/API.asmx");
            }
            APISoapClient dms = new APISoapClient(binding, address);
            CallMethodParams callMethodParm = new CallMethodParams();//GetBorgEmpDataByLogonID
            callMethodParm.Method = "DownloadFileSrm";
            callMethodParm.Token = "token";
            //callMethodParm.Options = "464805A0-5EB6-4B45-B1AB-1B172E78B07D";
            JObject param = new JObject();
           // FileObj fileObj = new FileObj();
            //fileObj.file_id = "464805A0-5EB6-4B45-B1AB-1B172E78B07D";
            param.Add("file_name", file_name);
            param.Add("auth_type", "user");
            param.Add("ds", "111");
            //callMethodParm.Method = "UpdateCF";
            callMethodParm.Options = JsonConvert.SerializeObject(param);
            Task<CallMethodResponse> response = dms.CallMethodAsync(callMethodParm);
            CallMethodResult result = response.Result.Body.CallMethodResult;
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            byte[] bytes = Convert.FromBase64String(result.Options.ToString());
            string path = await _srmFileService.UploadPoTempAsync(bytes, result.Message.ToString());
            string aaa = path;
            BaseFileData fd = new BaseFileData() {
                Name = result.Message.ToString(),
                Path = path,
            };
            return Ok(JsonConvert.SerializeObject(fd));
        }

        [HttpGet("DownloadFilePath")]
        public async Task<IActionResult> DownloadFilePath(string path,string file_name)
        {
            var stream = await _srmFileService.DownloadTempAsync(path);
            stream.Position = 0;
            try
            {
                return File(stream, "application/octet-stream", file_name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
            //var ms = new MemoryStream();
            ////doc.SaveAs(ms);
            ////doc.Dispose();
            //ms.Write(bytes, 0, bytes.Length);
            //ms.Position = 0;
            //if (!System.IO.Directory.Exists("temp"))
            //{
            //    System.IO.Directory.CreateDirectory("temp");//不存在就建立目錄
            //}
            //try
            //{
            //    File(bytes, "temp/" + result.Message);
            //}
            //catch (Exception e)
            //{
            //    string aaa = "aaa";
            //}
            ////return File(ms, "application/stream", result.Message);
            //using (FileStream fs = new FileStream(result.Message, FileMode.OpenOrCreate))
            //{
            //    fs.Write(bytes, 0, bytes.Length);
            //    fs.Position = 0;
            //    //return File(fs, "application/octet-stream", result.Message);
            //}


        }

    }

}
