using AutoMapper;

using Convience.Entity.Data;
using Convience.Entity.Entity;
using Convience.Entity.Entity.Identity;
using Convience.Entity.Entity.SRM;
using Convience.EntityFrameWork.Repositories;
using Convience.JwtAuthentication;
using Convience.Model.Constants.SystemManage;
using Convience.Model.Models;
using Convience.Model.Models.SystemManage;
using Convience.Util.Extension;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Convience.Model.Models.SRM;
using Newtonsoft.Json;
using Convience.Service.SystemManage;
using System.IO;
using Convience.Filestorage.Abstraction;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;

namespace Convience.Service.SRM
{
    public interface ISrmRfqHService
    {
        public void Save(SrmRfqH rfqH);
        public string Save(SrmRfqH rfqH, SrmRfqM[] rfqMs, SrmRfqV[] rfqVs);
        public ViewSrmRfqH GetDataByRfqId(int RfqId);
        public void End(QueryRfqList q);
        public PagingResultModel<ViewSrmRfqH> GetRfqList(QueryRfqList q, int page, int size);
        public ViewSrmRfqH[] GetRfqList(QueryRfqList q);
        public SrmRfqH UpdateStatus(int status, SrmRfqH rfqH);
        public PagingResultModel<AspNetUser> GetSourcer(string name, int[] werks, int size, int page);
        public SrmRfqH GetRfq(QueryRfq query);
        public void AsyncSourcer();
        public string Upload(Model.Models.SRM.FileUploadViewModel_RFQ fileUploadModel);
        public DataTable ReadExcel_Matnr(string path, UserClaims user);
        public DataTable ReadExcel_Vendor(string path, UserClaims user);
        public void Delete(string path);
    }
    public class SrmRfqHService : ISrmRfqHService
    {
        private readonly IRepository<SrmRfqH> _srmRfqHRepository;
        private readonly SRMContext _context;
        public SrmRfqHService(
            //IMapper mapper,
            IRepository<SrmRfqH> srmRfqHRepository, SRMContext dbContext)
        //SystemIdentityDbUnitOfWork systemIdentityDbUnitOfWork)
        {
            //_mapper = mapper;
            _srmRfqHRepository = srmRfqHRepository;
            _context = dbContext;
            //_systemIdentityDbUnitOfWork = systemIdentityDbUnitOfWork;
        }
        public void Save(SrmRfqH rfqH)
        {
            //using (var db = new SRMContext())
            //{
            if (rfqH.RfqId == 0)
            {
                _context.SrmRfqHs.Add(rfqH);
            }
            else
            {
                _context.SrmRfqHs.Update(rfqH);
            }
            _context.SaveChanges();
            //}
        }

