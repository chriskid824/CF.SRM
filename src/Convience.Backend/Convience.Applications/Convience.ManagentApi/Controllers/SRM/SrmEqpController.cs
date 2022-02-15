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
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Convience.ManagentApi.Controllers.Extension;
using BPMAPI;
using System.ServiceModel;
using System.Data;
using Convience.JwtAuthentication;
using System.Net.Http;
using System.Net;
using System.IO;

namespace Convience.ManagentApi.Controllers.SRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SrmEqpController : ControllerBase
    {
        private readonly appSettings _appSettingsService;
        private readonly ISrmEqpService _srmEqpService;
        private readonly IUserService _userService;
        private readonly ISrmRfqHService _srmRfqHService;
        private readonly ISrmFileService _srmFileService;

        public SrmEqpController(ISrmEqpService srmEqpService,
                                IOptions<appSettings> appSettingsOption,
                                ISrmRfqHService srmRfqHService,
                                ISrmFileService srmFileService
                                )
        {
            _srmEqpService = srmEqpService;
            _appSettingsService = appSettingsOption.Value;
            _srmFileService = srmFileService;
            _srmRfqHService = srmRfqHService;
        }


        [HttpPost("GetPoData")]
       
        public IActionResult GetPoData(QueryEqp query) 
        {         
            IEnumerable<ViewEqpList> eqplist = _srmEqpService.GetPoData( query);
            if (eqplist.Count() == 0)
            {
                return BadRequest("查無對應採購單資訊!");
            }
            else 
            {
                return Ok(eqplist);
            }
            
        }
        [HttpPost("GetEqpList")]
        [Permission("eqplist")]
        public IActionResult GetEqpList(JObject query)
        {
            QueryEqp e = new QueryEqp();

            e.txtSN = (query["eqpNum"] != null) ? query["eqpNum"].ToString() : null;
            e.woNum = (query["woNum"] != null) ? query["woNum"].ToString() : null;
            e.no = (query["no"] != null) ? query["no"].ToString() : null;
            e.matnr = (query["matnr"] != null) ? query["matnr"].ToString() : null;
            e.vendor = (query["vendor"] != null) ? query["vendor"].ToString() : null;
            //e.vendorid = query["vendor"];
            UserClaims user = User.GetUserClaims();
            //admin可看全部
            if (!user.IsVendor)
            {
                if (user.UserName.Substring(0, 5) == "admin")
                {
                    e.vendorid = 0;
                }
            }
            else
            {
                e.vendorid = _srmEqpService.GetVendorId(e);//13;
            }

            //e.vendorid = _srmEqpService.GetVendorId(e);//13;
            //e.vendorid = 13;//帶供應商id????? 要拿掉
            int status = (query["status"] != null) ? (int)query["status"] : 1; //沒值預設初始
            e.status = status;

            int page = (int)query["page"];
            int size = (int)query["size"];
            
            var h = _srmEqpService.GetEqpList(e, page, size);
            return Ok(h);
        }
       
        [HttpPost("Save")]
        //[Permission("eqp")]
        public IActionResult Save(JObject eqp)
        {
            

            string msg = string.Empty;
            try
            {
                SrmEqpH h = eqp["h"].ToObject<SrmEqpH>();
                //QueryEqp aa = new QueryEqp();
                //aa.txtSN = "PS22022";
                //fortest
                //int caseid = _srmEqpService.GetCaseId(aa);
                //int caseid = GetCaseId("PS22022"); //取得博格路徑           
                //GetFiles(aa.txtSN, caseid);
                //fortest


                DateTime now = DateTime.Now;
                UserClaims user = User.GetUserClaims();
                h.LastUpdateBy = user.UserName;
                h.LastUpdateDate = now;
                h.Status = 1;
                JObject j = _srmEqpService.Save(h);
                msg = j["msg"].ToString();
                //msg = j
                //msg = _srmEqpService.Save(h);
                string txtSN = j["txtSN"].ToString();
                string result = updateFileNumber(txtSN, h);
                if (!string.IsNullOrEmpty(result))
                {
                    return this.BadRequestResult(result);
                }
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    return this.BadRequestResult(msg);
                }
                //return Ok();
                return Ok(j);//20220214
            }
            catch (Exception ex)
            {
                return this.BadRequestResult(ex.Message);
            }
        }
        /*20220125*/
        private string updateFileNumber(string EqpNum, SrmEqpH e)
        {
            string guid = null;
            //有guid才替換
            if (e.Guid != null)
            {
                guid = e.Guid.ToString();
                //有guid跟rfqnum才替換
                if (!string.IsNullOrEmpty(EqpNum) && !string.IsNullOrEmpty(guid))
                {
                    //有樣板才替換(user的werks跟functionid跟場內場外
                    UserClaims user = User.GetUserClaims();
                    if (_srmFileService.HasTemplate(user.Werks[0], 3, 1))
                    {
                        string result = _srmFileService.UpdateNumber(EqpNum, guid);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }
                    }

                }
            }
            return string.Empty;
        }
        /*20220125*/
        [HttpPost("Start")]
        public IActionResult Start(JObject jobj)
        {
            string msg = string.Empty;
            ViewSrmEqpH eqp = jobj["h"].ToObject<ViewSrmEqpH>();
            using (var transaction = new System.Transactions.TransactionScope())
            {
                SrmEqpH e = jobj["h"].ToObject<SrmEqpH>();
                try
                {
                    DateTime now = DateTime.Now;
                    UserClaims user = User.GetUserClaims();
                    e.LastUpdateBy = user.UserName;
                    e.LastUpdateDate = now;
                    e.Status = 7;
                   
                    ViewSrmEqpH ve = _srmEqpService.GetEqpH(e);
                   
                    if (string.IsNullOrWhiteSpace(e.WoNum))
                    {
                        e.WoNum = ve.WoNum;
                    }
                    if (string.IsNullOrWhiteSpace(e.no))
                    {
                        e.no = ve.no;
                    }
                    //msg = _srmEqpService.Save(e);
                    JObject j =  _srmEqpService.Save(e);
                    msg = j["msg"].ToString();
                    string txtSN = j["txtSN"].ToString();
                    string result = updateFileNumber(txtSN, e);
                    if (!string.IsNullOrEmpty(result))
                    {
                        return this.BadRequestResult(result);
                    }


                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        return this.BadRequestResult(msg);
                    }

                    eqp.EqpId = e.EqpId;
                    eqp.LastUpdateBy = e.LastUpdateBy;
                    eqp.LastUpdateDate = e.LastUpdateDate;
                    //WriteFileToBorg(eqp);
                    RunBorg(eqp);
                    //發信
                    //sendRespective(e);
                    //起單後變更狀態
                    //_srmEqpService.UpdateStatus((int)Status.啟動, e);
                    transaction.Complete();

                    //return Ok();
                    return Ok(j);
                }
                catch (Exception ex)
                {
                    //_srmEqpService.UpdateStatus((int)Status.初始, e);
                    ////刪除博格單據??
                    //transaction.Dispose();
                    return this.BadRequestResult(ex.Message);
                }
            }
        }

        [HttpPost("Delete")]
        [Permission("eqp")]
        public IActionResult Delete(JObject eqp)
        {
            try
            {
              
                SrmEqpH eqpH = eqp["h"].ToObject<SrmEqpH>();
                UserClaims user = User.GetUserClaims();
                eqpH.LastUpdateBy = user.UserName;
                eqpH.LastUpdateDate = DateTime.Now;
                _srmEqpService.UpdateStatus((int)Status.刪除, eqpH);

                //刪除博格??
                ViewSrmEqpH eqpv = eqp["h"].ToObject<ViewSrmEqpH>();
                if (eqpH.Status == 7)
                {
                    StopBorg(eqpv);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return this.BadRequestResult(ex.Message);
            }
        }
        private void StopBorg(ViewSrmEqpH eqp) 
        {
            BasicHttpBinding binding = new BasicHttpBinding();

            EndpointAddress address = new EndpointAddress("http://10.1.1.181/CF.BPM.Service/BPMAPI.asmx");

            BPMAPISoapClient client = new BPMAPISoapClient(binding, address);

            CallMethodParams callMethodParm = new CallMethodParams();//GetBorgEmpDataByLogonID
            callMethodParm.Method = "GetBorgEmpDataByLogonID";
            string sqe = "134060";//邱正傑
        
            callMethodParm.Options = sqe;

            Task<CallMethodResponse> response = client.CallMethodAsync(callMethodParm);
            CallMethodResult result = response.Result.Body.CallMethodResult;
            if (!result.Success)
            {
                throw new Exception(result.Message);
            }
            SrmEqpH eq = new SrmEqpH();

            JObject param = new JObject();
            param.Add("txtSN", eqp.EqpNum);
            Dictionary<string, object> Variables = new Dictionary<string, object>();
            //GetMatnr
            Variables.Add("SPM_JUMP", "結束"); 
            Variables.Add("OPINION", $"SRM刪除，系統自動結案");

            param.Add("Variables", JObject.FromObject(Variables));
            //param.Add("FormControls", JObject.FromObject(FormControls));
            callMethodParm.Method = "Send";
            callMethodParm.Options = JsonConvert.SerializeObject(param);

            response = client.CallMethodAsync(callMethodParm);
            result = response.Result.Body.CallMethodResult;
            if (!result.Success)
            {
                throw new Exception(result.Message);
            }
            //_srmPriceService.UpdateCaseid(infos);
        }
        #region WriteFileToBorg
        private void WriteFileToBorg(ViewSrmEqpH eqp) 
        {
            BasicHttpBinding binding = new BasicHttpBinding();

            EndpointAddress address = new EndpointAddress("http://10.1.1.181/CF.BPM.Service/BPMAPI.asmx");

            BPMAPISoapClient client = new BPMAPISoapClient(binding, address);
            JObject param = new JObject();
            CallMethodParams callMethodParm = new CallMethodParams();//GetBorgEmpDataByLogonID
            Task<CallMethodResponse> response = client.CallMethodAsync(callMethodParm);
            CallMethodResult result = response.Result.Body.CallMethodResult;
            SrmEqpH eq = new SrmEqpH();
            SrmEqpH e = new SrmEqpH();
            if (string.IsNullOrWhiteSpace(eqp.EqpNum))
            {
                eq.EqpId = eqp.EqpId;
                e = _srmEqpService.GetEqp(eq);
                if (!string.IsNullOrWhiteSpace(e.EqpNum))
                {
                    eqp.EqpNum = e.EqpNum;
                }
            }

            param.Add("txtSN", eqp.EqpNum); //開單人員是否寫SRM資料庫??
                                            //param.Add("signType", "工程品質問題反應單");
            int index = 1;
            string filepath = $"D:\\CF.SRM\\src\\Convience.Backend\\Convience.Applications\\Convience.ManagentApi\\fileStore\\3100\\{eqp.EqpNum}\\廠外\\工程品質問題反應單\\";      
            string file = $"http://10.88.1.25//EqpFile/{eqp.EqpNum}/廠外/工程品質問題反應單/";
            DirectoryInfo di = new DirectoryInfo(filepath);
            //Console.WriteLine("No search pattern returns:");
            if (di.Exists != false) 
            {
                foreach (var fi in di.GetFiles())
                {
                    Console.WriteLine(fi.Name);
                    param.Add($"File{index}", file + fi.Name);
                    index++;
                    //CopyFileToBorg(fi.Name, filepath,fileto);
                }
                param.Add("filecount", index);
                callMethodParm.Method = "UpdateFileList";
                callMethodParm.Options = JsonConvert.SerializeObject(param);

                response = client.CallMethodAsync(callMethodParm);
                result = response.Result.Body.CallMethodResult;
                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }
            }
           

            
        }
        #endregion
        private void RunBorg(ViewSrmEqpH eqp)
        {
            BasicHttpBinding binding = new BasicHttpBinding();

            EndpointAddress address = new EndpointAddress("http://10.1.1.181/CF.BPM.Service/BPMAPI.asmx");

            BPMAPISoapClient client = new BPMAPISoapClient(binding, address);

            CallMethodParams callMethodParm = new CallMethodParams();//GetBorgEmpDataByLogonID
            callMethodParm.Method = "GetBorgEmpDataByLogonID";
            //string sqe = "134060";//邱正傑
            string sqe = "134060";//邱正傑
            callMethodParm.Options = sqe;          

            Task<CallMethodResponse> response = client.CallMethodAsync(callMethodParm);
            CallMethodResult result = response.Result.Body.CallMethodResult;
            if (!result.Success)
            {
                throw new Exception(result.Message);
            }
            SrmEqpH eq = new SrmEqpH();
            eq.PoId = eqp.PoId;
            eq.MatnrId = eqp.MatnrId;

            ViewSrmEqpH ve = _srmEqpService.GetEqpH(eq);

            if (string.IsNullOrWhiteSpace(eqp.WoNum))
            {
                eqp.WoNum = ve.WoNum;
            }
            if (string.IsNullOrWhiteSpace(eqp.no))
            {
                eqp.no = ve.no;
            }
            JObject param = new JObject();
            param.Add("logonid", sqe); //開單人員是否寫SRM資料庫??
            param.Add("signType", "工程品質問題反應單");
            Dictionary<string, object> Variables = new Dictionary<string, object>();
            //GetMatnr
            Variables.Add("SUBJECT", $"{eqp.WoNum} ， {eqp.Description}"); //帶入採單 + partname???
            //Variables.Add("SPM_JUMP", $"開單主管(128320)"); //???測試是否
            //objWF.set_Variables("SPM_JUMP", result.SPM_JUMP);


            Dictionary<string, object> FormControls = new Dictionary<string, object>();
            SrmEqpH e = new SrmEqpH();
            if (string.IsNullOrWhiteSpace(eqp.EqpNum)) 
            {
                eq.EqpId = eqp.EqpId;
                e = _srmEqpService.GetEqp(eq);
                if (!string.IsNullOrWhiteSpace(e.EqpNum))
                {
                    eqp.EqpNum = e.EqpNum;
                }
            }
            FormControls.Add("txtSN", eqp.EqpNum);
            FormControls.Add("txtPart", eqp.matnr);//料號
            FormControls.Add("txtNo", eqp.no);//序號
            FormControls.Add("eqp_id",eqp.EqpId);//eqp_id
            FormControls.Add("txtDeptFromSRM", eqp.CreateByName); //供應商名字

            FormControls.Add("txtPartName", eqp.Description);//品名
            FormControls.Add("txtWoNo", eqp.WoNum);//採購單/工單號
            FormControls.Add("DeliveryDate", eqp.DeliveryDate);//原定交期


            FormControls.Add("txtQrderQty", eqp.PoQty);//訂單數量
            FormControls.Add("txtAbnormalQty", eqp.NgQty);//異常數量
            FormControls.Add("txtRev", eqp.Version);//版次
            FormControls.Add("txtNonconformanceDescription",eqp.NgDesc);//異常狀況及過程說明
            FormControls.Add("txtPreliminaryanalyses", eqp.CauseAnalyses);//初步肇因分析
            FormControls.Add("uploadFile001", eqp.filename);//附件 ???寫attachfile 搬檔案

            DataTable resultdt = JsonConvert.DeserializeObject<DataTable>(result.Options.ToString());
            FormControls.Add("ddlUserName_USERNAME", resultdt.Rows[0]["USERNAME"].ToString());
            FormControls.Add("ddlDeptName", resultdt.Rows[0]["DEPTNAME"].ToString());
            //FormControls.Add("txtEXT", resultdt.Rows[0]["EXT"].ToString());
            //FormControls.Add("txtEmail", resultdt.Rows[0]["EMAIL"].ToString());
            FormControls.Add("ddlDept", resultdt.Rows[0]["DEPTID"].ToString());
            FormControls.Add("txtUserLOGONID", resultdt.Rows[0]["LOGONID"].ToString());
            FormControls.Add("IfFromSRM", "Y");
            QueryEqp qe = new QueryEqp();
            qe.vendor = eqp.LastUpdateBy;
            string vendorname = _srmEqpService.GetVendorName(qe);
            FormControls.Add("txtVendor", eqp.LastUpdateBy+ ":"+ vendorname); 
            #region 供應商起單固定送供應商品保部
            FormControls.Add("dept1", "3112"); //dept1
            FormControls.Add("dept2", "311200"); //dept2
            FormControls.Add("ddlRDept", "311200"); //ddlRDept 
            FormControls.Add("costno", "311200"); //20220207
            
            #endregion
            //FormControls.Add("txtUserTitle", resultdt.Rows[0]["TITLE"].ToString());
            //FormControls.Add("txtUserWorkplace", resultdt.Rows[0]["workplaceName"].ToString());
            //FormControls.Add("txtUserArriveDate", resultdt.Rows[0]["ArriveTime"].ToString());

            param.Add("Variables", JObject.FromObject(Variables));
            param.Add("FormControls", JObject.FromObject(FormControls));
            //param.Add("ds", JObject.FromObject(GetCfInfoRecord(rfqH, infos)));
            callMethodParm.Method = "Sign";
            callMethodParm.Options = JsonConvert.SerializeObject(param);

            response = client.CallMethodAsync(callMethodParm);
            result = response.Result.Body.CallMethodResult;
            if (!result.Success)
            {
                throw new Exception(result.Message);
            }

            WriteFileToBorg(eqp);
            //_srmPriceService.UpdateCaseid(infos);
            /*複製檔案到博格*/
            //string caseid = GetCaseId(eqp.EqpNum); //取得博格路徑           
            //int caseid = GetCaseId("PS22022"); //取得博格路徑           
            //GetFiles(eqp.EqpNum, caseid);
            /**/
        }
        private int GetCaseId(QueryEqp e)
        {
            int caseid = 0;// _srmEqpService.GetCaseId(e); 
            return caseid;
        }
        private void CopyFileToBorg(string fileName, string sourcePath,string targetPath) 
        {
            //string fileName = "test.txt";
            //string sourcePath = @"C:\Users\Public\TestFolder";
            //string targetPath = @"C:\Users\Public\TestFolder\SubDir";
            string file = fileName;
            string source = sourcePath;// @"C:\Users\Public\TestFolder";
            //string targetPath = @"C:\Users\Administrator\Desktop\20220117_TESt";

            // Use Path class to manipulate file and directory paths.
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            // To copy a folder's contents to a new location:
            // Create a new target folder.
            // If the directory already exists, this method does not create a new directory.
            System.IO.Directory.CreateDirectory(targetPath);

            // To copy a file to another location and
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(sourceFile, destFile, true);

            // To copy all the files in one directory to another directory.
            // Get the files in the source folder. (To recursively iterate through
            // all subfolders under the current directory, see
            // "How to: Iterate Through a Directory Tree.")
            // Note: Check for target path was performed previously
            //       in this code example.
            if (System.IO.Directory.Exists(sourcePath))
            {
                string[] files = System.IO.Directory.GetFiles(sourcePath);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    fileName = System.IO.Path.GetFileName(s);
                    destFile = System.IO.Path.Combine(targetPath, fileName);
                    System.IO.File.Copy(s, destFile, true);
                }
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
            }

            // Keep console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
        private void GetFiles(string txtSN,int caseid) 
        {
            //D:\CF.SRM\src\Convience.Backend\Convience.Applications\Convience.ManagentApi\fileStore\3100\PS22022\廠內\工程品質問題反應單
            string filepath = $"D:\\CF.SRM\\src\\Convience.Backend\\Convience.Applications\\Convience.ManagentApi\\fileStore\\3100\\{txtSN}\\廠內\\工程品質問題反應單";
            //string fileto = $"Z:\\20220118\\{caseid}\\";
            DirectoryInfo di = new DirectoryInfo(filepath);
            Console.WriteLine("No search pattern returns:");
            foreach (var fi in di.GetFiles())
            {
                Console.WriteLine(fi.Name);
                //CopyFileToBorg(fi.Name, filepath,fileto);
            }

            Console.WriteLine();

            //Console.WriteLine("Search pattern *2* returns:");
            //foreach (var fi in di.GetFiles("*2*"))
            //{
            //    Console.WriteLine(fi.Name);
            //}

            //Console.WriteLine();

            //Console.WriteLine("Search pattern test?.txt returns:");
            //foreach (var fi in di.GetFiles("test?.txt"))
            //{
            //    Console.WriteLine(fi.Name);
            //}

            //Console.WriteLine();

            //Console.WriteLine("Search pattern AllDirectories returns:");
            //foreach (var fi in di.GetFiles("*", SearchOption.AllDirectories))
            //{
            //    Console.WriteLine(fi.Name);
            //}
        }

        [HttpGet("GetEqpData")]
        [Permission("eqp")]
        public IActionResult GetEqpData(int id)
        {
            ViewSrmEqpH h = _srmEqpService.GetDataByEqpId(id);
            return Ok(h);
        }
        #region 收件者為生管、採購
        private void sendRespective(SrmEqpH e)
        {
            string vname = string.Empty;
            string etxtSN = string.Empty;
            StringBuilder sb = new StringBuilder();
            StringBuilder qotNums = new StringBuilder();
            try
            {
                MailMessage mail = new MailMessage();
                //mail.From = new MailAddress("mis@chenfull.com.tw");
                //mail.To.Add(vendor.Mail);
                string reciever = string.Empty;
                reciever = _srmEqpService.GetEkgryMail(e);//採購
                string matnr = _srmEqpService.GetMatnr(e); 
                //string mrphandler = GetMRPHandler(matnr);//RFC取得生管???
                //reciever = reciever + "," + mrphandler;
                reciever = "jenny.chen@chenfull.com.tw";
                mail.To.Add(reciever);
                //mail.CC.Add("leo.lai@chenfull.com.tw");
                //var sourcer = _userService.GetUsers(new UserQueryModel { UserName = rfq.CreateBy, Page = 1, Size = 1 });
                //if (!string.IsNullOrWhiteSpace(sourcer.Data[0].Email)) { mail.CC.Add(sourcer.Data[0].Email); }
                //mail.To.Add("leo.lai@chenfull.com.tw");
                //SrmEqpH[] eqps = _srmEqpService.Get(new QueryEqp() { eqpid = e.EqpId });
                ViewSrmEqpH eqpv = _srmEqpService.GetDataByEqpId(e.EqpId);
                vname = eqpv.CreateByName;
                etxtSN = eqpv.EqpNum;
                //SrmQotH[] qots = _srmQotHService.Get(new QueryQot() { rfqId = rfq.RfqId, vendorId = vendor.VendorId });
                sb.Clear();
                //sb.AppendLine("收件者:<br />");
                //sb.AppendLine(vendor.Mail+ "<br />");
               
                string body = "";
                string subject = "";
                //string bodyVendor = $"{vendor.SrmVendor1} {vendor.VendorName}";

                body = $@"各位主管及同仁 好,<br />

此信為系統發送,<br />
工程品質問題反應單{eqpv.EqpNum}已發送，請協助於簽核系統查詢此案件資訊，謝謝！<br />
 如有任何問題，<br />
 請回覆此E-MAIL致供應商{eqpv.CreateByName}或致電協調。<br />
<a href='http://10.1.1.181/bpm'>簽核系統入口</a><br />
{ sb.ToString()}";
                subject = $"{eqpv.CreateByName}-{eqpv.EqpNum} 工程品質問題反應單啟動通知";

                mail.Body = body;
                mail.Subject = subject;
                mail.IsBodyHtml = true;
                sendMail(mail);
            }
            catch (Exception ex)
            {
                MailMessage mail = new MailMessage();
                mail.To.Add("jenny.chen@chenfull.com.tw");
                mail.Body = $"寄信異常，供應商:{vname}-{etxtSN}，EX:{ex.Message}";
                mail.Subject = "寄信異常";
                sendMailMIS(mail);
            }
        }
        #endregion
        private void sendMailMIS(MailMessage mail)
        {
            #region test
            if (_appSettingsService.Environment == "dev")
            {
                mail.To.Clear();
                mail.CC.Clear();
                mail.Bcc.Clear();
                mail.To.Add("jenny.chen@chenfull.com.tw");
            }
            #endregion test

            using (System.Net.Mail.SmtpClient MySMTP = new System.Net.Mail.SmtpClient("mail.chenfull.com.tw", 25))
            {
                mail.From = new MailAddress("mis@chenfull.com.tw");
                MySMTP.Credentials = new System.Net.NetworkCredential("mis@chenfull.com.tw", "Chen@full");
                MySMTP.Send(mail);
                MySMTP.Dispose();
            }
        }
        private void sendMail(MailMessage mail)
        {
            #region test
            if (_appSettingsService.Environment == "dev")
            {
                mail.To.Clear();
                mail.CC.Clear();
                mail.Bcc.Clear();
                mail.To.Add("jenny.chen@chenfull.com.tw");
            }
            #endregion test

            using (System.Net.Mail.SmtpClient MySMTP = new System.Net.Mail.SmtpClient("cfp-ex01.cfprec.com.tw", 25))
            {
                mail.From = new MailAddress("purchasing@cfprec.com.tw", "千附-採購部");
                MySMTP.Credentials = new System.Net.NetworkCredential("purchasing@cfprec.com.tw", "Ab1234567");
                MySMTP.Send(mail);
                MySMTP.Dispose();
            }
        }
        //[HttpPost]
        //[Route("GetMRPHandler")]
        //public string GetMRPHandler(string matnr)
        //{
        //    string handlermail = string.Empty;      
        //    try
        //    {
        //        Dictionary<string, object> dicImport = new Dictionary<string, object>();

        //        dicImport.Add("I_MATNR", matnr.ToUpper());//物料???
        //        dicImport.Add("I_MAKTX", ""); //物料說明
        //        dicImport.Add("I_PAGE_NO", "1");
        //        dicImport.Add("I_PAGE_CNT", "20");
        //        dicImport.Add("I_NO_DEL", "");
        //        dicImport.Add("I_WERKS", "3100");

        //        Dictionary<string, object> hmImportTables = new Dictionary<string, object>();
        //        List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
        //        Dictionary<string, string> row = new Dictionary<string, string>();
        //        row.Add("MTART", "*");
        //        row.Add("GROES", "");
        //        row.Add("MEINS", "");
        //        row.Add("NORMT", "");
        //        row.Add("EAN11", "");
        //        row.Add("FERTH", "");

        //        Dictionary<string, string> dicExport = new Dictionary<string, string>();
        //        dicExport.Add("E_TYPE", null);
        //        dicExport.Add("E_MESSAGE", null);
        //        dicExport.Add("E_TOTAL_CNT", null);
        //        string[] table = new string[] { "T_MARC", "T_MARA", "T_T024D" };
        //        DataSet ds = SapHelper.RFC("s4_cfp_qas", "Z_RFC_GET_MATERIAL_DATA",
        //        dicImport,
        //        hmImportTables,
        //        dicExport,
        //        table);

        //        if (dicExport["E_TYPE"].Equals("S"))
        //        {
        //            if (ds.Tables[2].Rows.Count > 0)
        //            {
        //                handlermail = ds.Tables[2].Rows[0]["USRKEY"].ToString();
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception(dicExport["E_MESSAGE"].ToString());
        //        }
        //        return handlermail;
        //    }
        //    catch (Exception e)
        //    {
        //        //return Request.CreateResponse(HttpStatusCode.BadRequest, "sap報錯");
        //        MailMessage mail = new MailMessage();
        //        mail.To.Add("jenny.chen@chenfull.com.tw");
        //        mail.Body = $"料號:{matnr} 取得MRP控制員異常，EX:{e.Message}";
        //        mail.Subject = "反應單異常";
        //        sendMailMIS(mail);
        //        return e.ToString();
        //    }
        //}
    }
}
