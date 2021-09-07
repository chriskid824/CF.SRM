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
        private readonly ISrmVendorService _srmVendorService;
        private readonly ISrmMatnrService _srmMatnrService;
        private readonly ISrmInfoRecordService _srmInfoRecordService;

        public SrmPriceController(ISrmPriceService srmPriceService
            , ISrmQotService srmQotService
            , ISrmRfqHService srmRfqHService
            , ISrmRfqMService srmRfqMService
            , ISrmVendorService srmVendorService
            , ISrmMatnrService srmMatnrService
            ,ISrmInfoRecordService srmInfoRecordService
            )
        {
            _srmQotService = srmQotService;
            _srmPriceService = srmPriceService;
            _srmRfqMService = srmRfqMService;
            _srmRfqHService = srmRfqHService;
            _srmVendorService = srmVendorService;
            _srmMatnrService = srmMatnrService;
            _srmInfoRecordService = srmInfoRecordService;
        }
        [HttpPost("GetQotDetail")]
        [Permission("price")]
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

        [HttpPost("GetSummary")]
        [Permission("price")]
        public IActionResult GetSummary(QueryQot query) {
            var qots = _srmQotService.Get(query);
            var infos = _srmInfoRecordService.Get(new QueryInfoRecordModels() { qotIds = qots.Select(r => r.QotId).ToArray() });
            ViewSrmPriceDetail detail = _srmPriceService.GetDetail(qots);
            ViewSrmRfqH rfqH = _srmRfqHService.GetDataByRfqId(query.rfqId.Value);
            List<ViewSummary> summ = new List<ViewSummary>();
            foreach (var qot in qots) {
                int max = 1;
                var material = detail.material.AsEnumerable().Where(r => r.QotId == qot.QotId);
                var process = detail.process.AsEnumerable().Where(r => r.QotId == qot.QotId);
                var surface = detail.surface.AsEnumerable().Where(r => r.QotId == qot.QotId);
                var other = detail.other.AsEnumerable().Where(r => r.QotId == qot.QotId);
                var infoRecord = detail.infoRecord.AsEnumerable().Where(r => r.QotId == qot.QotId);

                max = Math.Max(max, material.Count());
                max = Math.Max(max, process.Count());
                max = Math.Max(max, surface.Count());
                max = Math.Max(max, other.Count());
                ViewSummary[] temp = new ViewSummary[max];
                for(int i = 0; i < temp.Count(); i++) { 
                    temp[i] = new ViewSummary();
                }
                SrmVendor vendor = _srmVendorService.GetVendorById(qot.VendorId.Value);
                ViewSrmRfqM matnr = _srmRfqMService.GetRfqMData(new SrmRfqM { RfqId = qot.RfqId, MatnrId = qot.MatnrId });
                temp[0].RfqId = rfqH.RfqId;
                temp[0].RfqNum = rfqH.RfqNum;
                temp[0].isStarted = infos.Any(r => r.QotId == qot.QotId) || qot.Status.GetValueOrDefault()!=(int)Status.確認;
                temp[0].qotStatus = ((Status)qot.Status.GetValueOrDefault());
                temp[0].Status = rfqH.Status;
                temp[0].sourcerName = rfqH.sourcerName;
                temp[0].Deadline = rfqH.Deadline;
                temp[0].vendor = vendor.SrmVendor1;
                temp[0].vendorId = vendor.VendorId;
                temp[0].vendorName = vendor.VendorName;
                temp[0].matnr = matnr.srmMatnr;
                temp[0].matnrId = matnr.MatnrId.Value;
                temp[0].material = matnr.Material;
                temp[0].volume = $"{matnr.Length}*{matnr.Width}*{matnr.Height}";
                temp[0].weight = matnr.Weight.HasValue? matnr.Weight.Value.ToString():"";
                temp[0].machineName = matnr.MachineName;
                temp[0].qotNum = qot.QotNum;
                temp[0].qotId = qot.QotId.ToString();
                foreach (var item in material.Select((value, i) => new { i, value })) {
                    temp[item.i].mMaterial = item.value.MMaterial;
                    temp[item.i].mPrice = item.value.MPrice?.NormalizeTwoDigits()??"";
                    temp[item.i].mLength = item.value.Length?.ToString()??"";
                    temp[item.i].mWidth = item.value.Width?.ToString()??"";
                    temp[item.i].mHeight = item.value.Height?.ToString() ?? "";
                    temp[item.i].mDensity = item.value.Density?.ToString() ?? "";
                    temp[item.i].mWeight = item.value.Weight?.ToString()??"";
                    temp[item.i].mCostPrice = item.value.MCostPrice?.NormalizeTwoDigits() ?? "";
                    temp[item.i].mNote = item.value.Note?.ToString() ?? "";
                }

                foreach (var item in process.Select((value, i) => new { i, value }))
                {
                    temp[item.i].pMachine = item.value.PMachine;
                    temp[item.i].pProcessNum = item.value.PProcessNum?.ToString() ?? "";
                    temp[item.i].pHours = item.value.PHours?.ToString() ?? "";
                    temp[item.i].pPrice = item.value.PPrice?.NormalizeTwoDigits() ?? "";
                    temp[item.i].pSubTotal = item.value.SubTotal.NormalizeTwoDigits();
                    temp[item.i].pNote = item.value.PNote;
                }

                foreach (var item in surface.Select((value, i) => new { i, value }))
                {
                    temp[item.i].sProcess = item.value.SProcess;
                    temp[item.i].sTimes = item.value.STimes?.ToString() ?? "";
                    temp[item.i].sPrice = item.value.SPrice?.NormalizeTwoDigits() ?? "";
                    temp[item.i].sSubTotal = item.value.SubTotal.NormalizeTwoDigits();
                    //todo 哀
                    //temp[item.i].sMethod = item.value.method.ToString();
                    temp[item.i].sNote = item.value.SNote;
                }

                foreach (var item in other.Select((value, i) => new { i, value }))
                {
                    temp[item.i].oItem = item.value.OItem;
                    temp[item.i].oDescription = item.value.ODescription;
                    temp[item.i].oPrice = item.value.OPrice?.NormalizeTwoDigits();
                    temp[item.i].oNote = item.value.ONote;
                }

                foreach (var item in infoRecord.Select((value, i) => new { i, value }))
                {
                    temp[item.i].aTotal = item.value.Atotal.NormalizeTwoDigits();
                    temp[item.i].bTotal = item.value.Btotal.NormalizeTwoDigits();
                    temp[item.i].cTotal = item.value.Ctotal.NormalizeTwoDigits();
                    temp[item.i].dTotal = item.value.Dtotal.NormalizeTwoDigits();
                    temp[item.i].total = (item.value.Atotal + item.value.Btotal + item.value.Ctotal + item.value.Dtotal).NormalizeTwoDigits();
                    if (temp[item.i].qotStatus == Status.確認)
                    {
                        temp[item.i].price = (item.value.Price.HasValue) ? item.value.Price.Value.NormalizeTwoDigits() : (item.value.Atotal + item.value.Btotal + item.value.Ctotal + item.value.Dtotal).NormalizeTwoDigits();
                        temp[item.i].unit = (item.value.Unit.HasValue) ? item.value.Unit.Value.ToString() : "1";
                        temp[item.i].currency = item.value.Currency?.ToString() ?? "TWD";
                        temp[item.i].currencyName = item.value.currencyName?.ToString() ?? "新台幣元";
                        temp[item.i].leadTime = (item.value.LeadTime.HasValue) ? item.value.LeadTime.Value.ToString() : qot.LeadTime.GetValueOrDefault().ToString();
                        temp[item.i].standQty = (item.value.StandQty.HasValue) ? item.value.StandQty.Value.ToString() : "1";
                        temp[item.i].minQty = (item.value.MinQty.HasValue) ? item.value.MinQty.Value.ToString() : "1";
                        temp[item.i].ekgry = item.value.Ekgry ?? rfqH.ekgry;
                        temp[item.i].taxcode = item.value.Taxcode?.ToString() ?? "V4";
                        temp[item.i].taxcodeName = string.IsNullOrWhiteSpace(item.value.Taxcode) ? "V4 進項稅5%" : $"{item.value.Taxcode} {item.value.taxcodeName}";
                        temp[item.i].effectiveDate = (item.value.EffectiveDate.HasValue) ? item.value.EffectiveDate.Value.ToString("yyyy/MM/dd") : DateTime.Now.ToString("yyyy/MM/dd");
                        temp[item.i].expirationDate = (item.value.ExpirationDate.HasValue) ? item.value.ExpirationDate.Value.ToString("yyyy/MM/dd") : new DateTime(DateTime.Now.Year + 1, 1, 1).AddDays(-1).ToString("yyyy/MM/dd");
                        temp[item.i].note = item.value.Note;
                    }
                }
                summ.AddRange(temp.ToList());
            }



            //SrmRfqM m = new SrmRfqM()
            //{
            //    RfqId = query.rfqId,
            //    MatnrId = query.matnrId
            //};
            //detail.matnr = _srmRfqMService.GetRfqMData(m);
            return Ok(summ);
        }

        [HttpPost("Start")]
        [Permission("price")]
        public IActionResult Start(JObject jobj) {
            int rfqId = (int)jobj["rfqId"];
            var rfqH = _srmRfqHService.GetDataByRfqId(rfqId);
            if (rfqH.Status.Value != (int)Status.確認 && rfqH.Status.Value != (int)Status.簽核中 && rfqH.Status.Value != (int)Status.已核發) {
                return this.BadRequestResult("詢價單狀態異常");
            }
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
                var matnr = _srmMatnrService.GetMatnrById(info.MatnrId.Value);
                if (string.IsNullOrWhiteSpace(matnr.SapMatnr)) { 
                    return this.BadRequestResult($"報價單號:{info.qotNum}，料號:{matnr.SrmMatnr1}，SapMatnr未存在");
                }
            }
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    rfqId = _srmPriceService.Start(infos).Value;
                    _srmRfqHService.UpdateStatus((int)Status.簽核中, new SrmRfqH { RfqId = rfqId, LastUpdateDate = now, LastUpdateBy = logonid });
                    RunBorg(rfqH,infos);
                    transaction.Complete();
                    return Ok();
                }
                catch (Exception ex) {
                    transaction.Dispose();
                    return this.BadRequestResult(ex.Message);
                }
            }
        }



        private void RunBorg(ViewSrmRfqH rfqH, viewSrmInfoRecord[] infos)
        {
            BasicHttpBinding binding = new BasicHttpBinding();

            EndpointAddress address = new EndpointAddress("http://10.1.1.181/CF.BPM.Service/BPMAPI.asmx");

            BPMAPISoapClient client = new BPMAPISoapClient(binding, address);

            CallMethodParams callMethodParm = new CallMethodParams();//GetBorgEmpDataByLogonID
            callMethodParm.Method = "GetBorgEmpDataByLogonID";
            callMethodParm.Options = rfqH.Sourcer;

            Task<CallMethodResponse> response = client.CallMethodAsync(callMethodParm);
            CallMethodResult result = response.Result.Body.CallMethodResult;
            if (!result.Success)
            {
                throw new Exception(result.Message);
            }



            JObject param = new JObject();
            param.Add("logonid", "137680");
            param.Add("signType", "採購資訊紀錄簽核單");
            Dictionary<string, object> Variables = new Dictionary<string, object>();
            Variables.Add("SUBJECT", "採購資訊紀錄簽核單TEST");
            Dictionary<string, object> FormControls = new Dictionary<string, object>();
            FormControls.Add("werks", rfqH.Werks.Value);
            FormControls.Add("rfqId", rfqH.RfqId);
            DataTable resultdt = JsonConvert.DeserializeObject<DataTable>(result.Options.ToString());
            FormControls.Add("ddlUserName_USERNAME", resultdt.Rows[0]["USERNAME"].ToString());
            FormControls.Add("ddlDeptName", resultdt.Rows[0]["DEPTNAME"].ToString());
            FormControls.Add("txtEXT", resultdt.Rows[0]["EXT"].ToString());
            FormControls.Add("txtEmail", resultdt.Rows[0]["EMAIL"].ToString());
            FormControls.Add("ddlDept", resultdt.Rows[0]["DEPTID"].ToString());
            FormControls.Add("txtUserLOGONID", resultdt.Rows[0]["LOGONID"].ToString());
            FormControls.Add("txtUserTitle", resultdt.Rows[0]["TITLE"].ToString());
            FormControls.Add("txtUserWorkplace", resultdt.Rows[0]["workplaceName"].ToString());
            FormControls.Add("txtUserArriveDate", resultdt.Rows[0]["ArriveTime"].ToString());

            DataSet ds = new DataSet();
            DataTable InfoRecord = new DataTable("CF_InfoRecord");
            InfoRecord.Columns.Add("SapMatnr");
            InfoRecord.Columns.Add("DESCRIPTION");
            InfoRecord.Columns.Add("QTY");
            InfoRecord.Columns.Add("Height");
            InfoRecord.Columns.Add("Length");
            InfoRecord.Columns.Add("Width");
            InfoRecord.Columns.Add("Weight");
            InfoRecord.Columns.Add("VendorName");
            InfoRecord.Columns.Add("ATotal");
            InfoRecord.Columns.Add("BTotal");
            InfoRecord.Columns.Add("CTotal");
            InfoRecord.Columns.Add("DTotal");
            InfoRecord.Columns.Add("Total");
            InfoRecord.Columns.Add("Price");
            InfoRecord.Columns.Add("HistoricalPrice");
            InfoRecord.Columns.Add("HistoricalDate");
            InfoRecord.Columns.Add("LastPrice");
            InfoRecord.Columns.Add("LastDate");
            InfoRecord.Columns.Add("BargainingRate");
            InfoRecord.Columns.Add("LastBargainingRate");
            InfoRecord.Columns.Add("ExpirationDate");
            InfoRecord.Columns.Add("InfoId");
            InfoRecord.Columns.Add("Img1");
            InfoRecord.Columns.Add("Img2");
            InfoRecord.Columns.Add("Note");
            foreach (var info in infos) {
                var rfqM = _srmRfqMService.GetRfqMData(new SrmRfqM() { RfqId = rfqH.RfqId, MatnrId = info.MatnrId });
                DataRow dr = InfoRecord.NewRow();
                dr["SapMatnr"] = rfqM.matnr;
                dr["DESCRIPTION"] = rfqM.description;
                dr["QTY"] = rfqM.Qty;
                dr["Height"] = rfqM.Height;
                dr["Length"] = rfqM.Length;
                dr["Width"] = rfqM.Width;
                dr["Weight"] = rfqM.Weight;
                dr["VendorName"] = info.VendorName;
                dr["ATotal"] = info.Atotal;
                dr["BTotal"] = info.Btotal;
                dr["CTotal"] = info.Ctotal;
                dr["DTotal"] = info.Dtotal;
                dr["Total"] = info.total;
                dr["Price"] = info.Price;
                dr["InfoId"] = info.InfoId;
                dr["Img1"] = "/BPM/images/logo.jpg";
                dr["Img2"] = "/BPM/images/logo2.png";
                dr["Note"] = info.Note;
                InfoRecord.Rows.Add(dr);
            }

            ds.Tables.Add(InfoRecord);
            param.Add("Variables",JObject.FromObject(Variables));
            param.Add("FormControls",JObject.FromObject(FormControls));
            param.Add("ds", JObject.FromObject(ds));
            callMethodParm.Method = "Sign";
            callMethodParm.Options = JsonConvert.SerializeObject(param);

            response = client.CallMethodAsync(callMethodParm);
            result = response.Result.Body.CallMethodResult;
            if (!result.Success) {
                throw new Exception(result.Message);
            }

            foreach (var info in infos) {
                info.Caseid = int.Parse(result.Options.ToString());
            }
            _srmPriceService.UpdateCaseid(infos);

            //var bpm = new BPMAPISoapClient(new BPMAPISoapClient.EndpointConfiguration());
            //JObject param = new JObject();
            //param.Add("logonid", Request.QueryString["logon"]);
            //param.Add("signType", "文件新增變更申請單");
            //Dictionary<string, object> Variables = new Dictionary<string, object>();
            //Variables.Add("SUBJECT", $"文件編號:{obj.doc_no} 文件名稱:{obj.doc_name} 申請類別:{(isNew ? "新增" : "改版")}");
            //Dictionary<string, object> FormControls = new Dictionary<string, object>();
            //setUser(ViewState["logonid"].ToString(), FormControls);
            //FormControls.Add("txtdoc_id", ViewState["doc_id"].ToString());
            //FormControls.Add("txtfolder_id", ViewState["folder_id"].ToString());
            //FormControls.Add("rblAppType", isNew ? "新增" : "改版");
            //FormControls.Add("txtdoc_no", obj.doc_no);
            //FormControls.Add("txtdoc_name", obj.doc_name);
            //FormControls.Add("txtversion_major", obj.version_major);
            //FormControls.Add("txtversion_minor", obj.version_minor);
            //FormControls.Add("txtVersion", obj.version);
            //FormControls.Add("txtwriter", obj.writer);
            //FormControls.Add("txtdms_type", dms_type);
            //FormControls.Add("txtAppDescription", obj.change_desc);
            //FormControls.Add("txtworkflow_id", workflow_id);
            //FormControls.Add("txtFolderPath", Utility.GetVirtualPath(ViewState["folder_id"].ToString()));
            //Variables.Add("APPLYDEPT", FormControls["ddlDept"].ToString());
            //param.Add("Variables", JObject.FromObject(Variables));
            //param.Add("FormControls", JObject.FromObject(FormControls));
            //IZ.WebFileManager.BPMAPI.CallMethodParams cmp = new IZ.WebFileManager.BPMAPI.CallMethodParams()
            //{
            //    Token = "",
            //    Method = "Sign",
            //    Options = JsonConvert.SerializeObject(param)
            //};
            //IZ.WebFileManager.BPMAPI.CallMethodResult result = bpm.CallMethod(cmp);
            //if (!result.Success)
            //{
            //    throw new Exception(result.Message);
            //}
            //return;
        }


        [HttpPost("GetTaxcodes")]
        [Permission("price")]
        public IActionResult GetTaxcodes() {
            return Ok(_srmPriceService.GetTaxcodes());
        }
        [HttpPost("GetCurrency")]
        [Permission("price")]
        public IActionResult GetCurrency()
        {
            return Ok(_srmPriceService.GetCurrency());
        }
        [HttpPost("GetEkgry")]
        [Permission("price")]
        public IActionResult GetEkgry(int[] werks)
        {
            return Ok(_srmPriceService.GetEkgry(werks));
        }

        [HttpPost("QueryInfoRecord")]
        [Permission("price")]
        public IActionResult QueryInfoRecord(QueryInfoRecordModels query)
        {
            return Ok(_srmInfoRecordService.Query(query));
        }
    }
}