        public string Save(SrmRfqH rfqH, SrmRfqM[] rfqMs, SrmRfqV[] rfqVs)
        {
            //using (var db = new SRMContext())
            //{
            //db.Database.BeginTransaction();
            //try
            //{
            DateTime now = DateTime.Now;
            rfqH.LastUpdateDate = now;
            if (rfqH.RfqId == 0)
            {
                rfqH.CreateDate = now;
                rfqH.CreateBy = rfqH.LastUpdateBy;
                _context.SrmRfqHs.Add(rfqH);
                _context.SaveChanges();
                //rfqH.RfqNum = "V" + rfqH.RfqId.ToString().PadLeft(6,'0');
                foreach (var rfqM in rfqMs)
                {
                    if (rfqM.Qty.HasValue && rfqM.Qty.Value <= 0)
                    {
                        throw new Exception($"料號{rfqM.Material}，數量應大於0");
                    }
                    if (rfqM.EstDeliveryDate.HasValue) {
                        rfqM.EstDeliveryDate = rfqM.EstDeliveryDate.Value.Date;
                    }
                    rfqM.RfqId = rfqH.RfqId;
                }
                foreach (var rfqV in rfqVs)
                {
                    rfqV.RfqId = rfqH.RfqId;
                }
                _context.SrmRfqHs.Update(rfqH);
                _context.SrmRfqMs.AddRange(rfqMs);
                _context.SrmRfqVs.AddRange(rfqVs);
            }
            else
            {
                //db.SrmRfqHs.Any(r => r.RfqId == rfqH.RfqId && r.Status == (int)Status.初始)
                if (!_context.SrmRfqHs.Any(r => r.RfqId == rfqH.RfqId && r.Status == (int)Status.初始))
                {
                    throw new Exception("非初始無法修改");
                }
                _context.SrmRfqHs.Update(rfqH);
                foreach (var rfqM in rfqMs)
                {
                    if (rfqM.Qty.HasValue && rfqM.Qty.Value <= 0)
                    {
                        throw new Exception($"料號{rfqM.Material}，數量應大於0");
                    }
                    if (rfqM.EstDeliveryDate.HasValue)
                    {
                        rfqM.EstDeliveryDate = rfqM.EstDeliveryDate.Value.Date;
                    }
                    rfqM.RfqId = rfqH.RfqId;
                    if (rfqM.RfqMId == 0)
                    {
                        _context.SrmRfqMs.Add(rfqM);
                    }
                    else
                    {
                        _context.SrmRfqMs.Update(rfqM);
                    }
                }
                var oldRfqMs = _context.SrmRfqMs.Where(r => r.RfqId == rfqH.RfqId);
                foreach (var oldrfqM in oldRfqMs)
                {
                    if (rfqMs.AsEnumerable().Where(item => item.RfqMId == oldrfqM.RfqMId).Count() == 0)
                    {
                        _context.SrmRfqMs.Remove(oldrfqM);
                    }
                }
                foreach (var rfqV in rfqVs)
                {
                    rfqV.RfqId = rfqH.RfqId;
                    if (rfqV.RfqVId == 0)
                    {
                        _context.SrmRfqVs.Add(rfqV);
                    }
                    else
                    {
                        _context.SrmRfqVs.Update(rfqV);
                    }
                }
                var oldRfqVs = _context.SrmRfqVs.Where(r => r.RfqId == rfqH.RfqId);
                foreach (var oldrfqV in oldRfqVs)
                {
                    if (rfqVs.AsEnumerable().Where(item => item.RfqVId == oldrfqV.RfqVId).Count() == 0)
                    {
                        _context.SrmRfqVs.Remove(oldrfqV);
                    }
                }
            }
            _context.SaveChanges();
            return rfqH.RfqNum;
            //    db.Database.CommitTransaction();
            //}
            //catch (Exception ex)
            //{
            //    db.Database.RollbackTransaction();
            //    throw;
            //}
            //}
        }

        public void End(QueryRfqList q) {
            var rfqHs = GetRfqList(q);
            DateTime now = DateTime.Now;
            foreach (ViewSrmRfqH rfq in rfqHs) {
                var isStarted = _context.SrmQotHs.Any(r => r.Status == (int)Status.確認 && r.RfqId == rfq.RfqId);
                rfq.Status = isStarted ? (int)Status.確認 : (int)Status.失效;
                rfq.LastUpdateBy = "MIS";
                rfq.LastUpdateDate = now;
                _context.Entry(rfq).Property(x => x.Status).IsModified = true;
                _context.Entry(rfq).Property(x => x.LastUpdateBy).IsModified = true;
                _context.Entry(rfq).Property(x => x.LastUpdateDate).IsModified = true;

                var Qots = _context.SrmQotHs.Where(r => r.Status == (int)Status.初始 && r.RfqId == rfq.RfqId);
                foreach (var qot in Qots) {
                    qot.Status = (int)Status.失效;
                    qot.LastUpdateBy = "MIS";
                    qot.LastUpdateDate = now;
                    _context.Entry(qot).Property(x => x.Status).IsModified = true;
                    _context.Entry(qot).Property(x => x.LastUpdateBy).IsModified = true;
                    _context.Entry(qot).Property(x => x.LastUpdateDate).IsModified = true;
                }
                _context.SaveChanges();
            }
        }


