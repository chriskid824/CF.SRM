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

        public SrmPriceController(ISrmPriceService srmPriceService
            , ISrmQotService srmQotService
            , ISrmRfqHService srmRfqHService
            , ISrmRfqMService srmRfqMService
            , ISrmVendorService srmVendorService
            , ISrmMatnrService srmMatnrService
            )
        {
            _srmQotService = srmQotService;
            _srmPriceService = srmPriceService;
            _srmRfqMService = srmRfqMService;
            _srmRfqHService = srmRfqHService;
            _srmVendorService = srmVendorService;
            _srmMatnrService = srmMatnrService;
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
            var qots = (_srmQotService.Get(query));
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
                temp[0].RfqNum = rfqH.RfqNum;
                temp[0].Status = rfqH.Status;
                temp[0].sourcerName = rfqH.sourcerName;
                temp[0].Deadline = rfqH.Deadline;
                temp[0].vendor = vendor.SrmVendor1;
                temp[0].vendorName = vendor.VendorName;
                temp[0].matnr = matnr.srmMatnr;
                temp[0].material = matnr.Material;
                temp[0].volume = $"{matnr.Length}*{matnr.Width}*{matnr.Height}";
                temp[0].weight = matnr.Weight.HasValue? matnr.Weight.Value.ToString():"";
                temp[0].machineName = matnr.MachineName;
                temp[0].qotNum = qot.QotNum;
                temp[0].qotId = qot.QotId.ToString();
                foreach (var item in material.Select((value, i) => new { i, value })) {
                    temp[item.i].mMaterial = item.value.MMaterial;
                    temp[item.i].mPrice = item.value.MPrice?.ToString()??"";
                    temp[item.i].mLength = item.value.Length?.ToString()??"";
                    temp[item.i].mWidth = item.value.Width?.ToString()??"";
                    temp[item.i].mHeight = item.value.Height?.ToString() ?? "";
                    temp[item.i].mDensity = item.value.Density?.ToString() ?? "";
                    temp[item.i].mWeight = item.value.Weight?.ToString()??"";
                    temp[item.i].mCostPrice = item.value.MCostPrice?.ToString() ?? "";
                    temp[item.i].mNote = item.value.Note?.ToString() ?? "";
                }

                foreach (var item in process.Select((value, i) => new { i, value }))
                {
                    temp[item.i].pMachine = item.value.PMachine;
                    temp[item.i].pProcessNum = item.value.PProcessNum?.ToString() ?? "";
                    temp[item.i].pHours = item.value.PHours?.ToString() ?? "";
                    temp[item.i].pPrice = item.value.PPrice?.ToString() ?? "";
                    temp[item.i].pSubTotal = item.value.SubTotal.ToString();
                    temp[item.i].pNote = item.value.PNote;
                }

                foreach (var item in surface.Select((value, i) => new { i, value }))
                {
                    temp[item.i].sProcess = item.value.SProcess;
                    temp[item.i].sTimes = item.value.STimes?.ToString() ?? "";
                    temp[item.i].sPrice = item.value.SPrice?.ToString() ?? "";
                    temp[item.i].sSubTotal = item.value.SubTotal.ToString();
                    //todo 哀
                    //temp[item.i].sMethod = item.value.method.ToString();
                    temp[item.i].sNote = item.value.SNote;
                }

                foreach (var item in other.Select((value, i) => new { i, value }))
                {
                    temp[item.i].oItem = item.value.OItem;
                    temp[item.i].oDescription = item.value.ODescription;
                    temp[item.i].oPrice = item.value.OPrice?.ToString() ?? "";
                    temp[item.i].oNote = item.value.ONote;
                }

                foreach (var item in infoRecord.Select((value, i) => new { i, value }))
                {
                    temp[item.i].aTotal = item.value.Atotal.ToString();
                    temp[item.i].bTotal = item.value.Btotal.ToString();
                    temp[item.i].cTotal = item.value.Ctotal.ToString();
                    temp[item.i].dTotal = item.value.Dtotal.ToString();
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
            }
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    rfqId = _srmPriceService.Start(infos).Value;
                    _srmRfqHService.UpdateStatus((int)Status.簽核中, new SrmRfqH { RfqId = rfqId, LastUpdateDate = now, LastUpdateBy = logonid }); 
                    transaction.Complete();
                    return Ok();
                }
                catch (Exception ex) {
                    transaction.Dispose();
                    return this.BadRequestResult(ex.Message);
                }
            }
        }
    }
}
