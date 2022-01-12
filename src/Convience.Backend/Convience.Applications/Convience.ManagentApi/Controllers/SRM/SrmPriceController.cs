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
        private readonly ISrmHistoryPriceService _srmHistoryPriceService;

        public SrmPriceController(ISrmPriceService srmPriceService
            , ISrmQotService srmQotService
            , ISrmRfqHService srmRfqHService
            , ISrmRfqMService srmRfqMService
            , ISrmVendorService srmVendorService
            , ISrmMatnrService srmMatnrService
            , ISrmInfoRecordService srmInfoRecordService
            , ISrmHistoryPriceService srmHistoryPriceService
            )
        {
            _srmQotService = srmQotService;
            _srmPriceService = srmPriceService;
            _srmRfqMService = srmRfqMService;
            _srmRfqHService = srmRfqHService;
            _srmVendorService = srmVendorService;
            _srmMatnrService = srmMatnrService;
            _srmInfoRecordService = srmInfoRecordService;
            _srmHistoryPriceService = srmHistoryPriceService;
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

            if (query.caseId.HasValue && !infos.Any(r=>r.Caseid==query.caseId)) {
                return Ok(new ViewSummary[0]);
            }

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
                ViewSrmRfqM rfqM = _srmRfqMService.GetRfqMData(new SrmRfqM { RfqId = qot.RfqId, MatnrId = qot.MatnrId });
                SrmMatnr matnr =  _srmMatnrService.GetMatnrById(qot.MatnrId.Value);
                temp[0].RfqId = rfqH.RfqId;
                temp[0].RfqNum = rfqH.RfqNum;
                temp[0].InfoId = infos.Where(r => r.QotId == qot.QotId).FirstOrDefault()?.InfoId ?? 0;
                temp[0].isStarted = infos.Any(r => r.QotId == qot.QotId) || qot.Status.GetValueOrDefault()!=(int)Status.確認;
                temp[0].canEdit = query.caseId.HasValue ? infos.Any(r => r.Status == (int)Status.初始 && r.Caseid == query.caseId.Value) &&(!infos.Any(r => r.QotId == qot.QotId) || infos.Any(r=>r.Caseid==query.caseId.Value&&r.QotId==qot.QotId&&r.Status==(int)Status.初始)) : false;
                temp[0].isStarted = query.caseId.HasValue ? !temp[0].canEdit || temp[0].isStarted: temp[0].isStarted;
                temp[0].caseId = infoRecord.Any() ? infoRecord.First().Caseid : null;
                temp[0].qotStatus = ((Status)qot.Status.GetValueOrDefault());
                temp[0].Status = rfqH.Status;
                temp[0].sourcerName = rfqH.sourcerName;
                temp[0].Deadline = rfqH.Deadline;
                temp[0].vendor = vendor.SrmVendor1;
                temp[0].vendorId = vendor.VendorId;
                temp[0].vendorName = vendor.VendorName;
                temp[0].vendorObject = vendor;
                temp[0].matnrObject = matnr;
                temp[0].matnr = rfqM.srmMatnr;
                temp[0].matnrId = rfqM.MatnrId.Value;
                temp[0].material = rfqM.Material;
                temp[0].description = rfqM.Description;
                temp[0].major_diameter = rfqM.Major_diameter;
                temp[0].minor_diameter = rfqM.Minor_diameter;
                temp[0].bn_num = rfqM.Bn_num;
                temp[0].volume = $"{rfqM.Length}*{rfqM.Width}*{rfqM.Height}";
                temp[0].weight = rfqM.Weight.HasValue? rfqM.Weight.Value.ToString():"";
                temp[0].gewei = rfqM.Gewei;
                temp[0].machineName = rfqM.MachineName;
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
                    temp[item.i].pProcessName = item.value.ProcessName;
                    temp[item.i].pHours = item.value.PHours?.ToString() ?? "";
                    temp[item.i].pPrice = item.value.PPrice?.NormalizeTwoDigits() ?? "";
                    temp[item.i].pSubTotal = item.value.SubTotal.NormalizeTwoDigits();
                    temp[item.i].pNote = item.value.PNote;
                }

                foreach (var item in surface.Select((value, i) => new { i, value }))
                {
                    temp[item.i].sProcess = item.value.SProcess;
                    temp[item.i].sProcessName = item.value.ProcessName;
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
                        temp[item.i].org = item.value.Org.HasValue? item.value.Org.Value: rfqH.Werks.Value;
                        temp[item.i].infoKind = (item.value.InfoKind.HasValue ? item.value.InfoKind.Value : (int)INFO_KIND.標準).ToString();
                        temp[item.i].infoKindName = item.value.InfoKind.HasValue ? $"{item.value.InfoKind} {item.value.infoKindName}" : $"{(int)INFO_KIND.標準} {INFO_KIND.標準}";
                        temp[item.i].type = item.value.Type?.ToString() ?? ((char)TYPE.物料).ToString();
                        temp[item.i].typeName = string.IsNullOrWhiteSpace(item.value.Type) ? $"{(char)TYPE.物料} {TYPE.物料}" : $"{item.value.Type} {((TYPE)Convert.ToChar(item.value.Type))}";
                        temp[item.i].ekgry = item.value.Ekgry ?? rfqH.ekgry;
                        temp[item.i].taxcode = item.value.Taxcode?.ToString() ?? "V4";
                        temp[item.i].taxcodeName = string.IsNullOrWhiteSpace(item.value.Taxcode) ? "V4 進項稅5%" : $"{item.value.Taxcode} {item.value.taxcodeName}";
                        temp[item.i].effectiveDate = (item.value.EffectiveDate.HasValue) ? item.value.EffectiveDate.Value.ToString("yyyy/MM/dd") : DateTime.Now.ToString("yyyy/MM/dd");
                        //temp[item.i].expirationDate = (item.value.ExpirationDate.HasValue) ? item.value.ExpirationDate.Value.ToString("yyyy/MM/dd") : new DateTime(DateTime.Now.Year + 1, 1, 1).AddDays(-1).ToString("yyyy/MM/dd");
                        temp[item.i].expirationDate = (item.value.ExpirationDate.HasValue) ? item.value.ExpirationDate.Value.ToString("yyyy/MM/dd") : qot.ExpirationDate.HasValue? qot.ExpirationDate.Value.ToString("yyyy/MM/dd") : new DateTime(DateTime.Now.Year + 1, 1, 1).AddDays(-1).ToString("yyyy/MM/dd");
                        temp[item.i].note = item.value.Note;
                        temp[item.i].sortl = item.value.Sortl;
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
            if (rfqH.Status.Value != (int)Status.確認 && rfqH.Status.Value != (int)Status.簽核中 && rfqH.Status.Value != (int)Status.完成) {
                return this.BadRequestResult("詢價單狀態異常");
            }
            ViewSrmInfoRecord[] infos = jobj["infos"].ToObject<ViewSrmInfoRecord[]>();
            UserClaims user = User.GetUserClaims();
            if (user.UserName != rfqH.Sourcer)
            {
                return this.BadRequestResult("非詢價本人");
            }
            string logonid = user.UserName;
            DateTime now = DateTime.Now;
            foreach (ViewSrmInfoRecord info in infos)
            {
                    info.Status = (int)Status.簽核中;
                    info.CreateDate = now;
                    info.CreateBy = logonid;
                    info.LastUpdateDate = now;
                    info.LastUpdateBy = logonid;
                var matnr = _srmMatnrService.GetMatnrById(info.MatnrId.Value);
                if (string.IsNullOrWhiteSpace(matnr.SapMatnr)) { 
                    return this.BadRequestResult($"報價單號:{info.qotNum}，料號:{matnr.SrmMatnr1}，SapMatnr未存在");
                }
                var vendor = _srmVendorService.GetVendorById(info.VendorId.Value);
                if (string.IsNullOrWhiteSpace(vendor.SapVendor))
                {
                    return this.BadRequestResult($"報價單號:{info.qotNum}，供應商:{matnr.SrmMatnr1}，SapVendor未存在");
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

        [HttpPost("Save")]
        [Permission("price")]
        public IActionResult Save(JObject jobj) {
            int rfqId = (int)jobj["rfqId"];
            int caseId = (int)jobj["caseId"];
            var rfqH = _srmRfqHService.GetDataByRfqId(rfqId);
            if (rfqH.Status.Value != (int)Status.確認 && rfqH.Status.Value != (int)Status.簽核中 && rfqH.Status.Value != (int)Status.完成)
            {
                return this.BadRequestResult("詢價單狀態異常");
            }
            ViewSrmInfoRecord[] infos = jobj["infos"].ToObject<ViewSrmInfoRecord[]>();
            UserClaims user = User.GetUserClaims();
            if (user.UserName != rfqH.Sourcer)
            {
                return this.BadRequestResult("非詢價本人");
            }
            string logonid = user.UserName;
            DateTime now = DateTime.Now;
            foreach (ViewSrmInfoRecord info in infos)
            {
                if (info.InfoId == 0)
                {
                    info.CreateDate = now;
                    info.CreateBy = logonid;
                }
                if (info.Caseid.HasValue && info.Caseid.Value != caseId) {
                    throw new Exception("caseId不一致");
                }
                info.Status = (int)Status.初始;
                info.Caseid = caseId;
                info.LastUpdateDate = now;
                info.LastUpdateBy = logonid;
                var matnr = _srmMatnrService.GetMatnrById(info.MatnrId.Value);
                if (string.IsNullOrWhiteSpace(matnr.SapMatnr))
                {
                    return this.BadRequestResult($"報價單號:{info.qotNum}，料號:{matnr.SrmMatnr1}，SapMatnr未存在");
                }
                var vendor = _srmVendorService.GetVendorById(info.VendorId.Value);
                if (string.IsNullOrWhiteSpace(vendor.SapVendor))
                {
                    return this.BadRequestResult($"報價單號:{info.qotNum}，供應商:{matnr.SrmMatnr1}，SapVendor未存在");
                }
            }
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    _srmPriceService.Save(infos);
                    UpdateCF(rfqH, infos, caseId);
                    transaction.Complete();
                    return Ok();
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    return this.BadRequestResult(ex.Message);
                }
            }
        }

        private DataSet GetCfInfoRecord(ViewSrmRfqH rfqH, ViewSrmInfoRecord[] infos) {
            DataSet ds = new DataSet();
            DataTable InfoRecord = new DataTable("CF_InfoRecord");
            //InfoRecord.Columns.Add("SapMatnr");
            InfoRecord.Columns.Add("DESCRIPTION");
            InfoRecord.Columns.Add("QTY");
            InfoRecord.Columns.Add("Height");
            InfoRecord.Columns.Add("Length");
            InfoRecord.Columns.Add("Width");
            InfoRecord.Columns.Add("Weight");
            InfoRecord.Columns.Add("Gewei");
            InfoRecord.Columns.Add("VendorName");
            InfoRecord.Columns.Add("ATotal");
            InfoRecord.Columns.Add("BTotal");
            InfoRecord.Columns.Add("CTotal");
            InfoRecord.Columns.Add("DTotal");
            InfoRecord.Columns.Add("Total");
            InfoRecord.Columns.Add("Price");
            InfoRecord.Columns.Add("Currency");
            InfoRecord.Columns.Add("HistoricalPrice");
            InfoRecord.Columns.Add("HistoricalDate");
            InfoRecord.Columns.Add("FirstPrice");//前次改成首次
            InfoRecord.Columns.Add("FirstDate");//前次改成首次
            InfoRecord.Columns.Add("HistoricalBargainingRate");
            InfoRecord.Columns.Add("FirstBargainingRate");//前次改成首次
            InfoRecord.Columns.Add("InfoId");
            //InfoRecord.Columns.Add("Img1");
            //InfoRecord.Columns.Add("Img2");
            InfoRecord.Columns.Add("Note");
            InfoRecord.Columns.Add("Org");
            InfoRecord.Columns.Add("InfoKind");
            InfoRecord.Columns.Add("InfoKindName");
            InfoRecord.Columns.Add("Type");
            InfoRecord.Columns.Add("TypeName");
            InfoRecord.Columns.Add("SAP_VENDOR");
            InfoRecord.Columns.Add("SAP_MATNR");
            InfoRecord.Columns.Add("MATNR_GROUP");
            InfoRecord.Columns.Add("EKGRY");
            InfoRecord.Columns.Add("LEAD_TIME");
            InfoRecord.Columns.Add("STAND_QTY");
            InfoRecord.Columns.Add("MIN_QTY");
            InfoRecord.Columns.Add("TAXCODE");
            InfoRecord.Columns.Add("UNIT");
            InfoRecord.Columns.Add("EFFECTIVE_DATE");
            InfoRecord.Columns.Add("EXPIRATION_DATE");
            InfoRecord.Columns.Add("Sortl");
            InfoRecord.Columns.Add("MeasureUnit");
            InfoRecord.Columns.Add("MeasureDesc");
            InfoRecord.Columns.Add("File1");

            foreach (var info in infos)
            {
                var rfqM = _srmRfqMService.GetRfqMData(new SrmRfqM() { RfqId = rfqH.RfqId, MatnrId = info.MatnrId });
                DataRow dr = InfoRecord.NewRow();
                //dr["SapMatnr"] = rfqM.matnr;
                dr["DESCRIPTION"] = rfqM.Description;
                dr["QTY"] = rfqM.Qty;
                dr["Height"] = rfqM.Height;
                dr["Length"] = rfqM.Length;
                dr["Width"] = rfqM.Width;
                dr["Weight"] = rfqM.Weight;
                dr["Gewei"] = rfqM.Gewei;
                dr["VendorName"] = info.VendorName;
                dr["ATotal"] = info.Atotal;
                dr["BTotal"] = info.Btotal;
                dr["CTotal"] = info.Ctotal;
                dr["DTotal"] = info.Dtotal;
                dr["Total"] = info.total;
                dr["Price"] = info.Price;
                dr["Currency"] = info.Currency;
                dr["InfoId"] = info.InfoId;
                //dr["Img1"] = "/BPM/images/logo.jpg";
                //dr["Img2"] = "/BPM/images/logo2.png";
                dr["Note"] = info.Note;
                dr["Org"] = info.Org;
                dr["InfoKind"] = info.InfoKind;
                dr["InfoKindName"] = $"{info.InfoKind} {info.infoKindName}";
                dr["Type"] = info.Type;
                dr["TypeName"] = $"{info.Type} {info.typeName}";
                dr["SAP_VENDOR"] = info.vendorObject.SapVendor;
                dr["SAP_MATNR"] = info.matnrObject.SapMatnr;
                dr["MATNR_GROUP"] = info.matnrObject.MatnrGroup;
                dr["EKGRY"] = info.Ekgry;
                dr["LEAD_TIME"] = info.LeadTime;
                dr["STAND_QTY"] = info.StandQty;
                dr["MIN_QTY"] = info.MinQty;
                dr["TAXCODE"] = info.Taxcode;
                dr["UNIT"] = info.Unit;
                dr["EFFECTIVE_DATE"] = info.EffectiveDate.Value.ToString("yyyy/MM/dd");
                dr["EXPIRATION_DATE"] = info.ExpirationDate.Value.ToString("yyyy/MM/dd");
                dr["Sortl"] = info.Sortl;
                dr["MeasureUnit"] = rfqM.Unit;
                dr["MeasureDesc"] = rfqM.MeasureDesc;
                dr["File1"] = "";
                var FirstPrice = _srmHistoryPriceService.GetHistoryPrice(new QuerySrmHistoryPrice() { Matnr = info.Type.ToUpper() == "M" ? info.matnrObject.SapMatnr : string.Empty, Essay = info.Type.ToUpper() == "W" ? rfqM.Description : string.Empty, orderASC = true });
                if (FirstPrice!=null)
                {
                    dr["FirstPrice"] = FirstPrice.HistoryPrice.Value;
                    dr["FirstDate"] = FirstPrice.OrderDate.Value.ToString("yyyy/MM/dd");
                    dr["FirstBargainingRate"] = Math.Round((decimal)(((info.Price - FirstPrice.HistoryPrice.Value)) / FirstPrice.HistoryPrice.Value*100), 2, MidpointRounding.AwayFromZero).ToString() + " % ";
                }
                var HistoricalPrice = _srmHistoryPriceService.GetHistoryPrice(new QuerySrmHistoryPrice() { Matnr = info.Type.ToUpper() == "M" ? info.matnrObject.SapMatnr : string.Empty, Essay = info.Type.ToUpper() == "W" ? rfqM.Description : string.Empty, orderASC = false,year=(rfqH.CreateDate.Value.Year-1) });
                if (HistoricalPrice != null) {
                    dr["HistoricalPrice"] = HistoricalPrice.HistoryPrice.Value;
                    dr["HistoricalDate"] = HistoricalPrice.OrderDate.Value.ToString("yyyy/MM/dd");
                    dr["HistoricalBargainingRate"] = Math.Round((decimal)(((info.Price - HistoricalPrice.HistoryPrice.Value)) / HistoricalPrice.HistoryPrice.Value * 100), 2, MidpointRounding.AwayFromZero).ToString() + " % ";
                }
                InfoRecord.Rows.Add(dr);
            }
            var view = InfoRecord.AsDataView();
            view.Sort = "HistoricalBargainingRate desc";
            ds.Tables.Add(view.ToTable());
            return ds;
        }

        private void UpdateCF(ViewSrmRfqH rfqH, ViewSrmInfoRecord[] infos,int caseId) {
            BasicHttpBinding binding = new BasicHttpBinding();

            EndpointAddress address = new EndpointAddress("http://10.1.1.180/CF.BPM.Service/BPMAPI.asmx");

            BPMAPISoapClient client = new BPMAPISoapClient(binding, address);

            CallMethodParams callMethodParm = new CallMethodParams();//GetBorgEmpDataByLogonID
            JObject param = new JObject();
            param.Add("logonid", rfqH.Sourcer);
            param.Add("caseId", caseId);
            param.Add("ds", JObject.FromObject(GetCfInfoRecord(rfqH, infos)));
            callMethodParm.Method = "UpdateCF";
            callMethodParm.Options = JsonConvert.SerializeObject(param);
            Task<CallMethodResponse> response = client.CallMethodAsync(callMethodParm);
            CallMethodResult result = response.Result.Body.CallMethodResult;
            if (!result.Success)
            {
                throw new Exception(result.Message);
            }
        }

        private void RunBorg(ViewSrmRfqH rfqH, ViewSrmInfoRecord[] infos)
        {
            BasicHttpBinding binding = new BasicHttpBinding();

            EndpointAddress address = new EndpointAddress("http://10.1.1.180/CF.BPM.Service/BPMAPI.asmx");

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
            param.Add("logonid", rfqH.Sourcer);
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

            param.Add("Variables",JObject.FromObject(Variables));
            param.Add("FormControls",JObject.FromObject(FormControls));
            param.Add("ds", JObject.FromObject(GetCfInfoRecord(rfqH,infos)));
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
        public IActionResult GetEkgry()
        {
            UserClaims user = User.GetUserClaims();
            return Ok(_srmPriceService.GetEkgry(user.Werks));
        }

        [HttpPost("QueryInfoRecord")]
        [Permission("price")]
        public IActionResult QueryInfoRecord(QueryInfoRecordModels query)
        {
            UserClaims user = User.GetUserClaims();
            query.werks = user.Werks;
            return Ok(_srmInfoRecordService.Query(query));
        }
        [HttpPost("GetIssuedVendor")]
        [Permission("price")]
        public IActionResult GetIssuedVendor(QueryInfoRecordModels query)
        {
            UserClaims user = User.GetUserClaims();
            query.werks = user.Werks;
            return Ok(_srmInfoRecordService.GetIssuedVendor(query));
        }
    }
}