        public ViewSrmRfqH[] GetRfqList(QueryRfqList q)
        {
            var rfqQuery = _srmRfqHRepository.Get().AndIfHaveValue(q.rfqNum, r => r.RfqNum.Contains(q.rfqNum))
        .AndIfCondition(q.status != 0, r => r.Status.Equals(q.status))
        .Where(r => r.Status != (int)Status.刪除);
            if (!string.IsNullOrWhiteSpace(q.matnr)) {
                rfqQuery = from rfq in rfqQuery
                           join rfqm in _context.SrmRfqMs on rfq.RfqId equals rfqm.RfqId
                           join m in _context.SrmMatnrs on rfqm.MatnrId equals m.MatnrId
                           where m.SrmMatnr1.Contains(q.matnr)
                           select rfq;
            }

            var rfqs = from rfq in rfqQuery
                           //join e in _context.SrmEkgries on rfq.CreateBy equals e.Empid
                       join u in _context.AspNetUsers on rfq.CreateBy equals u.UserName
                       join s in _context.AspNetUsers on rfq.Sourcer equals s.UserName
                       into gj
                       from x in gj.DefaultIfEmpty()
                       where q.werks.Contains(rfq.Werks.Value)
                       select new ViewSrmRfqH
                       {
                           RfqId = rfq.RfqId,
                           Status = rfq.Status,
                           RfqNum = rfq.RfqNum,
                           Sourcer = rfq.Sourcer,
                           sourcerName = x.Name,
                           CreateBy = rfq.CreateBy,
                           CreateDate = rfq.CreateDate,
                           C_by = u.Name,
                           Deadline = rfq.Deadline,
                           Werks = rfq.Werks
                       };
            rfqs = rfqs.AndIfHaveValue(q.name, r => r.C_by.Contains(q.name) || r.CreateBy.Contains(q.name))
                .AndIfCondition(q.end, r => r.Status == (int)Status.啟動 && r.Deadline.Value.AddDays(1) <= DateTime.Now.Date)
                .Distinct();
            if (q.orderDesc)
            {
                rfqs = rfqs.OrderByDescending(r => r.RfqId);
            }
            return rfqs.ToArray();
        }

        public PagingResultModel<ViewSrmRfqH> GetRfqList(QueryRfqList q, int page, int size)
        {
            int skip = (page - 1) * size;

            //var rfqQuery = _srmRfqHRepository.Get().AndIfHaveValue(q.rfqNum, r => r.RfqNum.Contains(q.rfqNum))
            //    .AndIfHaveValue(q.status, r => r.Status.Equals(q.status));

            //rfqQuery = rfqQuery.Where(r => r.Status != (int)Status.刪除);

            //var rfqs = from rfq in rfqQuery
            //           join e in _context.SrmEkgries on rfq.CreateBy equals e.Empid
            //           join u in _context.AspNetUsers on rfq.CreateBy equals u.UserName
            //           join s in _context.AspNetUsers on rfq.Sourcer equals s.UserName
            //           into gj
            //           from x in gj.DefaultIfEmpty()
            //           where q.werks.Contains(e.Werks)
            //           select new
            //           {
            //               id = rfq.RfqId,
            //               status = rfq.Status,
            //               rfqNum = rfq.RfqNum,
            //               sourcer = rfq.Sourcer,
            //               sourcerName = x.Name,
            //               createBy = rfq.CreateBy,
            //               createDate = rfq.CreateDate,
            //               c_by = u.Name,
            //               c_Date = rfq.CreateDate.Value.ToString("yyyy-MM-dd"),
            //               viewstatus = ((Status)rfq.Status).ToString(),
            //           };
            //var result = rfqs.AndIfHaveValue(q.name, r => r.c_by.Contains(q.name)).Distinct().ToArray();

            var result = GetRfqList(q);
            var r = result.AsQueryable().Skip(skip).Take(size).ToArray();//result.Skip(skip).Take(size);
            return new PagingResultModel<ViewSrmRfqH>
            {
                Data = r,
                Count = result.Count()
            };
            //JObject obj = new JObject() {
            //    { "data",JArray.FromObject(r)},
            //    { "total",result.Count()}
            //};
            //return obj;
        }

