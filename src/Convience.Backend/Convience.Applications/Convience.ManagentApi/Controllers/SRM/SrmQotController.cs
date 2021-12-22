using Convience.Entity.Entity.SRM;
using Convience.JwtAuthentication;
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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using NPOI.HSSF.Util;

namespace Convience.ManagentApi.Controllers.SRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SrmQotController : ControllerBase
    {
        private readonly ISrmQotService _srmQotService;
        private readonly ISrmRfqMService _srmRfqMService;
        private readonly appSettings _appSettingsService;
        private  Dictionary<string, string> excelColumn = new Dictionary<string, string>();
        private readonly ISrmFileService _srmFileService;

        public SrmQotController(
            ISrmQotService srmQotService, 
            ISrmRfqMService srmRfqMService, 
            IOptions<appSettings> appSettingsOption,
            ISrmFileService srmFileService)
        {
            _srmQotService = srmQotService;
            _srmRfqMService = srmRfqMService;
            _appSettingsService = appSettingsOption.Value;
            _srmFileService = srmFileService;
        }
        [HttpPost("GetQotList")]
        public string GetQotList(JObject query)
        {
            if (query == null)
            {
                return JsonConvert.SerializeObject(_srmQotService.GetQotList(), Formatting.None,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
            }
            QueryQotList qot = new QueryQotList();
            //var aaa = query.Property("poNum");
            if (query.Property("rfqId") != null)
            {
                qot.rfqId = (int)query["rfqId"];
            }
            var rfqno = (query["RFQ_NUM"] != null) ? query["RFQ_NUM"].ToString() : null;
            var matnr = (query["MATNR"] != null) ? query["MATNR"].ToString() : null;
            var status = (query["STATUS"] != null) ? (int)query["STATUS"] : 0;
            var vendor = (string)query["vendor"];
            qot.rfqno = rfqno;
            qot.status = status;//(int)query["status"];
            qot.matnr = matnr;
            qot.vendor = vendor;
            var result =  _srmQotService.GetQotList(qot);
            //if (query["vendor"].ToString().IndexOf("admin") != -1)
            //{
            //    result = _srmQotService.GetQotListByAdmin(qot);
            //}
            
            //var result = _srmQotService.GetQotListByAdmin(qot); 

            return JsonConvert.SerializeObject(result, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
        }
        //[HttpGet("GetQotData")]
        //public IActionResult GetQotData(int id)
        //{
        //    //ViewQot qot = (ViewQot)_srmQotService.GetDataBQotId(id);          
        //    //return this.BadRequestResult("12334");
        //    return Ok(
        //        );
        //}

        [HttpGet("GetQotData")]
        public IActionResult GetQotData(int id, int rfqid, int vendorid)
        {
            //for tree?
            //ViewSrmRfqH h = _srmRfqHService.GetDataByRfqId(id);
            //h.sourcerName = _userService.GetUsers(new UserQueryModel() { UserName = h.Sourcer, Page = 1, Size = 1 }).Data[0].Name;
            //h.C_by = _userService.GetUsers(new UserQueryModel() { UserName = h.CreateBy, Page = 1, Size = 1 }).Data[0].Name;
            
            
            System.Linq.IQueryable QotV = _srmQotService.GetQotData(rfqid, vendorid,id); //表單欄位用
            System.Linq.IQueryable matnr = _srmQotService.GetMatnrData(rfqid, vendorid); //表單欄位用

            //ViewSrmRfqV[] v = _srmRfqVService.GetDataByRfqId(id);
            //Newtonsoft.Json.JsonSerializer js = new Newtonsoft.Json.JsonSerializer();
            //js.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            ////JObject rfq = new JObject() {
            ////    { "h",JObject.FromObject(h,js)},
            ////    { "m",JArray.FromObject(m,js)},
            ////    { "v",JArray.FromObject(v,js)},
            ////};
            ResultQotModel qot = new ResultQotModel()
            {
                m = matnr,
                //h = h,
                q = QotV,
                //v = v
            };
            //return this.BadRequestResult("12334");
            return Ok(qot);
        }
        [HttpPost("GetQotInfo")]
        public IActionResult GetQotDetail(QueryQot query)
        {
            var qots = (_srmQotService.GetByVendor(query));
            //var qots = (_srmQotService.Get(query)); //因admin需看到全部
            ViewQotResult detail = _srmQotService.GetDetail(query);
            SrmRfqM m = new SrmRfqM()
            {
                RfqId = query.rfqId,
                MatnrId = query.matnrId
            };
            detail.matnr = _srmRfqMService.GetRfqMData(m);
            detail.qot = _srmQotService.GetQot( query);
            return Ok(detail);
        }

        [HttpPost("Save")]
        //[Permission("rfq")]//???
        public IActionResult Save(JObject qot)
        {
            SrmQotH q = qot["q"].ToObject<SrmQotH>();
            SrmQotMaterial[] m = qot["material"].ToObject<SrmQotMaterial[]>();
            SrmQotSurface[] s = qot["surface"].ToObject<SrmQotSurface[]>();
            SrmQotProcess[] p = qot["process"].ToObject<SrmQotProcess[]>();
            SrmQotOther[] o = qot["other"].ToObject<SrmQotOther[]>();
            DateTime now = DateTime.Now;
            q.LastUpdateDate = now;
            _srmQotService.Save(q, m, s, p, o);
            return Ok();
        }

        
        [HttpPost("Reject")]
        //[Permission("rfq")]
        public IActionResult Reject(JObject qot)
        {
            try
            {
                SrmQotUpdateMaterial q = qot["q"].ToObject<SrmQotUpdateMaterial>();           
                q.LastUpdateDate = DateTime.Now;
                _srmQotService.UpdateQotStatus(((int)Status.拒絕), q);
                _srmQotService.InsertRejectReason(q);
                //確認所有qot
                bool IfUpdateRfq = _srmQotService.CheckAllQot(q);
                //更新rfq
                if (IfUpdateRfq)
                {                   
                    _srmQotService.UpdateRfqStatus(((int)Status.確認), q);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return this.BadRequestResult(ex.Message);
            }
        }

        

        
        [HttpPost("GetRowNum")]
        //[Permission("rfq")]//???
        public int GetRowNum(JObject qot)
        {
            int index = 0;

            SrmQotH q = qot["q"].ToObject<SrmQotH>();
            index = _srmQotService.GetRowNum(q);
            return index;
        }

        [HttpPost("GetProcess")]
        public IActionResult GetProcess()
        {
            return Ok(_srmQotService.GetProcess());
        }
        [HttpPost("GetSurface")]
        public IActionResult GetSurface()
        {
            return Ok(_srmQotService.GetSurface());
        }
        [HttpPost("GetMaterial")]
        public IActionResult GetMaterial()
        {
            return Ok(_srmQotService.GetMaterial());
        }
        [HttpPost("GetProcessByNum")]
        public IActionResult GetProcessByNum(int num)
        {
            return Ok(_srmQotService.GetProcessByNum(num));
        }
        [HttpPost("Send")]
        //[Permission("rfq")]//???
        public IActionResult Send(JObject qot)
        {
            SrmQotH q = qot["q"].ToObject<SrmQotH>();
            SrmQotMaterial[] m = qot["material"].ToObject<SrmQotMaterial[]>();
            SrmQotSurface[] s = qot["surface"].ToObject<SrmQotSurface[]>();
            SrmQotProcess[] p = qot["process"].ToObject<SrmQotProcess[]>();
            SrmQotOther[] o = qot["other"].ToObject<SrmQotOther[]>();
            DateTime now = DateTime.Now;
            q.LastUpdateDate = now;
            _srmQotService.Save(q, m, s, p, o);
            SrmQotUpdateMaterial qu = qot["q"].ToObject<SrmQotUpdateMaterial>();
            _srmQotService.UpdateQotStatus(((int)Status.確認), qu);
            //確認所有qot
            bool IfUpdateRfq = _srmQotService.CheckAllQot(q);
            //更新rfq
            if (IfUpdateRfq)
            {
                _srmQotService.UpdateRfqStatus(((int)Status.確認), qu);
            }
            return Ok();
        }
        /*[HttpPost("SendAllQot")]
        //[Permission("rfq")]//???
        public string SendAllQot(JObject qot)
        {
            //int vendorid = _srmQotService.GetVendorId(qot);
            //string msg = _srmQotService.SendAll(int.Parse(qot["q"]["RfqId"].ToString()), int.Parse(qot["q"]["vendorId"].ToString()));
            //無錯誤訊息則變更狀態:初始 -> 
            if (string.IsNullOrWhiteSpace(msg)) 
            {
                //SrmQotUpdateMaterial qu = qot["q"].ToObject<SrmQotUpdateMaterial>();
                //string aa = qot["q"][""].ToString()
                //_srmQotService.UpdateQotStatus(((int)Status.確認), qu);
                ////確認所有qot
                //bool IfUpdateRfq = _srmQotService.CheckAllQot(q);
                ////更新rfq
                //if (IfUpdateRfq)
                //{
                //    _srmQotService.UpdateRfqStatus(((int)Status.確認), qu);
                //}
            }
            return msg;
        }*/
        [HttpPost("Download")]
        //[Permission("rfq")]//???
        public IActionResult Download(QueryQot query)
        {
            try
            {
                string RNo = string.Empty;
                string MatnrNo = string.Empty;
                string Desc = string.Empty;
                #region 取得報價資訊(詢價單、料號)
                ViewQotResult qotdetail = _srmQotService.GetDetailByVendorRfq(query);
                SrmRfqM m = new SrmRfqM()
                {
                    RfqId = query.rfqId,
                    MatnrId = query.matnrId
                };
                IQueryable QotV = _srmQotService.GetQotData(query.rfqId.Value, query.vendorId.Value, query.qotId.Value);
                var qots = Queryable.Cast<object>(QotV);
                #endregion

                IWorkbook wb = new HSSFWorkbook();
                ISheet ws = wb.CreateSheet("報價單表頭");
                ISheet ws1 = wb.CreateSheet("材料");
                ISheet ws2 = wb.CreateSheet("加工");
                ISheet ws3 = wb.CreateSheet("表面處理");
                ISheet ws4 = wb.CreateSheet("其他");

                //ICellStyle s = wb.CreateCellStyle();
                //s.FillBackgroundColor = HSSFColor.Yellow.Index;

                ICellStyle s = wb.CreateCellStyle();
                ICellStyle s1 = wb.CreateCellStyle();
                s.FillForegroundColor = HSSFColor.Yellow.Index;
                s.FillPattern = FillPattern.SolidForeground;
                
                HSSFFont font1 = (HSSFFont)wb.CreateFont();
                font1.FontName = "Microsoft JhengHei";
                font1.FontHeightInPoints = 12;
                s1.SetFont(font1);
                s.SetFont(font1);


                #region header
                InitExcelColumn("報價單表頭");
                ws.CreateRow(0);//第一行為欄位名稱
                //set 表頭欄位
                for (int i = 0; i < excelColumn.Count; i++)
                {
                    KeyValuePair<string, string> column = excelColumn.ElementAt(i);
                    ws.GetRow(0).CreateCell(i).SetCellValue(column.Value);
                    ws.GetRow(0).GetCell(i).CellStyle = s1;// 20211222 add style                   
                }
                int ii = 1;
                foreach (var q in qots)
                {
                    RNo = q.GetType().GetProperties()[0].GetValue(q).ToString();
                    MatnrNo = q.GetType().GetProperties()[1].GetValue(q).ToString();
                    Desc = q.GetType().GetProperties()[2].GetValue(q).ToString();
                 
                    var row2 = ws.CreateRow(ii);
                    var RfqNum = q.GetType().GetProperties()[0].GetValue(q);
                    var Matnr = q.GetType().GetProperties()[1].GetValue(q);
                    var Description = q.GetType().GetProperties()[2].GetValue(q);
                    var estdate = (q.GetType().GetProperties()[3].GetValue(q) == null) ? "" : q.GetType().GetProperties()[3].GetValue(q).ToString();
                    var leadtime = (q.GetType().GetProperties()[4].GetValue(q) == null) ? "" : q.GetType().GetProperties()[4].GetValue(q).ToString();
                    var note = (q.GetType().GetProperties()[5].GetValue(q) ==null )?"":q.GetType().GetProperties()[5].GetValue(q).ToString();
                    

                  
                    row2.CreateCell(0).SetCellValue((string)RfqNum);              
                    row2.CreateCell(1).SetCellValue((string)Matnr);
                    row2.CreateCell(2).SetCellValue((string)Description);
                    row2.CreateCell(3).SetCellValue((string)estdate);
                    row2.CreateCell(4).SetCellValue((string)leadtime);
                    row2.CreateCell(5).SetCellValue((string)note);
                    // 20211222 add style  
                    row2.GetCell(0).CellStyle = s1;
                    row2.GetCell(1).CellStyle = s1;
                    row2.GetCell(2).CellStyle = s1;
                    row2.GetCell(3).CellStyle = s1;
                    row2.GetCell(4).CellStyle = s1;
                    row2.GetCell(5).CellStyle = s1;

                    /*取得[計畫交貨時間(日曆天)]、有效期限、備註*/

                    /**/
                    ii++;
                }
                ws.GetRow(0).GetCell(0).CellStyle = s;
                ws.GetRow(0).GetCell(1).CellStyle = s;
                ws.GetRow(0).GetCell(2).CellStyle = s;
                ws.GetRow(0).GetCell(3).CellStyle = s;
                ws.GetRow(0).GetCell(4).CellStyle = s;
            
                #endregion


                /**/
                //loop資料進excel
                #region 材料
                InitExcelColumn("材料");
                ws1.CreateRow(0);
                for (int i1 = 0; i1 < excelColumn.Count; i1++)
                {
                    KeyValuePair<string, string> column1 = excelColumn.ElementAt(i1);
                    ws1.GetRow(0).CreateCell(i1).SetCellValue(column1.Value);
                    ws1.GetRow(0).GetCell(i1).CellStyle = s1;// 20211222 add style  
                }
                int im= 1;
                SrmQotMaterial[] materials = qotdetail.material;
                if (materials.Count() == 0)
                {
                    ii = 1;
                    foreach (var q in qots)
                    {
                        var rowm0 = ws1.CreateRow(ii);
                        var RfqNum = q.GetType().GetProperties()[0].GetValue(q);
                        var Matnr = q.GetType().GetProperties()[1].GetValue(q);
                        var Description = q.GetType().GetProperties()[2].GetValue(q);
                        rowm0.CreateCell(0).SetCellValue((string)RfqNum);
                        rowm0.CreateCell(1).SetCellValue((string)Matnr);
                        rowm0.CreateCell(2).SetCellValue((string)Description);

                        // 20211222 add style                     
                        rowm0.GetCell(0).CellStyle = s1;
                        rowm0.GetCell(1).CellStyle = s1;
                        rowm0.GetCell(2).CellStyle = s1;

                        var mEmptyFlag = q.GetType().GetProperties()[6].GetValue(q);
                        if (mEmptyFlag != null) 
                        {
                            if (mEmptyFlag.ToString() == "X")
                            {
                                rowm0.CreateCell(3).SetCellValue("Y");
                                rowm0.GetCell(3).CellStyle = s1;// 20211222 add style  
                            }
                        }
                        ii++;
                    }
                }
                else 
                {
                    foreach (var material in materials)
                    {
                        query.qotId = material.QotId;
                        var qotdata = _srmQotService.GetQotInfo(int.Parse(query.rfqId.ToString()), int.Parse(query.vendorId.ToString()), int.Parse(material.QotId.ToString()));
                        var qq = Queryable.Cast<object>(qotdata).FirstOrDefault();
                        RNo = qq.GetType().GetProperties()[0].GetValue(qq).ToString();
                        MatnrNo = qq.GetType().GetProperties()[1].GetValue(qq).ToString();
                        Desc = qq.GetType().GetProperties()[2].GetValue(qq).ToString();
                        //var mEmptyFlag = qq.GetType().GetProperties()[2].GetValue(qq).ToString();
                        var rowm = ws1.CreateRow(im);                       
                        rowm.CreateCell(0).SetCellValue(RNo);
                        rowm.CreateCell(1).SetCellValue(MatnrNo);
                        rowm.CreateCell(2).SetCellValue((Desc));
                        rowm.CreateCell(3).SetCellValue("");//不回填??

                        rowm.GetCell(0).CellStyle = s1;// 20211222 add style  
                        rowm.GetCell(1).CellStyle = s1;// 20211222 add style  
                        rowm.GetCell(2).CellStyle = s1;// 20211222 add style  
                        if (material.MMaterial != null)
                        {
                            rowm.CreateCell(4).SetCellValue(material.MMaterial);
                            rowm.GetCell(4).CellStyle = s1;// 20211222 add style  
                        }
                        if (material.Length != null) 
                        {
                            rowm.CreateCell(5).SetCellValue(material.Length.Value);
                            rowm.GetCell(5).CellStyle = s1;// 20211222 add style  
                        }
                        if (material.Width !=null) 
                        {
                            rowm.CreateCell(6).SetCellValue(material.Width.Value);
                            rowm.GetCell(6).CellStyle = s1;// 20211222 add style  
                        }
                        if (material.Height !=null) 
                        {
                            rowm.CreateCell(7).SetCellValue(material.Height.Value);
                            rowm.GetCell(7).CellStyle = s1;// 20211222 add style  
                        }
                        if (material.Density !=null)
                        {
                            rowm.CreateCell(8).SetCellValue(material.Density.Value);
                            rowm.GetCell(8).CellStyle = s1;// 20211222 add style  
                        }
                        if (material.Weight != null)
                        {
                            rowm.CreateCell(9).SetCellValue(material.Weight.Value);
                            rowm.GetCell(9).CellStyle = s1;// 20211222 add style  
                        }
                        if (material.MPrice != null)
                        {
                            rowm.CreateCell(10).SetCellValue(double.Parse(material.MPrice.ToString()));
                            rowm.GetCell(10).CellStyle = s1;// 20211222 add style  
                        }
                        if (material.MCostPrice !=null) 
                        {
                            rowm.CreateCell(11).SetCellValue(double.Parse(material.MCostPrice.ToString()));
                            rowm.GetCell(11).CellStyle = s1;// 20211222 add style  
                        }
                        if (material.Note !=null)
                        {
                            rowm.CreateCell(12).SetCellValue(material.Note);
                            rowm.GetCell(12).CellStyle = s1;// 20211222 add style  
                        }
                        im++;
                    }
                    int cm = materials.Count()+1;
                    foreach (var q in qots)
                    {
                        var rowm = ws1.CreateRow(cm);
                        var RfqNum = q.GetType().GetProperties()[0].GetValue(q);
                        var Matnr = q.GetType().GetProperties()[1].GetValue(q);
                        var Description = q.GetType().GetProperties()[2].GetValue(q);
                        var mEmptyFlag = q.GetType().GetProperties()[6].GetValue(q);
                        if (mEmptyFlag != null) 
                        {
                            if (mEmptyFlag.ToString() == "X")
                            {
                                rowm.CreateCell(0).SetCellValue((string)RfqNum);
                                rowm.CreateCell(1).SetCellValue((string)Matnr);
                                rowm.CreateCell(2).SetCellValue((string)Description);
                                rowm.CreateCell(3).SetCellValue("Y");
                                // 20211222 add style 
                                rowm.GetCell(0).CellStyle = s1; 
                                rowm.GetCell(1).CellStyle = s1;
                                rowm.GetCell(2).CellStyle = s1;
                                rowm.GetCell(3).CellStyle = s1;
                                cm++;
                            }
                        }
                    }

                }
                ws1.GetRow(0).GetCell(0).CellStyle = s;
                ws1.GetRow(0).GetCell(1).CellStyle = s;
                ws1.GetRow(0).GetCell(2).CellStyle = s;
                ws1.GetRow(0).GetCell(4).CellStyle = s;
                ws1.GetRow(0).GetCell(9).CellStyle = s;
                ws1.GetRow(0).GetCell(10).CellStyle = s;
                #endregion
                #region 加工

                InitExcelColumn("加工");
                
                ws2.CreateRow(0);
                for (int i2 = 0; i2 < excelColumn.Count; i2++)
                {
                    KeyValuePair<string, string> column = excelColumn.ElementAt(i2);
                    ws2.GetRow(0).CreateCell(i2).SetCellValue(column.Value);
                    ws2.GetRow(0).GetCell(i2).CellStyle = s1;// 20211222 add style  
                }
                int ip = 1;
                SrmQotProcess[] processs = qotdetail.process;
                if (processs.Count() == 0)
                {
                    ii = 1;
                    foreach (var q in qots)
                    {
                        var rowp0 = ws2.CreateRow(ii);
                        var RfqNum = q.GetType().GetProperties()[0].GetValue(q);
                        var Matnr = q.GetType().GetProperties()[1].GetValue(q);
                        var Description = q.GetType().GetProperties()[2].GetValue(q);
                        rowp0.CreateCell(0).SetCellValue((string)RfqNum);
                        rowp0.CreateCell(1).SetCellValue((string)Matnr);
                        rowp0.CreateCell(2).SetCellValue((string)Description);

                        rowp0.GetCell(0).CellStyle = s1;// 20211222 add style  
                        rowp0.GetCell(1).CellStyle = s1;// 20211222 add style  
                        rowp0.GetCell(2).CellStyle = s1;// 20211222 add style  

                        var pEmptyFlag = q.GetType().GetProperties()[7].GetValue(q);
                        if (pEmptyFlag != null) 
                        {
                            if (pEmptyFlag.ToString() == "X")
                            {
                                rowp0.CreateCell(3).SetCellValue("Y");
                                rowp0.GetCell(3).CellStyle = s1;// 20211222 add style  
                            }
                        }                        
                        ii++;
                    }
                }
                else
                {
                    foreach (var process in processs)
                    {
                        var rowp = ws2.CreateRow(ip);

                        query.qotId = process.QotId;
                        var qotdata = _srmQotService.GetQotInfo(int.Parse(query.rfqId.ToString()), int.Parse(query.vendorId.ToString()), int.Parse(process.QotId.ToString()));
                        var qq = Queryable.Cast<object>(qotdata).FirstOrDefault();
                        RNo = qq.GetType().GetProperties()[0].GetValue(qq).ToString();
                        MatnrNo = qq.GetType().GetProperties()[1].GetValue(qq).ToString();
                        Desc = qq.GetType().GetProperties()[2].GetValue(qq).ToString();

                        rowp.CreateCell(0).SetCellValue(RNo);
                        rowp.CreateCell(1).SetCellValue(MatnrNo);
                        rowp.CreateCell(2).SetCellValue((Desc));

                        rowp.GetCell(0).CellStyle = s1;// 20211222 add style  
                        rowp.GetCell(1).CellStyle = s1;// 20211222 add style  
                        rowp.GetCell(2).CellStyle = s1;// 20211222 add style  

                        rowp.CreateCell(3).SetCellValue("");//不回填??
                        if (process.PProcessNum !=null) 
                        {
                            string processname = _srmQotService.GetProcessByNum(int.Parse(process.PProcessNum));
                            rowp.CreateCell(4).SetCellValue(processname);
                            rowp.GetCell(4).CellStyle = s1;// 20211222 add style  
                        }
                        if (process.PHours != null)
                        {
                            rowp.CreateCell(5).SetCellValue(process.PHours.Value);
                            rowp.GetCell(5).CellStyle = s1;// 20211222 add style  
                        }
                        if (process.PPrice != null)
                        {
                            rowp.CreateCell(6).SetCellValue(double.Parse(process.PPrice.ToString()));
                            rowp.GetCell(6).CellStyle = s1;// 20211222 add style  
                        }
                        if (process.PCostsum != null)
                        {
                            rowp.CreateCell(7).SetCellValue(double.Parse(process.PCostsum.ToString()));
                            rowp.GetCell(7).CellStyle = s1;// 20211222 add style  
                        }
                        if (process.PMachine != null)
                        {
                            rowp.CreateCell(8).SetCellValue(process.PMachine);
                            rowp.GetCell(8).CellStyle = s1;// 20211222 add style 
                        }
                        if (process.PNote != null)
                        {
                            rowp.CreateCell(9).SetCellValue(process.PNote);
                            rowp.GetCell(9).CellStyle = s1;// 20211222 add style 
                        }
                        ip++;
                    }
                    int cp = processs.Count() + 1;
                    foreach (var q in qots)
                    {
                        var rowp = ws2.CreateRow(cp);
                        var RfqNum = q.GetType().GetProperties()[0].GetValue(q);
                        var Matnr = q.GetType().GetProperties()[1].GetValue(q);
                        var Description = q.GetType().GetProperties()[2].GetValue(q);
                        var pEmptyFlag = q.GetType().GetProperties()[7].GetValue(q);
                        if (pEmptyFlag != null) 
                        {
                            if (pEmptyFlag.ToString() == "X")
                            {
                                rowp.CreateCell(0).SetCellValue((string)RfqNum);
                                rowp.CreateCell(1).SetCellValue((string)Matnr);
                                rowp.CreateCell(2).SetCellValue((string)Description);
                                rowp.CreateCell(3).SetCellValue("Y");

                                rowp.GetCell(0).CellStyle = s1;// 20211222 add style 
                                rowp.GetCell(1).CellStyle = s1;// 20211222 add style 
                                rowp.GetCell(2).CellStyle = s1;// 20211222 add style 
                                rowp.GetCell(3).CellStyle = s1;// 20211222 add style 

                                cp++;
                            }
                        }
                    }
                }
                ws2.GetRow(0).GetCell(0).CellStyle = s;
                ws2.GetRow(0).GetCell(1).CellStyle = s;
                ws2.GetRow(0).GetCell(2).CellStyle = s;
                ws2.GetRow(0).GetCell(4).CellStyle = s;
                ws2.GetRow(0).GetCell(5).CellStyle = s;
                ws2.GetRow(0).GetCell(6).CellStyle = s;
                #endregion

                #region 表面處理

                InitExcelColumn("表面處理");
                ws3.CreateRow(0);
                for (int i3 = 0; i3 < excelColumn.Count; i3++)
                {
                    KeyValuePair<string, string> column = excelColumn.ElementAt(i3);
                    ws3.GetRow(0).CreateCell(i3).SetCellValue(column.Value);
                    ws3.GetRow(0).GetCell(i3).CellStyle = s1;// 20211222 add style  
                }
                int iss = 1;
                SrmQotSurface[] surfaces = qotdetail.surface;
                if (surfaces.Count() == 0)
                {
                    ii = 1;
                    foreach (var q in qots)
                    {
                        var rows0 = ws3.CreateRow(ii);
                        var RfqNum = q.GetType().GetProperties()[0].GetValue(q);
                        var Matnr = q.GetType().GetProperties()[1].GetValue(q);
                        var Description = q.GetType().GetProperties()[2].GetValue(q);
                        rows0.CreateCell(0).SetCellValue((string)RfqNum);
                        rows0.CreateCell(1).SetCellValue((string)Matnr);
                        rows0.CreateCell(2).SetCellValue((string)Description);

                        rows0.GetCell(0).CellStyle = s1;// 20211222 add style 
                        rows0.GetCell(1).CellStyle = s1;// 20211222 add style 
                        rows0.GetCell(2).CellStyle = s1;// 20211222 add style 
                        var sEmptyFlag = q.GetType().GetProperties()[8].GetValue(q);
                        if (sEmptyFlag != null)
                        {
                            if (sEmptyFlag.ToString() == "X")
                            {
                                rows0.CreateCell(3).SetCellValue("Y");
                                rows0.GetCell(3).CellStyle = s1;// 20211222 add style 
                            }
                        }
                        ii++;
                    }
                }
                else
                {  
                    foreach (var surface in surfaces)
                    {
                        var rows = ws3.CreateRow(iss);
                        query.qotId = surface.QotId;
                        var qotdata = _srmQotService.GetQotInfo(int.Parse(query.rfqId.ToString()), int.Parse(query.vendorId.ToString()), int.Parse(surface.QotId.ToString()));
                        var qq = Queryable.Cast<object>(qotdata).FirstOrDefault();
                        RNo = qq.GetType().GetProperties()[0].GetValue(qq).ToString();
                        MatnrNo = qq.GetType().GetProperties()[1].GetValue(qq).ToString();
                        Desc = qq.GetType().GetProperties()[2].GetValue(qq).ToString();
                        rows.CreateCell(0).SetCellValue(RNo);
                        rows.CreateCell(1).SetCellValue(MatnrNo);
                        rows.CreateCell(2).SetCellValue((Desc));
                        rows.CreateCell(3).SetCellValue("");//不回填??

                        rows.GetCell(0).CellStyle = s1;// 20211222 add style 
                        rows.GetCell(1).CellStyle = s1;// 20211222 add style 
                        rows.GetCell(2).CellStyle = s1;// 20211222 add style 

                        if (surface.SProcess != null) 
                        {
                            string sufacename = _srmQotService.GetSurfaceProcessByNum(int.Parse(surface.SProcess));
                            rows.CreateCell(4).SetCellValue(sufacename); //轉換成加工中文??
                            rows.GetCell(4).CellStyle = s1;// 20211222 add style 
                        }
                        if (surface.SPrice != null)
                        {
                            rows.CreateCell(5).SetCellValue(double.Parse(surface.SPrice.ToString()));
                            rows.GetCell(5).CellStyle = s1;// 20211222 add style 
                        }
                        if (surface.STimes != null)
                        {
                            rows.CreateCell(6).SetCellValue(double.Parse(surface.STimes.ToString()));
                            rows.GetCell(6).CellStyle = s1;// 20211222 add style 
                        }
                        if (surface.SCostsum != null)
                        {
                            rows.CreateCell(7).SetCellValue(double.Parse(surface.SCostsum.ToString()));
                            rows.GetCell(7).CellStyle = s1;// 20211222 add style 
                        }
                        if (surface.SNote != null)
                        {
                            rows.CreateCell(8).SetCellValue(surface.SNote);
                            rows.GetCell(8).CellStyle = s1;// 20211222 add style 
                        }
                        iss++;
                    }
                    int cs = surfaces.Count() + 1;
                    foreach (var q in qots)
                    {
                        var rows = ws3.CreateRow(cs);
                        var RfqNum = q.GetType().GetProperties()[0].GetValue(q);
                        var Matnr = q.GetType().GetProperties()[1].GetValue(q);
                        var Description = q.GetType().GetProperties()[2].GetValue(q);
                        var sEmptyFlag = q.GetType().GetProperties()[8].GetValue(q);
                        if (sEmptyFlag != null)
                        {
                            if (sEmptyFlag.ToString() == "X")
                            {
                                rows.CreateCell(0).SetCellValue((string)RfqNum);
                                rows.CreateCell(1).SetCellValue((string)Matnr);
                                rows.CreateCell(2).SetCellValue((string)Description);
                                rows.CreateCell(3).SetCellValue("Y");

                                rows.GetCell(0).CellStyle = s1;// 20211222 add style 
                                rows.GetCell(1).CellStyle = s1;// 20211222 add style 
                                rows.GetCell(2).CellStyle = s1;// 20211222 add style 
                                rows.GetCell(3).CellStyle = s1;// 20211222 add style 
                                cs++;
                            }
                        } 
                    }
                }
                ws3.GetRow(0).GetCell(0).CellStyle = s;
                ws3.GetRow(0).GetCell(1).CellStyle = s;
                ws3.GetRow(0).GetCell(2).CellStyle = s;
                ws3.GetRow(0).GetCell(4).CellStyle = s;
                ws3.GetRow(0).GetCell(5).CellStyle = s;
                ws3.GetRow(0).GetCell(6).CellStyle = s;
                #endregion

                #region 其他

                InitExcelColumn("其他");
                ws4.CreateRow(0);
                for (int i4 = 0; i4 < excelColumn.Count; i4++)
                {
                    KeyValuePair<string, string> column = excelColumn.ElementAt(i4);
                    ws4.GetRow(0).CreateCell(i4).SetCellValue(column.Value);
                    ws4.GetRow(0).GetCell(i4).CellStyle = s1;// 20211222 add style  
                }
                int io = 1;
                SrmQotOther[] others = qotdetail.other;
                if (others.Count() == 0)
                {
                    ii = 1;
                    foreach (var q in qots)
                    {
                        var rowo0 = ws4.CreateRow(ii);
                        var RfqNum = q.GetType().GetProperties()[0].GetValue(q);
                        var Matnr = q.GetType().GetProperties()[1].GetValue(q);
                        var Description = q.GetType().GetProperties()[2].GetValue(q);
                        rowo0.CreateCell(0).SetCellValue((string)RfqNum);
                        rowo0.CreateCell(1).SetCellValue((string)Matnr);
                        rowo0.CreateCell(2).SetCellValue((string)Description);

                        rowo0.GetCell(0).CellStyle = s1;// 20211222 add style 
                        rowo0.GetCell(1).CellStyle = s1;// 20211222 add style 
                        rowo0.GetCell(2).CellStyle = s1;// 20211222 add style 

                        var oEmptyFlag = q.GetType().GetProperties()[9].GetValue(q);
                        if (oEmptyFlag != null)
                        {
                            if (oEmptyFlag.ToString() == "X")
                            {
                                rowo0.CreateCell(3).SetCellValue("Y");
                                rowo0.GetCell(3).CellStyle = s1;// 20211222 add style 
                            }
                        }
                        ii++;
                    }
                }
                else
                {
                    foreach (var other in others)
                    {
                        var rowo = ws4.CreateRow(io);
                        query.qotId = other.QotId;
                        var qotdata = _srmQotService.GetQotInfo(int.Parse(query.rfqId.ToString()), int.Parse(query.vendorId.ToString()), int.Parse(other.QotId.ToString()));
                        var qq = Queryable.Cast<object>(qotdata).FirstOrDefault();
                        RNo = qq.GetType().GetProperties()[0].GetValue(qq).ToString();
                        MatnrNo = qq.GetType().GetProperties()[1].GetValue(qq).ToString();
                        Desc = qq.GetType().GetProperties()[2].GetValue(qq).ToString();
                        rowo.CreateCell(0).SetCellValue(RNo);
                        rowo.CreateCell(1).SetCellValue(MatnrNo);
                        rowo.CreateCell(2).SetCellValue((Desc));

                        rowo.GetCell(0).CellStyle = s1;// 20211222 add style 
                        rowo.GetCell(1).CellStyle = s1;// 20211222 add style 
                        rowo.GetCell(2).CellStyle = s1;// 20211222 add style 

                        rowo.CreateCell(3).SetCellValue("");//不回填??
                        if (other.OItem !=null)
                        {
                            rowo.CreateCell(4).SetCellValue(other.OItem);
                            rowo.GetCell(4).CellStyle = s1;// 20211222 add style 
                        }                       
                        if (other.OPrice != null)
                        {
                            rowo.CreateCell(5).SetCellValue(double.Parse(other.OPrice.ToString()));
                            rowo.GetCell(5).CellStyle = s1;// 20211222 add style 
                        }
                        if (other.ODescription != null)
                        {
                            rowo.CreateCell(6).SetCellValue(other.ODescription);
                            rowo.GetCell(6).CellStyle = s1;// 20211222 add style 
                        }
                        if (other.ONote != null)
                        {
                            rowo.CreateCell(7).SetCellValue(other.ONote);
                            rowo.GetCell(7).CellStyle = s1;// 20211222 add style 
                        }
                        io++;
                    }
                    int co = others.Count() + 1;
                    foreach (var q in qots)
                    {
                        var rowo = ws4.CreateRow(co);
                        var RfqNum = q.GetType().GetProperties()[0].GetValue(q);
                        var Matnr = q.GetType().GetProperties()[1].GetValue(q);
                        var Description = q.GetType().GetProperties()[2].GetValue(q);
                        var oEmptyFlag = q.GetType().GetProperties()[9].GetValue(q);
                        if (oEmptyFlag != null)
                        {
                            if (oEmptyFlag.ToString() == "X")
                            {
                                rowo.CreateCell(0).SetCellValue((string)RfqNum);
                                rowo.CreateCell(1).SetCellValue((string)Matnr);
                                rowo.CreateCell(2).SetCellValue((string)Description);
                                rowo.CreateCell(3).SetCellValue("Y");

                                rowo.GetCell(0).CellStyle = s1;// 20211222 add style 
                                rowo.GetCell(1).CellStyle = s1;// 20211222 add style 
                                rowo.GetCell(2).CellStyle = s1;// 20211222 add style 
                                rowo.GetCell(3).CellStyle = s1;// 20211222 add style 
                                co++;
                            }
                        }
                    }
                }
                ws4.GetRow(0).GetCell(0).CellStyle = s;
                ws4.GetRow(0).GetCell(1).CellStyle = s;
                ws4.GetRow(0).GetCell(2).CellStyle = s;
                ws4.GetRow(0).GetCell(4).CellStyle = s;
                ws4.GetRow(0).GetCell(5).CellStyle = s;
                
                #endregion

                string filepath = $"D:\\Excel";
                string fileName = filepath + $"\\{RNo}批次報價.xls";
                FileStream file = new FileStream(fileName, FileMode.Create);//產生檔案
                wb.Write(file);
                file.Close();
                Console.WriteLine($@"匯出EXCEL成功，檔名為 --> {fileName}");
                var j = new JObject();
                j.Add("fileName", fileName);
                return Ok(j);

            }
            catch (Exception ex)
            {
                return this.BadRequestResult(ex.Message);
                Console.WriteLine("匯出EXCEL發生錯誤：");
                Console.WriteLine(ex.Message);
            }
        }
        
        private void InitExcelColumn(string tabname)
        {
            excelColumn.Clear();
            switch (tabname)
            {
                case "報價單表頭":
                    excelColumn.Add("RfqNum", "詢價單號");
                    excelColumn.Add("Matnr", "料號");
                    excelColumn.Add("Description", "短文");
                    excelColumn.Add("LeadTime", "[計畫交貨時間(日曆天)]");
                    excelColumn.Add("ExpirationDate", "有效期限");
                    excelColumn.Add("Note", "備註");
                    break;
                case "材料":
                    excelColumn.Add("RfqNum", "詢價單號");
                    excelColumn.Add("Matnr", "料號");
                    excelColumn.Add("Description", "短文");
                    excelColumn.Add("MEmptyFlag", "不回填");
                    excelColumn.Add("MMaterial", "素材材質");
                    excelColumn.Add("Length", "長");
                    excelColumn.Add("Width", "寬");
                    excelColumn.Add("Height", "高");
                    excelColumn.Add("Density", "密度");
                    excelColumn.Add("Weight", "重量");
                    excelColumn.Add("MPrice", "材料單價");
                    excelColumn.Add("MCostPrice", "材料小計");
                    excelColumn.Add("Note", "備註");
                    break;
                case "加工":
                    excelColumn.Add("RfqNum", "詢價單號");
                    excelColumn.Add("Matnr", "料號");
                    excelColumn.Add("Description", "短文");
                    excelColumn.Add("PEmptyFlag", "不回填");
                    excelColumn.Add("PProcessNum", "工序");
                    excelColumn.Add("PHours", "[工時(時)]");
                    excelColumn.Add("PPrice", "[單價(時)]");
                    excelColumn.Add("PCostsum", "加工小計");
                    excelColumn.Add("PMachine", "機台");
                    excelColumn.Add("PNote", "備註");
                    break;
                case "表面處理":
                    excelColumn.Add("RfqNum", "詢價單號");
                    excelColumn.Add("Matnr", "料號");
                    excelColumn.Add("Description", "短文");
                    excelColumn.Add("SEmptyFlag", "不回填");
                    excelColumn.Add("SProcess", "工序");
                    excelColumn.Add("SPrice", "[單價(時)]");
                    excelColumn.Add("STimes", "次數");
                    excelColumn.Add("SCostsum", "表面處理小計");
                    excelColumn.Add("PNote", "備註");
                    break;
                case "其他":
                    excelColumn.Add("RfqNum", "詢價單號");
                    excelColumn.Add("Matnr", "料號");
                    excelColumn.Add("Description", "短文");
                    excelColumn.Add("OEmptyFlag", "不回填");
                    excelColumn.Add("OItem", "項目");
                    excelColumn.Add("OPrice", "單價");
                    excelColumn.Add("ODescription", "說明");
                    excelColumn.Add("ONote", "備註");
                    break;
            }
        }
        [HttpPost("BatchUpload")]
        public IActionResult BatchUpload([FromForm] Model.Models.SRM.FileUploadViewModel_QOT fileUploadModel)
        {
            UserClaims user = User.GetUserClaims();
            fileUploadModel.CreateBy = user.UserName;
            fileUploadModel.CurrentDirectory = _appSettingsService.CurrentDirectory + fileUploadModel.CurrentDirectory;
            string path = "";
            string RfqNum = "";
            int qotstatus = 0;
            try
            {
                path = _srmQotService.Upload(fileUploadModel);
                DataTable data_qoth = _srmQotService.ReadExcel_QotH(path, user);
                if (data_qoth.Rows.Count == 0)
                {
                    throw new Exception("報價單表頭至少需一筆");
                }
                DataTable data_m = _srmQotService.ReadExcel_Material(path, user);
                if (data_m.Rows.Count == 0)
                {
                    throw new Exception("材料至少需一筆");
                }
                DataTable data_p = _srmQotService.ReadExcel_Process(path, user);
                if (data_p.Rows.Count == 0)
                {
                    throw new Exception("加工至少需一筆");
                }
                DataTable data_s = _srmQotService.ReadExcel_Surface(path, user);
                if (data_s.Rows.Count == 0)
                {
                    throw new Exception("表面處理至少需一筆");
                }
                DataTable data_o = _srmQotService.ReadExcel_Other(path, user);
                if (data_o.Rows.Count == 0)
                {
                    throw new Exception("其他至少需一筆");
                }
                string errTitle = "";
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    try
                    {
                        string rfqnum = string.Empty;
                        string matnr = string.Empty;
                        int qotid = 0; //fileUploadModel.QotId;// _srmQotService.CheckMatnrData(user.UserName, rfqnum, matnr);
                        QueryQot query = new QueryQot();
                        //query.qotId = qotid;
                        SrmQotH qq = new SrmQotH();

                        //qoth資料需用update
                        SrmQotH[] qots = JsonConvert.DeserializeObject<SrmQotH[]>(JsonConvert.SerializeObject(data_qoth));
                        for (int i = 0; i < data_qoth.Rows.Count; i++)
                        {
                            DataRow dr_qoth = data_qoth.Rows[i];

                            RfqNum = dr_qoth["RfqNum"].ToString();
                            matnr = dr_qoth["Matnr"].ToString();
                            query.rfqId = fileUploadModel.RfqId;
                            query.vendorId = fileUploadModel.VendorId;
                            query.matnr = matnr;
                            int matnrid = _srmQotService.GetMatnrId(query);
                            query.matnrId = matnrid;
                            qotid = _srmQotService.GetQotId(query);
                            query.qotId = qotid;
                            qq = _srmQotService.GetQot(query);
                            qq.LeadTime = double.Parse(dr_qoth["LeadTime"].ToString());
                            qq.ExpirationDate = DateTime.Parse(dr_qoth["ExpirationDate"].ToString());
                            qq.MEmptyFlag = "";
                            qq.PEmptyFlag = "";
                            qq.SEmptyFlag = "";
                            qq.OEmptyFlag = "";

                            /*檢核報價單狀態須為初始*/
                            qotstatus = _srmQotService.GetQotStatus(qq);
                            if (qotstatus !=1) 
                            {
                                break;
                            }
                            /**/
                            errTitle = $"詢價單:{dr_qoth["RfqNum"].ToString()}，";
                            if (!Convert.ToBoolean(dr_qoth["IsExists"].ToString())) //false
                            {
                                throw new Exception($"詢價單:{dr_qoth["RfqNum"].ToString()} 料號:{dr_qoth["Matnr"].ToString()}不存在，請再次確認");
                            }
                            else
                            {
                                for(int qi = 0; qi < data_qoth.Rows.Count; qi++)
                                {
                                    DataRow dr_q = data_qoth.Rows[qi];
                                    //errTitle = $"報價單表頭 : 報價單號:{dr_q["MMaterial"].ToString()}，";
                                   
                                }
                                /*20211215*/
                                //DataTable dtm = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(data_m));
                                //DataRow[] drsm = dtm.Select($"RfqNum='{rfqnum}'  and Matnr ='{matnr}' and  IsExists ='1'");
                                DataRow[] drms = data_m.Select(($"RfqNum='{RfqNum}' and  Matnr ='{matnr}' "));
                                DataTable data_m1 = data_m.Clone();
                                foreach (DataRow drm in drms)
                                {
                                    data_m1.ImportRow(drm);
                                }
                                SrmQotMaterial[] ms = JsonConvert.DeserializeObject<SrmQotMaterial[]>(JsonConvert.SerializeObject(data_m1));
                                for (int mi = 0; mi < data_m1.Rows.Count; mi++)
                                {
                                    qq.MEmptyFlag = "";
                                    DataRow dr_m = data_m1.Rows[mi];
                                    errTitle = $"A材料 : 素材材質:{dr_m["MMaterial"].ToString()}，";
                                    //if ((!string.IsNullOrWhiteSpace(dr_m["MEmptyFlag"].ToString())) && (dr_m["MEmptyFlag"].ToString().ToUpper() == "Y"))
                                    if(dr_m["MEmptyFlag"].ToString().ToUpper() == "Y") 
                                    {
                                        qq.MEmptyFlag = "X";
                                        ms = JsonConvert.DeserializeObject<SrmQotMaterial[]>(JsonConvert.SerializeObject(new DataTable()));
                                    }
                                 
                                }
                                DataRow[] drps = data_p.Select(($"RfqNum='{RfqNum}' and  Matnr ='{matnr}' "));
                                DataTable data_p1 = data_p.Clone();
                                foreach (DataRow drp in drps)
                                {
                                    data_p1.ImportRow(drp);
                                }

                                SrmQotProcess[] ps = JsonConvert.DeserializeObject<SrmQotProcess[]>(JsonConvert.SerializeObject(data_p1));
                                for (int pi = 0; pi < data_p1.Rows.Count; pi++)
                                {
                                    qq.PEmptyFlag = "";
                                    DataRow dr_p = data_p1.Rows[pi];
                                    errTitle = $"B加工 : 工序:{dr_p["PProcess"].ToString()}，";
                                    //if ((!string.IsNullOrWhiteSpace(dr_p["PEmptyFlag"].ToString())) && (dr_p["PEmptyFlag"].ToString().ToUpper() == "Y"))
                                    if(dr_p["PEmptyFlag"].ToString().ToUpper() == "Y")
                                    {
                                        qq.PEmptyFlag = "X";
                                        ps = JsonConvert.DeserializeObject<SrmQotProcess[]>(JsonConvert.SerializeObject(new DataTable()));
                                    }
                                }
                                //SProcessDesc
                                DataRow[] drss = data_s.Select(($"RfqNum='{RfqNum}' and  Matnr ='{matnr}' "));
                                DataTable data_s1 = data_s.Clone();
                                foreach (DataRow drs in drss)
                                {
                                    data_s1.ImportRow(drs);
                                }
                                SrmQotSurface[] ss = JsonConvert.DeserializeObject<SrmQotSurface[]>(JsonConvert.SerializeObject(data_s1));
                                for (int si = 0; si < data_s1.Rows.Count; si++)
                                {
                                    qq.SEmptyFlag = "";
                                    DataRow dr_s = data_s1.Rows[si];
                                    errTitle = $"C表面處理 : 工序:{dr_s["SProcessDesc"].ToString()}，";
                                    //if ((!string.IsNullOrWhiteSpace(dr_s["SEmptyFlag"].ToString())) && (dr_s["SEmptyFlag"].ToString().ToUpper() == "Y"))
                                    if(dr_s["SEmptyFlag"].ToString().ToUpper() == "Y")
                                    {
                                        qq.SEmptyFlag = "X";
                                        ss = JsonConvert.DeserializeObject<SrmQotSurface[]>(JsonConvert.SerializeObject(new DataTable()));
                                    }                           
                                }
                                DataRow[] dros = data_o.Select(($"RfqNum='{RfqNum}' and  Matnr ='{matnr}' "));
                                DataTable data_o1 = data_o.Clone();
                                foreach (DataRow dro in dros)
                                {
                                    data_o1.ImportRow(dro);
                                }
                                SrmQotOther[] os = JsonConvert.DeserializeObject<SrmQotOther[]>(JsonConvert.SerializeObject(data_o1));
                                for (int oi = 0; oi < data_o1.Rows.Count; oi++)
                                {
                                    qq.OEmptyFlag = "";
                                    DataRow dr_o = data_o1.Rows[oi];
                                    errTitle = $"D其他 : 項目:{dr_o["OItem"].ToString()}，";
                                    //if ((!string.IsNullOrWhiteSpace(dr_o["OEmptyFlag"].ToString())) && (dr_o["OEmptyFlag"].ToString().ToUpper() == "Y"))
                                    if(dr_o["OEmptyFlag"].ToString().ToUpper() == "Y")
                                    {
                                        qq.OEmptyFlag = "X";
                                        os = JsonConvert.DeserializeObject<SrmQotOther[]>(JsonConvert.SerializeObject(new DataTable()));
                                    }                                    
                                }

                                //_srmQotService.Save(qq, ms, ss, ps, os);
                                /*20211215*/
                            }
                        }
                        #region for save
                        qots = JsonConvert.DeserializeObject<SrmQotH[]>(JsonConvert.SerializeObject(data_qoth));
                        for (int i = 0; i < data_qoth.Rows.Count; i++)
                        {
                            DataRow dr_qoth = data_qoth.Rows[i];

                            RfqNum = dr_qoth["RfqNum"].ToString();
                            matnr = dr_qoth["Matnr"].ToString();
                            query.rfqId = fileUploadModel.RfqId;
                            query.vendorId = fileUploadModel.VendorId;
                            query.matnr = matnr;
                            int matnrid = _srmQotService.GetMatnrId(query);
                            query.matnrId = matnrid;
                            qotid = _srmQotService.GetQotId(query);
                            query.qotId = qotid;
                            qq = _srmQotService.GetQot(query);
                            qq.LeadTime = double.Parse(dr_qoth["LeadTime"].ToString());
                            qq.ExpirationDate = DateTime.Parse(dr_qoth["ExpirationDate"].ToString());
                            qq.MEmptyFlag = "";
                            qq.PEmptyFlag = "";
                            qq.SEmptyFlag = "";
                            qq.OEmptyFlag = "";

                            /*檢核報價單狀態須為初始*/
                            qotstatus = _srmQotService.GetQotStatus(qq);
                            if (qotstatus != 1)
                            {
                                break;
                            }
                            /**/
                            errTitle = $"詢價單:{dr_qoth["RfqNum"].ToString()}，";
                            if (!Convert.ToBoolean(dr_qoth["IsExists"].ToString())) //false
                            {
                                throw new Exception($"詢價單:{dr_qoth["RfqNum"].ToString()} 料號:{dr_qoth["Matnr"].ToString()}不存在，請再次確認");
                            }
                            else
                            {
                                for (int qi = 0; qi < data_qoth.Rows.Count; qi++)
                                {
                                    DataRow dr_q = data_qoth.Rows[qi];
                                    //errTitle = $"報價單表頭 : 報價單號:{dr_q["MMaterial"].ToString()}，";

                                }
                                /*20211215*/
                                //DataTable dtm = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(data_m));
                                //DataRow[] drsm = dtm.Select($"RfqNum='{rfqnum}'  and Matnr ='{matnr}' and  IsExists ='1'");
                                DataRow[] drms = data_m.Select(($"RfqNum='{RfqNum}' and  Matnr ='{matnr}' "));
                                DataTable data_m1 = data_m.Clone();
                                foreach (DataRow drm in drms)
                                {
                                    data_m1.ImportRow(drm);
                                }
                                SrmQotMaterial[] ms = JsonConvert.DeserializeObject<SrmQotMaterial[]>(JsonConvert.SerializeObject(data_m1));
                                for (int mi = 0; mi < data_m1.Rows.Count; mi++)
                                {
                                    qq.MEmptyFlag = "";
                                    DataRow dr_m = data_m1.Rows[mi];
                                    errTitle = $"A材料 : 素材材質:{dr_m["MMaterial"].ToString()}，";
                                    //if ((!string.IsNullOrWhiteSpace(dr_m["MEmptyFlag"].ToString())) && (dr_m["MEmptyFlag"].ToString().ToUpper() == "Y"))
                                    if (dr_m["MEmptyFlag"].ToString().ToUpper() == "Y")
                                    {
                                        qq.MEmptyFlag = "X";
                                        ms = JsonConvert.DeserializeObject<SrmQotMaterial[]>(JsonConvert.SerializeObject(new DataTable()));
                                    }

                                }
                                DataRow[] drps = data_p.Select(($"RfqNum='{RfqNum}' and  Matnr ='{matnr}' "));
                                DataTable data_p1 = data_p.Clone();
                                foreach (DataRow drp in drps)
                                {
                                    data_p1.ImportRow(drp);
                                }

                                SrmQotProcess[] ps = JsonConvert.DeserializeObject<SrmQotProcess[]>(JsonConvert.SerializeObject(data_p1));
                                for (int pi = 0; pi < data_p1.Rows.Count; pi++)
                                {
                                    qq.PEmptyFlag = "";
                                    DataRow dr_p = data_p1.Rows[pi];
                                    errTitle = $"B加工 : 工序:{dr_p["PProcess"].ToString()}，";
                                    //if ((!string.IsNullOrWhiteSpace(dr_p["PEmptyFlag"].ToString())) && (dr_p["PEmptyFlag"].ToString().ToUpper() == "Y"))
                                    if (dr_p["PEmptyFlag"].ToString().ToUpper() == "Y")
                                    {
                                        qq.PEmptyFlag = "X";
                                        ps = JsonConvert.DeserializeObject<SrmQotProcess[]>(JsonConvert.SerializeObject(new DataTable()));
                                    }
                                }
                                //SProcessDesc
                                DataRow[] drss = data_s.Select(($"RfqNum='{RfqNum}' and  Matnr ='{matnr}' "));
                                DataTable data_s1 = data_s.Clone();
                                foreach (DataRow drs in drss)
                                {
                                    data_s1.ImportRow(drs);
                                }
                                SrmQotSurface[] ss = JsonConvert.DeserializeObject<SrmQotSurface[]>(JsonConvert.SerializeObject(data_s1));
                                for (int si = 0; si < data_s1.Rows.Count; si++)
                                {
                                    qq.SEmptyFlag = "";
                                    DataRow dr_s = data_s1.Rows[si];
                                    errTitle = $"C表面處理 : 工序:{dr_s["SProcessDesc"].ToString()}，";
                                    //if ((!string.IsNullOrWhiteSpace(dr_s["SEmptyFlag"].ToString())) && (dr_s["SEmptyFlag"].ToString().ToUpper() == "Y"))
                                    if (dr_s["SEmptyFlag"].ToString().ToUpper() == "Y")
                                    {
                                        qq.SEmptyFlag = "X";
                                        ss = JsonConvert.DeserializeObject<SrmQotSurface[]>(JsonConvert.SerializeObject(new DataTable()));
                                    }
                                }
                                DataRow[] dros = data_o.Select(($"RfqNum='{RfqNum}' and  Matnr ='{matnr}' "));
                                DataTable data_o1 = data_o.Clone();
                                foreach (DataRow dro in dros)
                                {
                                    data_o1.ImportRow(dro);
                                }
                                SrmQotOther[] os = JsonConvert.DeserializeObject<SrmQotOther[]>(JsonConvert.SerializeObject(data_o1));
                                for (int oi = 0; oi < data_o1.Rows.Count; oi++)
                                {
                                    qq.OEmptyFlag = "";
                                    DataRow dr_o = data_o1.Rows[oi];
                                    errTitle = $"D其他 : 項目:{dr_o["OItem"].ToString()}，";
                                    //if ((!string.IsNullOrWhiteSpace(dr_o["OEmptyFlag"].ToString())) && (dr_o["OEmptyFlag"].ToString().ToUpper() == "Y"))
                                    if (dr_o["OEmptyFlag"].ToString().ToUpper() == "Y")
                                    {
                                        qq.OEmptyFlag = "X";
                                        os = JsonConvert.DeserializeObject<SrmQotOther[]>(JsonConvert.SerializeObject(new DataTable()));
                                    }
                                }

                                _srmQotService.Save(qq, ms, ss, ps, os);
                                /*20211215*/
                            }
                        }
                        #endregion

                        errTitle = "";


                        transaction.Complete();

                    }
                    catch (Exception ex)
                    {
                        transaction.Dispose();
                        throw new Exception(errTitle + ex.Message);
                    }
                    finally
                    {
                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            _srmQotService.Delete(path);
                        }
                    }
                }
                var j = new JObject();
                j.Add("RfqNum", RfqNum);
                return Ok(j);
            }
            catch (Exception ex)
            {
                return this.BadRequestResult(ex.Message);
            }
        }

    }
}
