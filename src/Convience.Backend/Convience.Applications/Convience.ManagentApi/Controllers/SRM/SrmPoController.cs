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
using System.Web;

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
        //public bool isValdeXml()
        //{
        //    string sss = "<CAT><NAME>Izzy</NAME><BREED>Siamese</BREED><AGE>6</AGE><ALTERED>yes</ALTERED><DECLAWED>no</DECLAWED><LICENSE>Izz138bod</LICENSE><OWNER>Colin Wilcox</OWNER></CAT>";
        //    List<string> tags = getalltags(sss);
        //    if (xmlrule1(tags) && xmlrule2(tags))
        //    {
        //        return true;
        //    }

        //    return false;
        //}
        //public List<string> getalltags(string xmlstring)
        //{
        //   List<string> xmltags = new List<string>();
        //    int lastindex = 0;
        //    while (lastindex < xmlstring.Length)
        //    {
        //        string remainString = xmlstring.Substring(lastindex, xmlstring.Length- lastindex);
        //        int pFrom = remainString.IndexOf("<") + 1;
        //        int pTo = remainString.IndexOf(">");
        //        if (pTo == -1)
        //        {
        //            return null;
        //        }
        //        lastindex = lastindex+pTo + 1;
        //        string result = remainString.Substring(pFrom, pTo - pFrom);
        //        xmltags.Add(result);
        //    }
        //    return xmltags;
        //}
        //public bool xmlrule1(List<string> tags)
        //{
        //    foreach (var tag in tags)
        //    {
        //        //找出是否全部都有前後對應
        //        string key = "";
        //        if (tag.StartsWith('/'))
        //        {
        //            key = tag.Substring(1);
        //        }
        //        else
        //        {
        //            key = "/" + tag;

        //        }
        //        if (!tags.Contains(key))
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}
        //public bool xmlrule2(List<string> tags)
        //{
        //    List<string> sttags = tags.FindAll(p => !p.StartsWith('/'));
        //    //找出中間any沒前後對應的
        //    foreach (var tag in sttags)
        //    {
        //        int startindex = tags.FindIndex(p => p == tag);
        //        string key = "/" + tag;
        //        if (tags.Contains(key))
        //        {
        //            int lastindex = tags.FindIndex(p=>p==key);
        //            for (int i = startindex+1; i < lastindex; i++)
        //            {

        //                if (i == 1 && tags[i].StartsWith('/'))
        //                {
        //                    return false;
        //                }
        //                if (!tags[i].StartsWith('/'))
        //                {
        //                    string innerkey = "/" + tags[i];
        //                    int innerkeyindex = tags.FindIndex(p => p == innerkey);
        //                    //如果中間有任何結尾的index不在st跟last中間 false
        //                    if (innerkeyindex > lastindex || innerkeyindex < 0)
        //                    {
        //                        return false;
        //                    }
        //                }

        //            }
        //        }                
        //    }
        //    return true;
        //}
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
            q.ekgryDesc = query["ekgryDesc"].ToString();
            q.org = (query["org"] == null || string.IsNullOrWhiteSpace(query["org"].ToString())) ? null : (int)query["org"];
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
            q.onlysevendays = query["onlysevendays"] == null ? true : Convert.ToBoolean(query["onlysevendays"]);
            q.org = (query["org"] == null || string.IsNullOrWhiteSpace(query["org"].ToString())) ? null : (int)query["org"];
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
        [HttpPost("UpdateReplyDeliveryDateWithReason")]
        public IActionResult UpdateReplyDeliveryDateWithReason(JObject po)
        {
            if (po.Property("PoId") == null || po.Property("date") == null|| po.Property("PoLId") == null || po.Property("Reason") == null) return Ok();
            int PoId = (int)po["PoId"];
            int PoLId = (int)po["PoLId"];
            DateTime date = Convert.ToDateTime(po["date"].ToString());
            string reason = po["Reason"].ToString();
            int poid = _srmDeliveryService.UpdateReplyDeliveryDateWithReason(PoId,PoLId, date, reason);

            //_srmPoService.UpdateStatus(poid, 15);
            return Ok();
        }
        [HttpPost("UpdateReplyDeliveryDate")]
        public IActionResult UpdateReplyDeliveryDate(JObject po)
        {
            if (po.Property("ReplyDeliveryDate") == null) return Ok();
            SrmPoL data = po.ToObject<SrmPoL>();
            data.Status = 15;
            _srmDeliveryService.UpdateReplyDeliveryDate(data);
            if (_srmPoService.CheckAllReply(data.PoId))
            {
                _srmPoService.UpdateStatus(data.PoId, 15);
            }
            return Ok();
        }
        [HttpPost("UpdateReplyDeliveryDateH")]
        public IActionResult UpdateReplyDeliveryDateH(JObject po)
        {
            if (po.Property("PoNum") == null|| po.Property("date") == null) return Ok();
            string PoNum = po["PoNum"].ToString();
            DateTime date = (DateTime)po["date"];
            int poid=_srmDeliveryService.UpdateReplyDeliveryDateH(PoNum, date);
            _srmPoService.UpdateStatus(poid, 15);
            return Ok();
        }
        [HttpGet("UpdateStatus")]
        public IActionResult UpdateStatus(int id)
        {
            //int data= id.ToObject<int>();
            _srmPoService.UpdateStatus(id, 15);
            return Ok();
        }
        [HttpGet("UpdateStatus_Reply")]
        public IActionResult UpdateStatus_Reply(int poid,int polid)
        {
            //int data= id.ToObject<int>();
            _srmPoService.UpdateStatus(poid, polid, 15);
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
                    string ponum = data["I_EBELN"].ToString();
                    string polid = data["I_EBELP"].ToString();
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
                            List<BaseFileData> fdList = _srmPoService.GetMatnrDocListFromSap(ponum, Convert.ToInt32(polid), dataSet, User.GetIsVendor());
                            //returndata.List = _srmPoService.UpdateSapData(dataSet, User.GetUserName());
                            return Ok(fdList);
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

        [HttpPost("GetPoPoL")]
        [Permission("po-examine")]
        public IActionResult GetPoPoL(JObject query)
        {
            QueryPoList q = new QueryPoList();
            q.poNum = query["poNum"] == null ? null : query["poNum"].ToString();
            q.status = query["status"] == null ? 0 : (int)query["status"];
            q.ekgryDesc = query["ekgryDesc"].ToString();
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
            BasicHttpsBinding binding = new BasicHttpsBinding();
            binding.MaxReceivedMessageSize = 20000000;
            binding.MaxBufferSize = 20000000;
            binding.MaxBufferPoolSize = 20000000;
            binding.AllowCookies = true;
            EndpointAddress address = new EndpointAddress("https://cfns178.chenfull.com.tw/CF.DMS.Service/API.asmx");
            if (_appSettingsService.Environment == "dev")
            {
                //address = new EndpointAddress("http://10.88.1.90/CF.DMS.Service/API.asmx");
                //address = new EndpointAddress("http://localhost/CF.DMS.Service/API.asmx");
                address = new EndpointAddress("https://cfns178.chenfull.com.tw/CF.DMS.Service/API.asmx");
            }
            else
            {
                address = new EndpointAddress("https://cfns178.chenfull.com.tw/CF.DMS.Service/API.asmx");
                //address = new EndpointAddress("http://10.88.1.90/CF.DMS.Service/API.asmx");
                //http://10.88.1.90/CF.DMS.Service/API.asmx
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
        public async Task<IActionResult> DownloadFilePath(string path,string file_name,int po_id,int po_l_id)
        {
            var stream = await _srmFileService.DownloadTempAsync(path);
            stream.Position = 0;
            try
            {
                string remoteIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                if (!string.IsNullOrEmpty(remoteIpAddress) && System.Text.RegularExpressions.Regex.IsMatch(remoteIpAddress, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
                {
                }
                else
                {
                    remoteIpAddress = "127.0.0.1";
                }
                _srmPoService.addLog(po_id, po_l_id, file_name, remoteIpAddress, User.GetUserName());
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

        [HttpPost("UpdatePoLDoc")]
        public IActionResult UpdatePoLDoc(JObject query)
        {
            int matnr_id = Convert.ToInt32(query["I_MATNR"]);
            string Des = query["DESCRIPTION"].ToString();
            int vendor_id = Convert.ToInt32(query["VENDOR_ID"]);
            List<BaseFileData> fdList = JsonConvert.DeserializeObject<List<BaseFileData>>(query["fileNameList"].ToString());
            string result=_srmPoService.UpdatePoLDoc(matnr_id,Des, vendor_id, fdList, User.GetUserName());
            if (string.IsNullOrEmpty(result)) return Ok();
            return BadRequest(result);
        }

        [HttpPost("GetDownLoadLog")]
        public IActionResult GetDownLoadLog(QueryPoDownloadLogList query)
        {
            query.user = User;
            var h = _srmPoService.GetDownloadList(query);
            return Ok(h);
        }

        [HttpPost("GetPoLAbnormal")]
        public string GetPoLAbnormal(JObject query)
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
            q.srmvendor = query["srmvendor"] == null ? null : query["srmvendor"].ToString();
            q.poNum = query["poNum"] == null ? null : query["poNum"].ToString();

            q.status = query["status"] == null ? 0 : (int)query["status"];
            q.user = User;
            var aaa = _srmPoService.GetPoLAbnormal(q);

            return JsonConvert.SerializeObject(aaa, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
        }
        [HttpPost("GetOrg")]
        public IActionResult GetOrg(QueryPoList query)
        {
            query.user = User;
            var h = _srmPoService.GetOrg(query);
            return Ok(h);
        }
    }

}