        public ViewSrmRfqH GetDataByRfqId(int RfqId)
        {
            var rfq = (from rfqH in _context.SrmRfqHs
                       join sourcer in _context.AspNetUsers on rfqH.Sourcer equals sourcer.UserName
                       join create in _context.AspNetUsers on rfqH.CreateBy equals create.UserName
                       join ekgry in _context.SrmEkgries on rfqH.Sourcer equals ekgry.Empid into egrouping
                       from ekgry in egrouping.DefaultIfEmpty()
                       where rfqH.RfqId.Equals(RfqId)
                       select new ViewSrmRfqH
                       {
                           CreateBy = rfqH.CreateBy,
                           CreateDate = rfqH.CreateDate,
                           C_by = create.Name,
                           Deadline = rfqH.Deadline,
                           EndDate = rfqH.EndDate,
                           EndBy = rfqH.EndBy,
                           LastUpdateBy = rfqH.LastUpdateBy,
                           LastUpdateDate = rfqH.LastUpdateDate,
                           RfqId = rfqH.RfqId,
                           RfqNum = rfqH.RfqNum,
                           Sourcer = rfqH.Sourcer,
                           sourcerName = sourcer.Name,
                           Status = rfqH.Status,
                           ekgry = ekgry.Ekgry,
                           Werks = rfqH.Werks
                       }).First();


            //var rfq = _srmRfqHRepository.Get(r => r.RfqId == RfqId).DefaultIfEmpty().Join(_context.AspNetUsers,a=>a.Sourcer,b=>b.UserName,(a,b)=>new ViewSrmRfqH { 
            // CreateBy = a.CreateBy,
            // CreateDate = a.CreateDate,
            // C_by =     
            //    ,b.Name
            //}).First();

            //string temp = JsonConvert.SerializeObject(rfq);
            //ViewSrmRfqH result = JsonConvert.DeserializeObject<ViewSrmRfqH>(temp);
            return rfq;
        }

        public SrmRfqH UpdateStatus(int status, SrmRfqH rfqH)
        {
            var rfq = _srmRfqHRepository.Get(r => r.RfqId == rfqH.RfqId).First();
            switch ((Status)status)
            {
                case Status.啟動:
                    if ((Status)rfq.Status != Status.初始)
                    {
                        throw new Exception($"非初始狀態無法{((Status)status).ToString()}");
                    }
                    break;
                case Status.作廢:
                case Status.刪除:
                    if ((Status)rfq.Status != Status.初始 && (Status)rfq.Status != Status.啟動)
                    {
                        throw new Exception($"非初始或啟動狀態無法{((Status)status).ToString()}");
                    }
                    rfq.EndDate = DateTime.Now;
                    rfq.EndBy = rfqH.EndBy;
                    break;
                case Status.簽核中:
                    if ((Status)rfq.Status != Status.確認 && (Status)rfq.Status != Status.簽核中 && (Status)rfq.Status != Status.完成 && (Status)rfq.Status != Status.啟動)
                    {
                        throw new Exception($"狀態異常無法{((Status)status).ToString()}");
                    }
                    if ((Status)rfq.Status == Status.完成)
                    {
                        return rfq;
                    }
                    break;
                default:
                    throw new Exception($"未定義{((Status)status).ToString()}");
                    break;
            }
            rfq.Status = status;
            rfq.LastUpdateDate = rfqH.LastUpdateDate;
            rfq.LastUpdateBy = rfqH.LastUpdateBy;
            using (SRMContext db = new SRMContext())
            {
                db.Update(rfq);
                db.SaveChanges();
                return rfq;
            }
        }
        public PagingResultModel<AspNetUser> GetSourcer(string name, int[] werks, int size, int page)
        {
            //int[] werks = _dbContext.SrmEkgries.Where(r => r.Empid == logonid).Select(r => r.Werks).ToArray();

            //using (SRMContext db = new SRMContext())
            //{
            var resultQuery = (from s in _context.AspNetUsers
                               join sa in _context.SrmEkgries on s.UserName equals sa.Empid
                               where werks.Contains(sa.Werks)
                               //where s.UserName.Contains(name) && s.Name.Contains(name) && s.CostNo.StartsWith(werks)
                               //where s.UserName.Contains(name) && s.Name.Contains(name) && sa.Werks.StartsWith(werks)
                               select s).Distinct();
            var skip = size * (page - 1);
            var users = size == 0 ? resultQuery.ToArray() : resultQuery.Skip(skip).Take(size).ToArray();
            return new PagingResultModel<AspNetUser>
            {
                Data = users,
                Count = resultQuery.Count()
            };
            //}
        }
        public SrmRfqH GetRfq(QueryRfq query)
        {
            //using (SRMContext db = new SRMContext())
            //{
            return _context.SrmRfqHs.AsQueryable().AndIfHaveValue(query.rfqNum, r => r.RfqNum == query.rfqNum)
                 .AndIfHaveValue(query.status, r => r.Status == query.status)
                 .AndIfHaveValue(query.statuses, r => query.statuses.Contains(r.Status.Value))
                 .AndIfHaveValue(query.werks, r => query.werks.Contains(r.Werks.Value))
                 .AndIfHaveValue(query.rfqId, r => query.rfqId == r.RfqId)
                 .FirstOrDefault();
            //}
        }
        public void AsyncSourcer()
        {
            int roleid = _context.AspNetRoles.Where(r => r.Name == "詢價人員").Select(r => r.Id).First();
            string[] ekgries = _context.SrmEkgries.Select(r => r.Empid).ToArray();
            int[] userIds = (from e in ekgries
                             join u in _context.AspNetUsers on e equals u.UserName
                             select u.Id).ToArray();
            foreach (int userid in userIds)
            {
                if (!_context.AspNetUserRoles.Any(r => r.UserId == userid && r.RoleId == roleid))
                {
                    _context.AspNetUserRoles.Add(new AspNetUserRole() { UserId = userid, RoleId = roleid });
                }
            }
        }

        #region upload
        public string Upload(Model.Models.SRM.FileUploadViewModel_RFQ fileUploadModel) {
            Guid g = Guid.NewGuid();
            var file = fileUploadModel.Files.First();
            var path = fileUploadModel.CurrentDirectory?.TrimEnd('/') +'/' + fileUploadModel.CreateBy + '/' + g +'_' + file.FileName;
            switch (Path.GetExtension(file.FileName).ToLower())
            {
                case ".xlsx":
                    break;
                default:
                    throw new FileStoreException("限定xlsx！");
            }
            var info = new Utility.UploadFile().GetFileInfoAsync(path);
            if (info != null)
            {
                throw new FileStoreException("文件名重複！");
            }
            var stream = file.OpenReadStream();
            var result = new Utility.UploadFile().CreateFileFromStreamAsync(path, stream);
            if (string.IsNullOrEmpty(result))
            {
                throw new FileStoreException("文件上傳失敗！");
            }
            return path;
        }
        public DataTable ReadExcel_Matnr(string path, UserClaims user)
        {
            IWorkbook workbook;
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(stream);
            }

            ISheet sheet = workbook.GetSheetAt(0); // zero-based index of your target sheet
            DataTable dt = new DataTable(sheet.SheetName);

            // write header row
            IRow headerRow = sheet.GetRow(0);
            for (int i = 0; i < headerRow.Cells.Count; i++)
            {
                headerRow.GetCell(i).SetCellType(CellType.String);
                dt.Columns.Add(headerRow.GetCell(i).StringCellValue);
            }
            dt.Columns.Add("IsExists");
            dt.Columns.Add("Unit");
            string[] headers = new string[] { "料號", "物料內文", "物料群組", "工廠", "採購群組代碼", "版次", "材質規格", "長", "寬", "高(厚)", "圓外徑", "圓內徑", "密度", "重量", "重量單位", "評估案號", "備註", "數量","期望日期","計量單位" };
            string[] cols = new string[] { "SrmMatnr1", "Description", "MatnrGroup", "Werks", "Ekgrp", "Version", "Material", "Length", "Width", "Height", "Major_diameter", "Minor_diameter", "Density", "Weight", "Gewei", "Bn_num", "Note", "QTY", "EstDeliveryDate", "UnitDesc" };
            Dictionary<string, int> dtHeader = new Dictionary<string, int>();
            foreach (string header in headers) {
                if (!dt.Columns.Contains(header))
                {
                    throw new Exception($"格式錯誤，沒有欄位{header}");
                }
                else
                {
                    dtHeader.Add(header, dt.Columns.IndexOf(header));
                }
            }
            
            int rowIndex = 0;
            foreach (IRow row in sheet)
            {
                if (rowIndex ==0) { rowIndex++; continue; }
                if (row.GetCell(dtHeader["數量"]) != null)
                {
                    row.GetCell(dtHeader["數量"]).SetCellType(CellType.String);
                    if (string.IsNullOrWhiteSpace(row.GetCell(dtHeader["數量"]).StringCellValue)) { break; }
                }
                else {
                    break;
                }
                DataFormatter formatter = new DataFormatter();
                DataRow dataRow = dt.NewRow();
                foreach (var h in dtHeader) {
                    if (row.GetCell(h.Value) != null) {
                        row.GetCell(h.Value).SetCellType(CellType.String);
                        dataRow[h.Key] = row.GetCell(h.Value).StringCellValue.Replace(System.Environment.NewLine, "").Replace("\n", ""); ;
                    }
                }
                if (_context.SrmMatnrs.Any(r => r.SrmMatnr1.Equals(dataRow["料號"].ToString()) && r.Status != (int)(Status.失效) && user.Werks.Contains(r.Werks.Value)))//2021/10/19問過LEO 同名字不同廠可能存在多筆
                {
                    dataRow["IsExists"] = true;
                }
                else
                {
                    dataRow["IsExists"] = false;
                    if (_context.SrmMatnrs.Any(r => r.SrmMatnr1.Equals(dataRow["料號"].ToString()) && r.Status.Equals(Status.失效) && user.Werks.Contains(r.Werks.Value)))//2021/10/19問過LEO 同名字不同廠可能存在多筆
                    {
                        throw new Exception($"料號:{dataRow["料號"].ToString()}已失效");
                    }
                    if (string.IsNullOrWhiteSpace(dataRow["計量單位"].ToString()))
                    {
                        throw new Exception($"計量單位:{dataRow["料號"].ToString()}計量單位未填");
                    }
                    var measureUnit = _context.SrmMeasureUnits.Where(r => r.MeasureDesc.Equals(dataRow["計量單位"].ToString())).ToList();
                    if (measureUnit.Count() == 0)
                    {
                        throw new Exception($"計量單位:{dataRow["料號"].ToString()}計量單位不存在");
                    }
                    dataRow["Unit"] = measureUnit[0].MeasureId;
                }
                if (string.IsNullOrWhiteSpace(dataRow["數量"].ToString())) 
                {
                    throw new Exception($"料號:{dataRow["料號"].ToString()}數量未填");
                }
                //if (string.IsNullOrWhiteSpace(dataRow["期望日期"].ToString()))
                //{
                //    throw new Exception($"料號:{dataRow["料號"].ToString()}期望日期未填");
                //}
                double qty = 0;
                DateTime estDeliveryDate = new DateTime();
                if (!double.TryParse(dataRow["數量"].ToString(),out qty)||qty<=0)
                {
                    throw new Exception($"料號:{dataRow["料號"].ToString()}數量格式錯誤");
                }
                if (!string.IsNullOrWhiteSpace(dataRow["期望日期"].ToString()))
                {
                    if (!DateTime.TryParse(dataRow["期望日期"].ToString(), out estDeliveryDate))
                    {
                        double d = 0;
                        if (!double.TryParse(dataRow["期望日期"].ToString(), out d))
                        {
                            throw new Exception($"料號:{dataRow["料號"].ToString()}期望日期格式錯誤");
                        }
                        estDeliveryDate = DateTime.FromOADate(Convert.ToDouble(dataRow["期望日期"].ToString()));
                    }
                    dataRow["期望日期"] = estDeliveryDate.ToString("yyyy/MM/dd");
                }
                else {
                    dataRow["期望日期"] = null;
                }
                //dataRow["IsExists"] = _context.SrmMatnrs.Any(r => r.SrmMatnr1.Equals(dataRow["料號"].ToString()) && user.Werks.Contains(r.Werks.Value));//2021/10/19問過LEO 同名字不同廠可能存在多筆
                dt.Rows.Add(dataRow);
                rowIndex++;
            }
            for (int i = 0; i < headers.Count(); i++) {
                dt.Columns[headers[i]].ColumnName = cols[i];
            }
            return dt;
        }
        public DataTable ReadExcel_Vendor(string path, UserClaims user)
        {
            IWorkbook workbook;
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(stream);
            }

            ISheet sheet = workbook.GetSheetAt(1); // zero-based index of your target sheet
            DataTable dt = new DataTable(sheet.SheetName);

            // write header row
            IRow headerRow = sheet.GetRow(0);
            for (int i = 0; i < headerRow.Cells.Count; i++)
            {
                headerRow.GetCell(i).SetCellType(CellType.String);
                dt.Columns.Add(headerRow.GetCell(i).StringCellValue);
            }
            dt.Columns.Add("IsExists");
            string[] headers = new string[] { "供應商編號", "供應商名稱", "公司代碼", "採購組織", "聯絡人", "地址", "信箱", "傳真號碼", "電話號碼", "分機", "手機號碼" };
            string[] cols = new string[] { "SrmVendor1", "VendorName", "Org", "Ekorg", "Person", "Address", "Mail", "FaxNumber", "TelPhone", "Ext", "CellPhone" };
            Dictionary<string, int> dtHeader = new Dictionary<string, int>();
            foreach (string header in headers)
            {
                if (!dt.Columns.Contains(header))
                {
                    throw new Exception($"格式錯誤，沒有欄位{header}");
                }
                else
                {
                    dtHeader.Add(header, dt.Columns.IndexOf(header));
                }
            }

            int rowIndex = 0;
            foreach (IRow row in sheet)
            {
                if (rowIndex == 0) { rowIndex++; continue; }
                if (row.GetCell(dtHeader["供應商編號"]) != null)
                {
                    row.GetCell(dtHeader["供應商編號"]).SetCellType(CellType.String);
                    if (row.GetCell(dtHeader["供應商編號"]).StringCellValue.IndexOf("1.") == 0){
                        { break; }
                    }
                    row.GetCell(dtHeader["供應商名稱"]).SetCellType(CellType.String);
                    if (string.IsNullOrWhiteSpace(row.GetCell(dtHeader["供應商編號"]).StringCellValue) && string.IsNullOrWhiteSpace(row.GetCell(dtHeader["供應商名稱"]).StringCellValue))
                    { break; }
                }
                else
                {
                    break;
                }
                DataFormatter formatter = new DataFormatter();
                DataRow dataRow = dt.NewRow();
                foreach (var h in dtHeader)
                {
                    if (row.GetCell(h.Value) != null)
                    {
                        row.GetCell(h.Value).SetCellType(CellType.String);
                        dataRow[h.Key] = row.GetCell(h.Value).StringCellValue.Replace(System.Environment.NewLine,"").Replace("\n", "");
                    }
                }
                if (_context.SrmVendors.Any(r => r.SrmVendor1.Equals(dataRow["供應商編號"]) && r.Status != (int)(Status.失效) && user.Werks.Contains(r.Ekorg.Value)))//2021/10/19問過LEO 同名字不同廠可能存在多筆
                {
                    dataRow["IsExists"] = true;
                }
                else
                {
                    dataRow["IsExists"] = false;
                    if (_context.SrmVendors.Any(r => r.SrmVendor1.Equals(dataRow["供應商編號"]) && r.Status.Equals(Status.失效) && user.Werks.Contains(r.Ekorg.Value)))//2021/10/19問過LEO 同名字不同廠可能存在多筆
                    {
                        throw new Exception($"供應商編號:{dataRow["供應商編號"]}已失效");
                    }
                }
                //dataRow["IsExists"] = _context.SrmVendors.Any(r => r.SrmVendor1.Equals(dataRow["供應商編號"]) && user.Werks.Contains(r.Ekorg.Value));//2021/10/19問過LEO 同名字不同廠可能存在多筆
                dt.Rows.Add(dataRow);
                rowIndex++;
            }
            for (int i = 0; i < headers.Count(); i++)
            {
                dt.Columns[headers[i]].ColumnName = cols[i];
            }
            return dt;
        }
        public void Delete(string path) {
            try
            {
                FileInfo f = new FileInfo(path);
                if (f.Exists)
                {
                    f.Delete();
                }
            }
            catch (Exception ex) { 
            }
        }
        #endregion upload
    }
}
