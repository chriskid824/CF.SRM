using AutoMapper;
using Convience.Entity.Data;
using Convience.Entity.Entity;
using Convience.Entity.Entity.Identity;
using Convience.Entity.Entity.SRM;
using Convience.EntityFrameWork.Repositories;
using Convience.Filestorage.Abstraction;
using Convience.JwtAuthentication;
using Convience.Model.Constants.SystemManage;
using Convience.Model.Models;
using Convience.Model.Models.SRM;
using Convience.Model.Models.SystemManage;
using Convience.Util.Extension;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace Convience.Service.SRM
{
    public interface ISrmQotService
    {
        /// <summary>
        /// 取得全部角色
        /// </summary>
        public void Add(SrmQotH[] qoths);
        public void UpdateStatus(int status, SrmRfqH rfqH);
        public SrmQotH[] Get(QueryQot query);
        public IEnumerable<ViewQotListH> GetQotList();
        public IEnumerable<ViewQotListH> GetQotList(QueryQotList query);
        //public IEnumerable<ViewQot> GetDataBQotId(int QotId);
        public IQueryable GetQotData(int QotId, int vendorid, int qotid);
        public IQueryable GetMatnrData(int rfqid, int vendorid);
        //public ViewQotResult GetDetail(SrmQotH[] query);
        public ViewQotResult GetDetail(QueryQot query);
        public string Save(SrmQotH qotH, SrmQotMaterial[] qotMaterials, SrmQotSurface[] qotSurfaces, SrmQotProcess[] qotProcesses, SrmQotOther[] qotOthers);
        public void UpdateQotStatus(int status, SrmQotUpdateMaterial qotH);
        public SrmQotH[] GetByVendor(QueryQot query);
        //public ViewQotListL GetDataByRfqId(int RfqId);
        public IEnumerable<ViewQotListH> GetQotListByAdmin(QueryQotList query);
        public void InsertRejectReason(SrmQotUpdateMaterial qotH);
        public SrmQotH GetQot(QueryQot query);
        public bool CheckAllQot(SrmQotH qotH);
        public void UpdateRfqStatus(int status, SrmQotUpdateMaterial qoth);
        public int GetRowNum(SrmQotH qotH);
        public SrmProcess[] GetProcess();
        public SrmMaterial[] GetMaterial();
        public int GetQotStatus(SrmQotH qotH);
        public string GetProcessByNum(int num);
        //public string SendAll(int rfqid, int vendorid);
        public int GetVendorId(JObject qot);

        public string GetSapid(QueryQotList query);
        public SrmSurface[] GetSurface();
        public string Upload(Model.Models.SRM.FileUploadViewModel_QOT fileUploadModel);

        public DataTable ReadExcel_QotH(string path, UserClaims user);
        public DataTable ReadExcel_Material(string path, UserClaims user);
        public DataTable ReadExcel_Process(string path, UserClaims user);
        public DataTable ReadExcel_Surface(string path, UserClaims user);
        public DataTable ReadExcel_Other(string path, UserClaims user);
        public int CheckMatnrData(string vendor, string RfqNum, string Matnr);
        public void Delete(string path);
        public IQueryable GetQotInfo(int rfqid, int vendorid, int qotid);
        public int GetQotId(QueryQot query);
        public int  GetMatnrId(QueryQot query);
        public string GetSurfaceProcessByNum(int num);
        public ViewQotResult GetDetailByVendorRfq(QueryQot query);
        public SrmQotH GetQotById(QueryQot query);
        public HttpResponseMessage GetFile(string fileName, string localFilePath);

    }
    public class SrmQotService : ISrmQotService
    {
        private readonly IRepository<SrmRfqH> _srmRfqHRepository;
        private readonly IRepository<SrmRfqM> _srmRfqMRepository;
        private readonly IRepository<SrmQotH> _srmQotHRepository;
        private readonly IRepository<SrmMatnr> _srmSrmMatnrRepository;
        private readonly SRMContext _context;

        public SrmQotService(
            IRepository<SrmQotH> srmQotHRepository, IUnitOfWork<SystemIdentityDbContext> unitOfWork, SRMContext context)
        {
            _srmQotHRepository = srmQotHRepository;
            _context = context;
        }
        public void Add(SrmQotH[] qoths)
        {

            //using (SRMContext db = new SRMContext())
            //{
            _context.SrmQotHs.AddRange(qoths);
            _context.SaveChanges();
            //}

            //db.SrmQotHs.AddRange(qoths);
            //db.SaveChanges();
        }
        public void UpdateStatus(int status, SrmRfqH rfqH)
        {
            var qots = _srmQotHRepository.Get(r => r.RfqId == rfqH.RfqId);
            foreach (var qot in qots)
            {
                qot.Status = status;
                qot.LastUpdateDate = rfqH.LastUpdateDate;
                qot.LastUpdateBy = rfqH.LastUpdateBy;
            }
            //using (SRMContext db = new SRMContext())
            //{
            _context.UpdateRange(qots);
            _context.SaveChanges();
            //}
        }
        //public bool IfCanUpdate(SrmQotH qotH)
        //{
        //    bool ifcanupdate = true;
        //    var rfq = _srmRfqRepository.Get(r => r.RfqId == qotH.RfqId).First();
        //    if ((Status)rfq.Status != Status.確認)
        //    {
        //        ifcanupdate = false;
        //    }
        //    DateTime today = DateTime.Now.Date;
        //    if (today > rfq.EndDate)
        //    {
        //        ifcanupdate = false;
        //    }
        //    return ifcanupdate;
        //}
        public void UpdateQotStatus(int status, SrmQotUpdateMaterial qotH)
        {
            //0825
            var qotid = qotH.QotId;
            if (qotH.MatnrId != null)
            {
                QueryQot query = new QueryQot();
                query.rfqId = qotH.RfqId;
                query.vendorId = qotH.VendorId;
                query.matnrId = qotH.MatnrId;
                qotid = GetQotId(query);
            }
            /*QueryQot q = new QueryQot();
            q.matnrId = qotH.MatnrId;
            q.vendorId = qotH.VendorId;
            q.rfqId = qotH.RfqId;
            var qotid = GetQotId(q);*/
            //0825
            var qot = _srmQotHRepository.Get(r => r.QotId == qotid).First(); //qotH.QotId).First();

            //if ((Status)qot.Status != Status.初始)
            //20211227 暫開失效
            if (((Status)qot.Status != Status.初始) && ((Status)qot.Status != Status.失效))
            {
                throw new Exception($"非初始狀態無法{((Status)status).ToString()}");
            }
            /*qot.Status = status;
            qot.LastUpdateDate = qotH.LastUpdateDate;
            qot.LastUpdateBy = qotH.LastUpdateBy;
            using (SRMContext db = new SRMContext())
            {
                db.Update(qot);
                db.SaveChanges();
                return qot;
            }*/
            DateTime now = DateTime.Now;
            using (var db = new SRMContext())
            {
                var qoth = new SrmQotH() { QotId = qotid };//qotH.QotId };
                db.SrmQotHs.Attach(qoth);
                qoth.LastUpdateDate = now;
                qoth.LastUpdateBy = qotH.LastUpdateBy;
                qoth.Status = status;
                db.Entry(qoth).Property(p => p.LastUpdateBy).IsModified = true;
                db.Entry(qoth).Property(p => p.LastUpdateDate).IsModified = true;
                db.Entry(qoth).Property(p => p.Status).IsModified = true;
                var result = db.SaveChanges();
                //return result;
            }
        }
        #region 檢核所有qot
        public bool CheckAllQot(SrmQotH qotH)
        {
            //只要有一張qot為初始狀態，rfq就不更新
            bool IfChangeRfq = true; //預設變更
            SrmQotH[] qs = _context.SrmQotHs.AsQueryable().Where(p => p.RfqId == qotH.RfqId).ToArray();
            foreach (var q in qs)
            {
                if (q.Status == 1)
                {
                    IfChangeRfq = false;
                    return IfChangeRfq;
                }
            }
            //檢核該rfq的所有qot loop 檢核
            return IfChangeRfq;
        }
        #endregion
        #region 更新rfq 狀態
        public void UpdateRfqStatus(int status, SrmQotUpdateMaterial qoth)
        {
            //int rfqid = qotH.RfqId.Value;

            var rfq = _context.SrmRfqHs.Where(r => r.RfqId == qoth.RfqId.Value).FirstOrDefault();
            //var rfq = _srmRfqHRepository.Get().First();
            //var qot = _srmQotHRepository.Get(r => r.QotId == qotid).First();
            if ((Status)rfq.Status != Status.啟動)
            {
                throw new Exception($"非啟動狀態無法{((Status)status).ToString()}");
            }
            DateTime now = DateTime.Now;
            using (var db = new SRMContext())
            {
                var rfqh = new SrmRfqH() { RfqId = qoth.RfqId.Value };//qotH.QotId };
                db.SrmRfqHs.Attach(rfqh);
                rfqh.LastUpdateDate = now;
                rfqh.LastUpdateBy = qoth.LastUpdateBy;
                rfqh.Status = status;
                db.Entry(rfqh).Property(p => p.LastUpdateBy).IsModified = true;
                db.Entry(rfqh).Property(p => p.LastUpdateDate).IsModified = true;
                db.Entry(rfqh).Property(p => p.Status).IsModified = true;
                var result = db.SaveChanges();
                //return result;
            }
        }
        #endregion
        //#region 檢核日期，日期已過回傳status =17失效
        //public int GetQotStatus(SrmQotH qot) 
        //{
        //    int status = qot.Status.Value;
        //    //查詢rfq的日期
        //    return status;
        //}
        //#endregion
        public SrmQotH[] Get(QueryQot query)
        {
            //using (SRMContext db = new SRMContext())
            //{
            var qotQurty = _context.SrmQotHs.AsQueryable()
                .AndIfHaveValue(query.rfqId, r => r.RfqId == query.rfqId)
                .AndIfHaveValue(query.matnrId, r => r.MatnrId == query.matnrId)
                .AndIfHaveValue(query.vendorId, r => r.VendorId == query.vendorId)
                .OrderBy(r => r.MatnrId);
            return qotQurty.ToArray();
            //}
        }
        public SrmQotH[] GetByVendor(QueryQot query)
        {
            //using (SRMContext db = new SRMContext())
            //{
            var qotQurty = _context.SrmQotHs.AsQueryable()
                .AndIfHaveValue(query.rfqId, r => r.RfqId == query.rfqId)
                //.AndIfHaveValue(query.matnrId, r => r.MatnrId == query.matnrId)
                .AndIfHaveValue(query.vendorId, r => r.VendorId == query.vendorId)
                .OrderBy(r => r.QotId);
            return qotQurty.ToArray();
            //}
        }
        public SrmQotH GetQot(QueryQot query)
        {
            //using (SRMContext db = new SRMContext())
            //{
            var qot = _context.SrmQotHs
.Where(p => p.QotId == query.qotId).ToList().First();

            if (query.matnrId != null)
            {
                qot = _context.SrmQotHs
              .AndIfHaveValue(query.matnrId, r => r.MatnrId == query.matnrId)
              .Where(p => p.RfqId == query.rfqId && p.VendorId == query.vendorId).ToList().First();
            }

            return qot;
            //}
        }
        /*public IEnumerable<ViewSrmQotList> GetQotList(QueryQotList query)
        {
            
            var qotlist = (from q in _context.SrmQotHs
                           join r in _context.SrmRfqHs on q.RfqId equals r.RfqId
                           join v in _context.SrmVendors on q.VendorId equals v.VendorId
                           join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
                           //where e.OwnerID == user.UID
                           select new ViewSrmQotList
                           {
                               RFQ_NUM = r.RfqNum,
                               RFQ_STATUS = r.Status,
                               QOT_NUM = q.QotNum,
                               VENDOR_ID = q.VendorId,
                               MATNR_ID = q.MatnrId,
                               CURRENCY = q.Currency,
                               LEAD_TIME = q.LeadTime,
                               MIN_QTY = q.MinQty,
                               TOTAL_AMOUNT = q.TotalAmount,
                               QSTATUS = q.Status,
                               QCREATE_DATE = q.CreateDate,
                               QCREATE_BY = q.CreateBy,
                               QLAST_UPDATE_DATE = q.LastUpdateDate,
                               QLAST_UPDATE_BY = q.LastUpdateBy,
                               RSTATUS = r.Status,
                               RCREATE_DATE = r.CreateDate,
                               RCREATE_BY = r.CreateBy,
                               RLAST_UPDATE_DATE = r.LastUpdateDate,
                               RLAST_UPDATE_BY = r.LastUpdateBy,
                               VENDOR = v.SapVendor,
                               MATNR = m.SapMatnr,
                           })
                           .AndIfCondition(query.status != 0, p => p.QSTATUS == query.status)
                           .AndIfHaveValue(query.matnr, p => p.MATNR == query.matnr)
                           .AndIfHaveValue(query.rfqno, p => p.RFQ_NUM == query.rfqno);
            qotlist = qotlist.Where(p => p.VENDOR_ID == query.vendor);
            return qotlist;           
        }*/
        public IEnumerable<ViewQotListH> GetQotList()
        {
            //只帶供應商
            //var result = _context.SrmRfqHs
            //    .ToList();
            // .AndIfCondition(!string.IsNullOrWhiteSpace(query.deliveryNum), p => p.DeliveryNum.IndexOf(query.deliveryNum) > -1)
            // .AndIfCondition(query.status != 0, p => p.Status == query.status).ToList();
            //result.ForEach(p => {
            //    //p.SrmQotHs = _context.SrmQotHs.Where(m => m.RfqId == p.RfqId).ToList();//.Select(new SrmRfqH { }).ToList();
            //    p.SrmQotHs = _context.SrmQotHs.Where(m => m.RfqId == p.RfqId).ToList();
            //});
            //return result;
            var result = (from r in _context.SrmRfqHs
                          join q in _context.SrmQotHs on r.RfqId equals q.RfqId 
                          join status in _context.SrmStatuses on r.Status equals status.Status
                          join vendor in _context.SrmVendors on q.VendorId equals vendor.VendorId
                          select new ViewQotListH
                          {
                              VRfqId = r.RfqId,
                              VRfqNum = r.RfqNum,
                              VStatusDesc = status.StatusDesc,
                              VCreateDate = r.CreateDate,
                              VCreateBy = r.CreateBy,
                              VLastUpdateDate = r.LastUpdateDate,
                              VLastUpdateBy = r.LastUpdateBy,
                              VEndDate = r.EndDate,
                              VVendor = vendor.VendorName,
                          })
                          .Where(p => p.VStatus == 7)
                          .Distinct()
                          .ToList();

            result.ForEach(p =>
            {
                p.SrmQotHs = (from q in _context.SrmQotHs

                              join r in _context.SrmRfqHs on q.RfqId equals r.RfqId

                              join status in _context.SrmStatuses on q.Status equals status.Status
                              join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
                              join v in _context.SrmVendors on q.VendorId equals v.VendorId
                              select new ViewQotListL
                              {
                                  QRfqId = q.RfqId,
                                  QStatus = q.Status,
                                  QQotId = q.QotId,
                                  QQotNum = q.QotNum,
                                  QMatnr = (!string.IsNullOrWhiteSpace(m.SapMatnr))? m.SapMatnr : m.SrmMatnr1,
                                  QCreateBy = q.CreateBy,
                                  QCreateDate = q.CreateDate,
                                  QLastUpdateBy = q.LastUpdateBy,
                                  QLastUpdateDate = q.LastUpdateDate,
                                  QStatusDesc = status.StatusDesc
                              }).Where(p => p.Status != 5)
                              .Where(l => l.RfqId == p.RfqId)
                              .ToList();
            });
            return result.Where(p => p.SrmQotHs.Count > 0).ToList();

        }
        //原本的
        /*public IEnumerable<SrmRfqH> GetQotList(QueryQotList query)
        {
            //只帶供應商
            var result = _context.SrmRfqHs
                .ToList();
            // .AndIfCondition(!string.IsNullOrWhiteSpace(query.deliveryNum), p => p.DeliveryNum.IndexOf(query.deliveryNum) > -1)
            // .AndIfCondition(query.status != 0, p => p.Status == query.status).ToList();
            result.ForEach(p => {
                //p.SrmQotHs = _context.SrmQotHs.Where(m => m.RfqId == p.RfqId).ToList();//.Select(new SrmRfqH { }).ToList();
                p.SrmQotHs = _context.SrmQotHs.Where(m => m.RfqId == p.RfqId).ToList();
            });
            return result;

        }*/
        //原本的
        public IEnumerable<ViewQotListH> GetQotListByAdmin(QueryQotList query)
        {
            //int venderid = query.vendor;
            var result = (from r in _context.SrmRfqHs
                          join q in _context.SrmQotHs on new { RfqId=r.RfqId, username=r.CreateBy } equals new { RfqId=q.RfqId.Value, username=query.username }
                          //join q in _context.SrmQotHs on r.RfqId equals q.RfqId
                          join status in _context.SrmStatuses on q.Status equals status.Status
                          //join vendor in _context.SrmVendors on q.VendorId equals vendor.VendorId
                          join u1 in _context.AspNetUsers on q.CreateBy equals u1.UserName
                          join u2 in _context.AspNetUsers on q.LastUpdateBy equals u2.UserName
                          join record in _context.SrmInforecords on q.QotId equals record.QotId into recordt
                          from record in recordt.DefaultIfEmpty()
                          select new ViewQotListH
                          {
                              VRfqId = r.RfqId,
                              VRfqNum = q.QotNum,
                              VStatus = q.Status,
                              VStatusDesc = status.StatusDesc,
                              VCreateDate = q.CreateDate,
                              VCreateBy = u1.Name,
                              VLastUpdateDate = q.LastUpdateDate,
                              VLastUpdateBy = u2.Name,
                              VEndDate = r.EndDate,
                              Sourcer= record.InfoId.ToString()
                              //VVendor = vendor.SapVendor,


                          })
                        //.Where(p => p.VVendor == query.vendor)
                        .Where(p => (p.VStatus == 5||p.VStatus==6)&& p.Sourcer==null )
                            //.Where()
                            //.AndIfCondition(query.status != 0, p => p.Status == 7)
                            //.AndIfCondition(!string.IsNullOrWhiteSpace(query.vendor), p => p.VVendor == query.vendor)
                            .AndIfCondition(!string.IsNullOrWhiteSpace(query.rfqno), p => p.VRfqNum == query.rfqno)
                            .AndIfHaveValue(query.rfqId, p => p.VRfqId == query.rfqId)
                            //.AndIfHaveValue(query.username,p=>p.)
                        //.Distinct()
                        .ToList();

            //result.ForEach(p =>
            //{
            //    p.SrmQotHs = (from q in _context.SrmQotHs

            //                  join r in _context.SrmRfqHs on q.RfqId equals r.RfqId
            //                  join status in _context.SrmStatuses on q.Status equals status.Status
            //                  join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
            //                  join u3 in _context.AspNetUsers on r.CreateBy equals u3.UserName
            //                  join u4 in _context.AspNetUsers on r.LastUpdateBy equals u4.UserName
            //                  //join v in _context.SrmVendors on q.VendorId equals v.VendorId
            //                  select new ViewQotListL
            //                  {
            //                      QRfqId = q.RfqId,
            //                      QStatus = q.Status,
            //                      QQotId = q.QotId,
            //                      QQotNum = q.QotNum,
            //                      QMatnr = (!string.IsNullOrWhiteSpace(m.SapMatnr)) ? m.SapMatnr : m.SrmMatnr1,
            //                      QCreateBy = u3.Name,
            //                      QCreateDate = q.CreateDate,
            //                      QLastUpdateBy = u4.Name,
            //                      QLastUpdateDate = q.LastUpdateDate,
            //                      QVendorId = q.VendorId,
            //                      //QVendor = v.SapVendor,
            //                      QStatusDesc = status.StatusDesc
            //                  })
            //                  //.ToList();
            //                  //.Where(p => p.QVendorId.Value == query.vendor)

            //                  //.Where(p => p.QVendor == query.vendor) //供應商登入帳號為供應商代碼
            //                  .Where(l => l.QRfqId.Value == p.VRfqId)
            //                  .AndIfCondition(!string.IsNullOrWhiteSpace(query.matnr), p => p.QMatnr == query.matnr)
            //                  .AndIfCondition(query.status != 0, p => p.QStatus.Value == query.status)
            //                  .ToList();
            //});
            return result.ToList();
        }
       

        public IEnumerable<ViewQotListH> GetQotList(QueryQotList query)
        {

            string vendorname = GetSapid(query);//20211203
            #region getvendorid 20211101
            int venderid = GetVendorId(vendorname);
            #endregion
            //int venderid = query.vendor;
            var result = (from r in _context.SrmRfqHs
                          join q in _context.SrmQotHs on r.RfqId equals q.RfqId
                          join status in _context.SrmStatuses on r.Status equals status.Status
                          join vendor in _context.SrmVendors on q.VendorId equals vendor.VendorId                         
                          join u1 in _context.AspNetUsers on r.CreateBy equals u1.UserName into u1g
                          from u1 in u1g.DefaultIfEmpty()
                          join u2 in _context.AspNetUsers on r.LastUpdateBy equals u2.UserName into u2g
                          from u2 in u2g.DefaultIfEmpty()
                          select new ViewQotListH
                          {
                              VRfqId = r.RfqId,
                              VRfqNum = r.RfqNum,
                              VStatus = r.Status,
                              VStatusDesc = status.StatusDesc,
                              VCreateDate = r.CreateDate,
                              VCreateBy = (!string.IsNullOrWhiteSpace(u1.Name)) ? u1.Name : r.CreateBy,
                              VLastUpdateDate = r.LastUpdateDate,
                              VLastUpdateBy = (!string.IsNullOrWhiteSpace(u2.Name))? u2.Name:r.LastUpdateBy,
                              VEndDate = r.EndDate,
                              VVendor = (!string.IsNullOrWhiteSpace(vendor.SapVendor))? vendor.SapVendor:vendor.SrmVendor1,
                              VVendorId = vendor.VendorId,
                              Werks = r.Werks,
                              VDeadline = r.Deadline
                          })
                        //.Where(p => p.VVendor == query.vendor)
                        .Where(p => p.VVendorId == venderid)
                            //.Where(p => p.VStatus == 7) //0827
                            //.Where()
                            //.AndIfCondition(query.status != 0, p => p.Status == 7)
                            //.AndIfCondition(!string.IsNullOrWhiteSpace(query.vendor), p => p.VVendor == query.vendor)
                            .AndIfCondition(!string.IsNullOrWhiteSpace(query.rfqno), p => p.VRfqNum == query.rfqno)
                            .AndIfHaveValue(query.rfqId, m => m.VRfqId == query.rfqId)
                        .Distinct()
                        .ToList();
           
            result.ForEach(p =>
            {
                p.SrmQotHs = (from q in _context.SrmQotHs

                              join r in _context.SrmRfqHs on q.RfqId equals r.RfqId
                              join status in _context.SrmStatuses on q.Status equals status.Status
                              join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
                              join v in _context.SrmVendors on q.VendorId equals v.VendorId
                              join u1 in _context.AspNetUsers on q.CreateBy equals u1.UserName into u1g
                              from u1 in u1g.DefaultIfEmpty()
                              join u2 in _context.AspNetUsers on q.LastUpdateBy equals u2.UserName into u2g
                              from u2 in u2g.DefaultIfEmpty()
                              select new ViewQotListL
                              {
                                  QRfqId = q.RfqId,
                                  QStatus = q.Status,
                                  QQotId = q.QotId,
                                  QQotNum = q.QotNum,
                                  QMatnr = (!string.IsNullOrWhiteSpace(m.SapMatnr)) ? m.SapMatnr : m.SrmMatnr1,
                                  QCreateBy = (!string.IsNullOrWhiteSpace(u1.Name)) ? u1.Name : q.CreateBy,
                                  QCreateDate = q.CreateDate,
                                  QLastUpdateBy = (!string.IsNullOrWhiteSpace(u2.Name)) ? u2.Name : q.LastUpdateBy,
                                  QLastUpdateDate = q.LastUpdateDate,
                                  QVendorId = q.VendorId,
                                  QVendor = (!string.IsNullOrWhiteSpace(v.SapVendor)) ? v.SapVendor : v.SrmVendor1,
                                  QStatusDesc = status.StatusDesc
                              })
                              //.ToList();
                              //.Where(p => p.QVendorId.Value == query.vendor)

                              .Where(p => p.QVendor == query.vendor) //供應商登入帳號為供應商代碼
                              .Where(l => l.QRfqId.Value == p.VRfqId)

                              .AndIfCondition(!string.IsNullOrWhiteSpace(query.matnr), p => p.QMatnr == query.matnr)
                              .AndIfCondition(query.status != 0, p => p.QStatus.Value == query.status) //0827
                              .ToList();
            });
            return result.Where(p => p.SrmQotHs.Count > 0).ToList();
        }

        /*public IEnumerable<ViewSrmQotList> GetQotList(QueryQotList query)
        {

            var result = _context.SrmRfqHs.Join(
                _context.SrmQotHs,
                r => r.RfqId,
                q => q.RfqId,
                (r, q) => new ViewSrmQotList
                {
                    RFQ_NUM = r.RfqNum,
                    RSTATUS = r.Status,
                    RFQ_ID = r.RfqId,
                    RCREATE_DATE = r.CreateDate,
                    RCREATE_BY = r.CreateBy,
                    RLAST_UPDATE_DATE = r.LastUpdateDate,
                    RLAST_UPDATE_BY = r.LastUpdateBy,
                    QOT_ID = q.QotId,
                    QOT_NUM = q.QotNum,
                    QSTATUS = q.Status,
                    QCREATE_DATE = q.CreateDate,
                    QCREATE_BY = q.CreateBy,
                    QLAST_UPDATE_DATE = q.LastUpdateDate,
                    QLAST_UPDATE_BY = q.LastUpdateBy,
                    MATNR_ID = q.MatnrId,
                    VENDOR_ID = q.VendorId
                }
                ).ToList();
              
            // .AndIfCondition(!string.IsNullOrWhiteSpace(query.deliveryNum), p => p.DeliveryNum.IndexOf(query.deliveryNum) > -1)
            // .AndIfCondition(query.status != 0, p => p.Status == query.status).ToList();
            result.ForEach(p =>
            {
                p.MATNR = _context.SrmMatnrs.Find(p.MATNR_ID).SapMatnr;
                //p.SrmQotHs = _context.SrmQotHs.Where(m => m.RfqId == p.RfqId).ToList();//.Select(new SrmRfqH { }).ToList();
                p.SrmQotHs = _context.SrmQotHs.Where(m => m.RfqId == p.RfqId).ToList();
            });
            int vendor = query.vendor;
            return result.Where(p => p.VENDOR_ID == vendor).ToList();
        }*/
        public IQueryable GetMatnrData(int rfqid, int vendorid)
        {
            var qotlist = (from q in _context.SrmQotHs
                           join r in _context.SrmRfqHs on q.RfqId equals r.RfqId
                           join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
                           join s in _context.SrmStatuses on q.Status equals s.Status
                           select new
                           {
                               QotId = q.QotId,
                               RfqId = r.RfqId,
                               VendorId = q.VendorId,
                               RfqNum = r.RfqNum,
                               Matnr = ((!string.IsNullOrWhiteSpace(m.SapMatnr)) ? m.SapMatnr : m.SrmMatnr1) + "(" + s.StatusDesc + ")",
                               MatnrId = q.MatnrId,
                               QotNum = q.QotNum,
                               Status = s.StatusDesc,
                               Werks = r.Werks,
                               Deadline = r.Deadline
                           });
            //.AndIfCondition(query.status != 0, p => p.QSTATUS == query.status)
            //.AndIfHaveValue(query.matnr, p => p.MATNR == query.matnr)
            //.AndIfHaveValue(query.rfqno, p => p.RFQ_NUM == query.rfqno);
            qotlist = qotlist.Where(p => p.RfqId == rfqid && p.VendorId == vendorid).OrderBy(p => p.QotId);
            return qotlist;
        }
        public decimal GetHistoryPrice(int qotid) 
        {
            decimal historyprice = 0;
            var history = (from q in _context.SrmQotHs 
                           join v in _context.SrmVendors on q.VendorId equals v.VendorId
                           join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
                           join h in _context.SrmHistoryPrices on new { Vendor = (!string.IsNullOrWhiteSpace(v.SapVendor)? v.SapVendor:v.SrmVendor1), Matnr = (!string.IsNullOrWhiteSpace(m.SapMatnr)? m.SapMatnr:m.SrmMatnr1) } equals new { Vendor = h.Vendor, Matnr = h.Matnr }
                           where (q.QotId == qotid)
                          select new
                          {
                              historyprice = h.TargetPrice,
                          });
            if (history.Count()>0)
            {
                historyprice = history.Select(r => (r.historyprice != null) ? r.historyprice.Value:0).First();
            }
            return historyprice;
        }
        public IQueryable GetQotData(int rfqid, int vendorid, int qotid)
        {
            //decimal historyprice = GetHistoryPrice(qotid);
            var qotlist = (from r in _context.SrmRfqHs
                           join q in _context.SrmQotHs on r.RfqId equals q.RfqId
                           join rm in _context.SrmRfqMs on new { RfqId = q.RfqId, MatnrId = q.MatnrId } equals new { RfqId = rm.RfqId, MatnrId = rm.MatnrId }                         
                           join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
                           join v in _context.SrmVendors on q.VendorId equals v.VendorId
                           join hi in _context.SrmHistoryPrices on new { Vendor = (!string.IsNullOrWhiteSpace(v.SapVendor)) ? v.SapVendor : v.SrmVendor1, Matnr = (!string.IsNullOrWhiteSpace(m.SapMatnr)) ? m.SapMatnr : m.SrmMatnr1 } equals new { Vendor = hi.Vendor, Matnr = hi.Matnr } into hig
                           from hi in hig.DefaultIfEmpty()
                           join mu in _context.SrmMeasureUnits on rm.Unit equals mu.MeasureId into mug
                           from mu in mug.DefaultIfEmpty()
                           join s in _context.SrmStatuses on q.Status equals s.Status
                           join u1 in _context.AspNetUsers on q.CreateBy equals u1.UserName into u1g
                           from u1 in u1g.DefaultIfEmpty()
                          

                           //where e.OwnerID == user.UID 
                           select new
                           {
                               RfqNum = r.RfqNum,
                               Matnr = (!string.IsNullOrWhiteSpace(m.SapMatnr)) ? m.SapMatnr : m.SrmMatnr1,
                               Description = m.Description,
                               Expiringdate = q.ExpirationDate,
                               Leadtime = q.LeadTime,
                               Note = q.Note,
                               mEmptyFlag = q.MEmptyFlag,
                               pEmptyFlag = q.PEmptyFlag,
                               sEmptyFlag = q.SEmptyFlag,
                               oPEmptyFlag = q.OEmptyFlag,
                               QotId = q.QotId,
                               CreateBy = (!string.IsNullOrWhiteSpace(u1.Name))? u1.Name:q.CreateBy,//q.CreateBy,
                               CreateDate = DateTime.Parse(q.CreateDate.ToString()).ToString("yyyy/MM/dd HH:mm:ss"),
                               Status = s.StatusDesc,
                               
                               QotNum = q.QotNum,
                               //Status = q.Status.HasValue ? ((Status)q.Status).ToString() : "",
                               Material = rm.Material,
                               Weight = rm.Weight,
                               MachineName = rm.MachineName,
                               Size = rm.Length + "*" + rm.Width + "*" + rm.Height,
                               Length = rm.Length,
                               Width = rm.Width,
                               Height = rm.Height,
                               RfqId = r.RfqId,
                               VendorId = q.VendorId,
                               Qty = rm.Qty,
                               estDeliveryDate = (!string.IsNullOrWhiteSpace(rm.EstDeliveryDate.ToString())) ? DateTime.Parse(rm.EstDeliveryDate.ToString()).ToString("yyyy/MM/dd") : "",
                               Unit = mu.MeasureDesc,//20211203
                               Purposeprice = hi.TargetPrice//historyprice
                           });
            //.AndIfCondition(query.status != 0, p => p.QSTATUS == query.status)
            //.AndIfHaveValue(matnrid, p => p.MATNR == query.matnr).t
            //.AndIfHaveValue(query.rfqno, p => p.RFQ_NUM == query.rfqno);
            qotlist = qotlist
            .Where(p => p.RfqId == rfqid)
            .Where(p => p.VendorId == vendorid)
            .OrderBy(p => p.QotId);
            //.AndIfHaveValue(matnrid , p => p.QotId == qotid);
            //.Where(p => p.QotId == qotid);
            return qotlist;
        }
     
        /*public IEnumerable<ViewQot> GetDataBQotId(int QotId)
        {
            var qotlist = (from q in _context.SrmQotHs
                           join r in _context.SrmRfqHs on q.RfqId equals r.RfqId
                           join rm in _context.SrmRfqMs on r.RfqId equals rm.RfqId
                           join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId

                           //where e.OwnerID == user.UID
                           select new ViewQot
                           {
                               CreateBy = q.CreateBy,
                               CreateDate = q.CreateDate,
                               QotId = q.QotId,
                               QotNum = q.QotNum,
                               Status = q.Status,
                               Matnr = m.SapMatnr,
                               Material = rm.Material,
                               Weight = rm.Weight,
                               MachineName = rm.MachineName,
                               Size = rm.Length +"*" + rm.Width +"*" + rm.Height,
                               Length = rm.Length,
                               Width = rm.Width,
                               Height = rm.Height
                           }).ToList();
                            //.AndIfCondition(query.status != 0, p => p.QSTATUS == query.status)
                            //.AndIfHaveValue(query.matnr, p => p.MATNR == query.matnr)
                            //.AndIfHaveValue(query.rfqno, p => p.RFQ_NUM == query.rfqno);
            qotlist = qotlist.Where(p => p.QotId == QotId).ToList(); ;
            return qotlist;
        }*/
        //public ViewQotResult GetDetail(SrmQotH[] query)
        //{
        //    //.AsEnumerable().Select(r => r.Field<string>("Name")).ToArray();
        //    int[] qotIds = query.AsEnumerable().Select(r => r.QotId).ToArray();
        //    int[] vendorIds = query.AsEnumerable().Select(r => r.VendorId.Value).Distinct().ToArray();
        //    var temp = query.AsEnumerable().Select(r => new { a = r.RfqId, b = r.VendorId }).ToArray();
        //    ViewQotResult qotinfo = new ViewQotResult();
        //    qotinfo.qot = query;
        //    //using (SRMContext _context = new SRMContext())
        //    //{

        //    qotinfo.material = (from material in _context.SrmQotMaterial
        //                      join qot in _context.SrmQotHs
        //                      on material.QotId equals qot.QotId
        //                      join vendor in _context.SrmVendors
        //                      on qot.VendorId equals vendor.VendorId
        //                      where qotIds.Contains(material.QotId.Value)
        //                      select new viewSrmQotMaterial
        //                      {
        //                          QotMId = material.QotMId,
        //                          QotId = material.QotId,
        //                          MMaterial = material.MMaterial,
        //                          MPrice = material.MPrice,
        //                          MCostPrice = material.MCostPrice,
        //                          Length = material.Length,
        //                          Width = material.Width,
        //                          Height = material.Height,
        //                          Density = material.Density,
        //                          Weight = material.Weight,
        //                          Note = material.Note,
        //                          VendorId = vendor.VendorId,
        //                          VendorName = vendor.VendorName
        //                      }).ToArray();
        //    //price.material = (from material in db.SrmQotMaterial
        //    //           where qotIds.Contains(material.QotId.Value)
        //    //           select material).ToArray();
        //    qotinfo.process = (from process in _context.SrmQotProcesses
        //                     join qot in _context.SrmQotHs
        //                     on process.QotId equals qot.QotId
        //                     join vendor in _context.SrmVendors
        //                     on qot.VendorId equals vendor.VendorId
        //                     where qotIds.Contains(process.QotId.Value)
        //                     select new viewSrmQotProcess
        //                     {
        //                         PHours = process.PHours,
        //                         PMachine = process.PMachine,
        //                         PNote =process.PNote,
        //                         PPrice = process.PPrice,
        //                         PProcessNum = process.PProcessNum,
        //                         VendorId = vendor.VendorId,
        //                         VendorName = vendor.VendorName,
        //                         SubTotal = process.PPrice.Value * (decimal)process.PHours.Value
        //                     }).ToArray();
        //    //price.process = (from process in db.SrmQotProcesses
        //    //                             where qotIds.Contains(process.QotId.Value)
        //    //                             select process).ToArray();
        //    qotinfo.surface = (from surface in _context.SrmQotSurfaces
        //                     join qot in _context.SrmQotHs
        //     on surface.QotId equals qot.QotId
        //                     join vendor in _context.SrmVendors
        //                     on qot.VendorId equals vendor.VendorId
        //                     where qotIds.Contains(surface.QotId.Value)
        //                     select new viewSrmQotSurface
        //                     {
        //                         SNote =surface.SNote,
        //                         SPrice = surface.SPrice,
        //                         SProcess = surface.SProcess,
        //                         STimes = surface.STimes,
        //                         VendorId = vendor.VendorId,
        //                         VendorName = vendor.VendorName,
        //                         SubTotal = surface.SPrice.Value * (decimal)surface.STimes.Value
        //                     }).ToArray();
        //    qotinfo.other = (from other in _context.SrmQotOthers
        //                   join qot in _context.SrmQotHs
        //     on other.QotId equals qot.QotId
        //                   join vendor in _context.SrmVendors
        //                   on qot.VendorId equals vendor.VendorId
        //                   where qotIds.Contains(other.QotId.Value)
        //                   select new viewSrmQotOther
        //                   {
        //                       ODescription =other.ODescription,
        //                       OItem = other.OItem,
        //                       ONote = other.ONote,
        //                       OPrice = other.OPrice,
        //                       VendorId = vendor.VendorId,
        //                       VendorName = vendor.VendorName
        //                   }).ToArray();




        //    return qotinfo;
        //}
        #region 丟user取sapid
        public string GetSapid(QueryQotList query)
        {
            string vendorid = string.Empty;
            var vendor = (from u in _context.AspNetUsers
                          where (u.UserName == query.vendor)
                          select new
                          {
                              SapId = u.SapId
                          });
            vendorid = vendor.Select(r => r.SapId).First();
            return vendorid;
        }
        #endregion
        public int GetVendorId(string vendorname)
        {
            var vendorid = 0;
            var vendor = (from v in _context.SrmVendors
                          where (v.SapVendor == vendorname || v.SrmVendor1 == vendorname)
                          select new
                          {
                              VendorId = v.VendorId,
                          });
            vendorid = vendor.Select(r => r.VendorId).First();
            return vendorid;

        }
        public int GetQotId(QueryQot query)
        {
            var qotid = 0;
            var qot = (from r in _context.SrmRfqHs
                       join q in _context.SrmQotHs on r.RfqId equals q.RfqId
                       join rm in _context.SrmRfqMs on new { RfqId = q.RfqId, MatnrId = q.MatnrId } equals new { RfqId = rm.RfqId, MatnrId = rm.MatnrId }
                       select new
                       {

                           QotId = q.QotId,
                           RfqId = r.RfqId,
                           VendorId = q.VendorId,
                           MatnrId = q.MatnrId

                       });
            //.AndIfCondition(query.status != 0, p => p.QSTATUS == query.status)
            //.AndIfHaveValue(query.matnr, p => p.MATNR == query.matnr)
            //.AndIfHaveValue(query.rfqno, p => p.RFQ_NUM == query.rfqno);
            qot = qot
            .Where(p => p.RfqId == query.rfqId)
            .Where(p => p.VendorId.Value == query.vendorId)
            .Where(p => p.MatnrId.Value == query.matnrId);
            qotid = qot.Select(r => r.QotId).First();
            return qotid;
        }
        public ViewQotResult GetDetail(QueryQot query)
        {
            var qotid = query.qotId;
            if (query.matnrId != null)
            {
                //var qot = GetQotId(query);
                qotid = GetQotId(query);
            }
            //.AsEnumerable().Select(r => r.Field<string>("Name")).ToArray();
            //int[] qotIds = query.AsEnumerable().Select(r => r.QotId).ToArray();
            //int[] vendorIds = query.AsEnumerable().Select(r => r.VendorId.Value).Distinct().ToArray();
            //var temp = query.AsEnumerable().Select(r => new { a = r.RfqId, b = r.VendorId }).ToArray();
            ViewQotResult qotinfo = new ViewQotResult();
            //qotinfo.qot = query;
            //using (SRMContext _context = new SRMContext())
            //{

            qotinfo.material = (from material in _context.SrmQotMaterial
                                join qot in _context.SrmQotHs
                                on material.QotId equals qot.QotId
                                join vendor in _context.SrmVendors
                                on qot.VendorId equals vendor.VendorId
                                //where qot.QotId equals(material.QotId.Value)
                                select new viewSrmQotMaterial
                                {
                                    MMaterial = material.MMaterial,
                                    Length = material.Length,
                                    Width = material.Width,
                                    Height = material.Height,
                                    Density = material.Density,
                                    Weight = material.Weight,
                                    MPrice = material.MPrice,
                                    MCostPrice = material.MCostPrice,
                                    Note = material.Note,
                                    QotMId = material.QotMId,
                                    QotId = material.QotId,
                                    VendorId = vendor.VendorId,
                                    VendorName = vendor.VendorName
                                })
                                .Where(p => p.QotId == qotid)
                                .ToArray();
            //price.material = (from material in db.SrmQotMaterial
            //           where qotIds.Contains(material.QotId.Value)
            //           select material).ToArray();
            qotinfo.process = (from process in _context.SrmQotProcesses
                               join qot in _context.SrmQotHs on process.QotId equals qot.QotId
                               join p in _context.SrmProcesss on process.PProcessNum equals p.ProcessNum.ToString() //20211217
                               join vendor in _context.SrmVendors
                               on qot.VendorId equals vendor.VendorId
                               //where qotIds.Contains(process.QotId.Value)
                               select new viewSrmQotProcess
                               {
                                   ProcessName = p.Process, //20211217
                                   PHours = process.PHours,
                                   PPrice = process.PPrice,
                                   PMachine = process.PMachine,
                                   PCostsum = process.PCostsum,
                                   PNote = process.PNote,
                                   PProcessNum = process.PProcessNum,
                                   VendorId = vendor.VendorId,
                                   VendorName = vendor.VendorName,                                 
                                   QotId = process.QotId
                               })
                               .Where(p => p.QotId == qotid)
                               .ToArray();
            //price.process = (from process in db.SrmQotProcesses
            //                             where qotIds.Contains(process.QotId.Value)
            //                             select process).ToArray();
            qotinfo.surface = (from surface in _context.SrmQotSurfaces
                               join qot in _context.SrmQotHs on surface.QotId equals qot.QotId
                               join s in _context.SrmSurfaces on surface.SProcess equals s.SurfaceId.ToString()  //20211217
                               join vendor in _context.SrmVendors
                               on qot.VendorId equals vendor.VendorId
                               //where qotIds.Contains(surface.QotId.Value)
                               select new viewSrmQotSurface
                               {
                                   ProcessName = s.SurfaceDesc, //20211217
                                   SPrice = surface.SPrice,
                                   STimes = surface.STimes,
                                   SCostsum = surface.SCostsum,
                                   SNote = surface.SNote,
                                   SProcess = surface.SProcess,
                                   VendorId = vendor.VendorId,
                                   VendorName = vendor.VendorName,
                                   //SubTotal = surface.SPrice.Value * (decimal)surface.STimes.Value,
                                   QotId = surface.QotId
                               })
                               .Where(p => p.QotId == qotid)
                               .ToArray();
            qotinfo.other = (from other in _context.SrmQotOthers
                             join qot in _context.SrmQotHs
               on other.QotId equals qot.QotId
                             join vendor in _context.SrmVendors
                             on qot.VendorId equals vendor.VendorId
                             //where qotIds.Contains(other.QotId.Value)
                             select new viewSrmQotOther
                             {
                                 OItem = other.OItem,
                                 OPrice = other.OPrice,
                                 ONote = other.ONote,
                                 ODescription = other.ODescription,
                                 VendorId = vendor.VendorId,
                                 VendorName = vendor.VendorName,
                                 QotId = other.QotId
                             }).Where(p => p.QotId == qotid)
                               .ToArray();

            return qotinfo;
        }
        #region
        public ViewQotResult GetDetailByVendorRfq(QueryQot query)
        {
          
            ViewQotResult qotinfo = new ViewQotResult();

            qotinfo.material = (from material in _context.SrmQotMaterial
                                join qot in _context.SrmQotHs
                                on material.QotId equals qot.QotId
                                join vendor in _context.SrmVendors
                                on qot.VendorId equals vendor.VendorId
                                //where qot.QotId equals(material.QotId.Value)
                                select new viewSrmQotMaterial
                                {
                                    
                                    MMaterial = material.MMaterial,
                                    Length = material.Length,
                                    Width = material.Width,
                                    Height = material.Height,
                                    Density = material.Density,
                                    Weight = material.Weight,
                                    MPrice = material.MPrice,
                                    MCostPrice = material.MCostPrice,
                                    Note = material.Note,
                                    QotMId = material.QotMId,
                                    QotId = material.QotId,
                                    VendorId = vendor.VendorId,
                                    VendorName = vendor.VendorName,
                                    RfqId = qot.RfqId.Value,                                 
                                })
                                .Where(p => p.RfqId == query.rfqId)
                                .Where(p => p.VendorId == query.vendorId)
                                .ToArray();
            
            qotinfo.process = (from process in _context.SrmQotProcesses
                               join qot in _context.SrmQotHs on process.QotId equals qot.QotId
                               join p in _context.SrmProcesss on process.PProcessNum equals p.ProcessNum.ToString() //20211217
                               join vendor in _context.SrmVendors
                               on qot.VendorId equals vendor.VendorId
                               //where qotIds.Contains(process.QotId.Value)
                               select new viewSrmQotProcess
                               {
                                   ProcessName = p.Process, //20211217
                                   PHours = process.PHours,
                                   PPrice = process.PPrice,
                                   PMachine = process.PMachine,
                                   PCostsum = process.PCostsum,
                                   PNote = process.PNote,
                                   PProcessNum = process.PProcessNum,
                                   VendorId = vendor.VendorId,
                                   VendorName = vendor.VendorName,
                                   QotId = process.QotId,
                                   RfqId = qot.RfqId.Value
                               })
                               .Where(p => p.RfqId == query.rfqId)
                               .Where(p => p.VendorId == query.vendorId)
                               .ToArray();
            //price.process = (from process in db.SrmQotProcesses
            //                             where qotIds.Contains(process.QotId.Value)
            //                             select process).ToArray();
            qotinfo.surface = (from surface in _context.SrmQotSurfaces
                               join qot in _context.SrmQotHs on surface.QotId equals qot.QotId
                               join s in _context.SrmSurfaces on surface.SProcess equals s.SurfaceId.ToString()  //20211217
                               join vendor in _context.SrmVendors
                               on qot.VendorId equals vendor.VendorId
                               //where qotIds.Contains(surface.QotId.Value)
                               select new viewSrmQotSurface
                               {
                                   ProcessName = s.SurfaceDesc, //20211217
                                   SPrice = surface.SPrice,
                                   STimes = surface.STimes,
                                   SCostsum = surface.SCostsum,
                                   SNote = surface.SNote,
                                   SProcess = surface.SProcess,
                                   VendorId = vendor.VendorId,
                                   VendorName = vendor.VendorName,
                                   //SubTotal = surface.SPrice.Value * (decimal)surface.STimes.Value,
                                   QotId = surface.QotId,
                                   RfqId = qot.RfqId.Value
                               })
                               .Where(p => p.RfqId == query.rfqId)
                               .Where(p => p.VendorId == query.vendorId)
                               .ToArray();
            qotinfo.other = (from other in _context.SrmQotOthers
                             join qot in _context.SrmQotHs
               on other.QotId equals qot.QotId
                             join vendor in _context.SrmVendors
                             on qot.VendorId equals vendor.VendorId
                             //where qotIds.Contains(other.QotId.Value)
                             select new viewSrmQotOther
                             {
                                 OItem = other.OItem,
                                 OPrice = other.OPrice,
                                 ONote = other.ONote,
                                 ODescription = other.ODescription,
                                 VendorId = vendor.VendorId,
                                 VendorName = vendor.VendorName,
                                 QotId = other.QotId,
                                 RfqId = qot.RfqId.Value
                             })
                                .Where(p => p.RfqId == query.rfqId)
                                .Where(p => p.VendorId == query.vendorId)
                                .ToArray();

            return qotinfo;
        }
        #endregion

        public ViewQotResult GetDetailByMatnr(QueryQot query)
        {
            //.AsEnumerable().Select(r => r.Field<string>("Name")).ToArray();
            //int[] qotIds = query.AsEnumerable().Select(r => r.QotId).ToArray();
            //int[] vendorIds = query.AsEnumerable().Select(r => r.VendorId.Value).Distinct().ToArray();
            //var temp = query.AsEnumerable().Select(r => new { a = r.RfqId, b = r.VendorId }).ToArray();
            ViewQotResult qotinfo = new ViewQotResult();
            //qotinfo.qot = query;
            //using (SRMContext _context = new SRMContext())
            //{

            qotinfo.material = (from material in _context.SrmQotMaterial
                                join qot in _context.SrmQotHs
                                on material.QotId equals qot.QotId
                                join vendor in _context.SrmVendors
                                on qot.VendorId equals vendor.VendorId
                                //where qot.QotId equals(material.QotId.Value)
                                select new viewSrmQotMaterial
                                {
                                    QotMId = material.QotMId,
                                    QotId = material.QotId,
                                    MMaterial = material.MMaterial,
                                    MPrice = material.MPrice,
                                    MCostPrice = material.MCostPrice,
                                    Length = material.Length,
                                    Width = material.Width,
                                    Height = material.Height,
                                    Density = material.Density,
                                    Weight = material.Weight,
                                    Note = material.Note,
                                    VendorId = vendor.VendorId,
                                    VendorName = vendor.VendorName,
                                })
                                .Where(p => p.QotId == query.qotId)
                                .ToArray();
            //price.material = (from material in db.SrmQotMaterial
            //           where qotIds.Contains(material.QotId.Value)
            //           select material).ToArray();
            qotinfo.process = (from process in _context.SrmQotProcesses
                               join qot in _context.SrmQotHs
                               on process.QotId equals qot.QotId
                               join vendor in _context.SrmVendors
                               on qot.VendorId equals vendor.VendorId
                               //where qotIds.Contains(process.QotId.Value)
                               select new viewSrmQotProcess
                               {
                                   PHours = process.PHours,
                                   PMachine = process.PMachine,
                                   PNote = process.PNote,
                                   PPrice = process.PPrice,
                                   PProcessNum = process.PProcessNum,
                                   VendorId = vendor.VendorId,
                                   VendorName = vendor.VendorName,
                                   SubTotal = process.PPrice.Value * (decimal)process.PHours.Value,
                                   QotId = process.QotId

                               })
                               .Where(p => p.QotId == query.qotId)
                               .ToArray();
            //price.process = (from process in db.SrmQotProcesses
            //                             where qotIds.Contains(process.QotId.Value)
            //                             select process).ToArray();
            qotinfo.surface = (from surface in _context.SrmQotSurfaces
                               join qot in _context.SrmQotHs
               on surface.QotId equals qot.QotId
                               join vendor in _context.SrmVendors
                               on qot.VendorId equals vendor.VendorId
                               //where qotIds.Contains(surface.QotId.Value)
                               select new viewSrmQotSurface
                               {
                                   SNote = surface.SNote,
                                   SPrice = surface.SPrice,
                                   SProcess = surface.SProcess,
                                   STimes = surface.STimes,
                                   VendorId = vendor.VendorId,
                                   VendorName = vendor.VendorName,
                                   SubTotal = surface.SPrice.Value * (decimal)surface.STimes.Value,
                                   QotId = surface.QotId
                               })
                               .Where(p => p.QotId == query.qotId)
                               .ToArray();
            qotinfo.other = (from other in _context.SrmQotOthers
                             join qot in _context.SrmQotHs
               on other.QotId equals qot.QotId
                             join vendor in _context.SrmVendors
                             on qot.VendorId equals vendor.VendorId
                             //where qotIds.Contains(other.QotId.Value)
                             select new viewSrmQotOther
                             {
                                 ODescription = other.ODescription,
                                 OItem = other.OItem,
                                 ONote = other.ONote,
                                 OPrice = other.OPrice,
                                 VendorId = vendor.VendorId,
                                 VendorName = vendor.VendorName,
                                 QotId = other.QotId
                             }).Where(p => p.QotId == query.qotId)
                               .ToArray();




            return qotinfo;
        }


        public string Save(SrmQotH qotH, SrmQotMaterial[] qotMaterials, SrmQotSurface[] qotSurfaces, SrmQotProcess[] qotProcesses, SrmQotOther[] qotOthers)
        {
            string errmsg = string.Empty;
            DateTime now = DateTime.Now;
            //qotH.LastUpdateDate = now;
            //0825
            var qotid = qotH.QotId;
            if (qotH.MatnrId != null)
            {
                QueryQot query = new QueryQot();
                query.rfqId = qotH.RfqId;
                query.vendorId = qotH.VendorId;
                query.matnrId = qotH.MatnrId;
                qotid = GetQotId(query);
            }
            /*20211227*/
            if (qotH.MEmptyFlag =="X") 
            {
                if(qotMaterials.Length>0) 
                {
                    errmsg = "A.材料已勾選不回填，無需填寫材料明細";
                    return errmsg;
                    //throw new Exception("A.材料已勾選不回填，無需填寫材料明細");
                }
            }
            if (qotH.PEmptyFlag == "X")
            {
                if (qotProcesses.Length > 0)
                {
                    //throw new Exception("B.加工已勾選不回填，無需填寫加工明細");
                    errmsg = "B.加工已勾選不回填，無需填寫加工明細";
                    return errmsg;
                }
            }
            if (qotH.SEmptyFlag == "X")
            {
                if (qotSurfaces.Length > 0)
                {
                    //throw new Exception("C.表面處理已勾選不回填，無需填寫表面處理明細");            
                    errmsg = "C.表面處理已勾選不回填，無需填寫表面處理明細";
                    return errmsg;
                }
            }
            if (qotH.OEmptyFlag == "X")
            {
                if (qotOthers.Length > 0)
                {
                    //throw new Exception("D.其他費用已勾選不回填，無需填寫其他費用明細");
                    errmsg = "D.其他費用已勾選不回填，無需填寫其他費用明細";
                    return errmsg;
                }
            }
            /*20211227*/
            //0825
            using (var db = new SRMContext())
            {
                var qot = new SrmQotH() { QotId = qotid };//qotH.QotId };
                db.SrmQotHs.Attach(qot);
                qot.LastUpdateDate = now;
                qot.LastUpdateBy = qotH.LastUpdateBy;
                qot.MEmptyFlag = qotH.MEmptyFlag;
                qot.PEmptyFlag = qotH.PEmptyFlag;
                qot.SEmptyFlag = qotH.SEmptyFlag;
                qot.OEmptyFlag = qotH.OEmptyFlag;
                qot.LeadTime = qotH.LeadTime;
                qot.ExpirationDate = qotH.ExpirationDate;
                qot.Note = qotH.Note;
                db.Entry(qot).Property(p => p.LastUpdateBy).IsModified = true;
                db.Entry(qot).Property(p => p.LastUpdateDate).IsModified = true;
                db.Entry(qot).Property(p => p.MEmptyFlag).IsModified = true;
                db.Entry(qot).Property(p => p.PEmptyFlag).IsModified = true;
                db.Entry(qot).Property(p => p.SEmptyFlag).IsModified = true;
                db.Entry(qot).Property(p => p.OEmptyFlag).IsModified = true;
                db.Entry(qot).Property(p => p.LeadTime).IsModified = true;
                db.Entry(qot).Property(p => p.ExpirationDate).IsModified = true;
                db.Entry(qot).Property(p => p.Note).IsModified = true;

                var result = db.SaveChanges();
                //return result;
            }
            //using (var db = new SRMContext())
            //{
            //db.Database.BeginTransaction();
            //try
            //{
            //var oldQotHs = _context.SrmQotHs.Where(r => r.QotId == qotH.QotId);


            //foreach (var oldQotH in oldQotHs)
            //{
            //    oldQotH.QotId = qotH.QotId;
            //    qotH.CreateBy = oldQotH.CreateBy;
            //    qotH.CreateDate = oldQotH.CreateDate;
            //    qotH.MatnrId = oldQotH.MatnrId;
            //    qotH.VendorId = oldQotH.VendorId;
            //    qotH.Status = oldQotH.Status;
            //}

            //初始才能修改

            //if (!_context.SrmQotHs.Any(r => r.RfqId == qotH.RfqId && r.Status == (int)Status.初始))
            //20211227 暫開失效
            if ((!_context.SrmQotHs.Any(r => r.RfqId == qotH.RfqId && r.Status == (int)Status.初始)) && (!_context.SrmQotHs.Any(r => r.RfqId == qotH.RfqId && r.Status == (int)Status.失效))) 
            {
                //throw new Exception("非初始無法修改");
                errmsg = "非初始無法修改";
                return errmsg;
            }
            //_context.SrmQotHs.Update(qotH);

            #region material
            foreach (var srmQotMaterial in qotMaterials)
            {
                srmQotMaterial.QotId = qotid;// qotH.QotId;
                if (srmQotMaterial.QotMId == 0)
                {
                    _context.SrmQotMaterial.Add(srmQotMaterial);
                }
                else
                {
                    _context.SrmQotMaterial.Update(srmQotMaterial);
                }
            }
            var oldQotMaterials = _context.SrmQotMaterial.Where(r => r.QotId == qotid);// qotH.QotId);
            foreach (var oldQotMaterial in oldQotMaterials)
            {
                if (qotMaterials.AsEnumerable().Where(item => item.QotMId == oldQotMaterial.QotMId).Count() == 0)
                {
                    _context.SrmQotMaterial.Remove(oldQotMaterial);
                }
            }
            #endregion
            #region surface
            foreach (var srmQotSurface in qotSurfaces)
            {
                srmQotSurface.QotId = qotid;// qotH.QotId;
                if (srmQotSurface.QotSId == 0)
                {
                    _context.SrmQotSurfaces.Add(srmQotSurface);
                }
                else
                {
                    _context.SrmQotSurfaces.Update(srmQotSurface);
                }
            }
            var oldqotSurfaces = _context.SrmQotSurfaces.Where(r => r.QotId == qotid);// qotH.QotId);
            foreach (var oldqotSurface in oldqotSurfaces)
            {
                if (qotSurfaces.AsEnumerable().Where(item => item.QotSId == oldqotSurface.QotSId).Count() == 0)
                {
                    _context.SrmQotSurfaces.Remove(oldqotSurface);
                }
            }
            #endregion
            #region process
            foreach (var srmQotProcess in qotProcesses)
            {
                srmQotProcess.QotId = qotid;// qotH.QotId;
                if (srmQotProcess.QotPId == 0)
                {
                    _context.SrmQotProcesses.Add(srmQotProcess);
                }
                else
                {
                    _context.SrmQotProcesses.Update(srmQotProcess);
                }
            }
            var oldqotProcesss = _context.SrmQotProcesses.Where(r => r.QotId == qotid);// qotH.QotId);
            foreach (var oldqotProcess in oldqotProcesss)
            {
                if (qotProcesses.AsEnumerable().Where(item => item.QotPId == oldqotProcess.QotPId).Count() == 0)
                {
                    _context.SrmQotProcesses.Remove(oldqotProcess);
                }
            }
            #endregion
            #region other
            foreach (var srmQotOther in qotOthers)
            {
                srmQotOther.QotId = qotid;// qotH.QotId;
                if (srmQotOther.QotOId == 0)
                {
                    _context.SrmQotOthers.Add(srmQotOther);
                }
                else
                {
                    _context.SrmQotOthers.Update(srmQotOther);
                }
            }
            var oldqotOthers = _context.SrmQotOthers.Where(r => r.QotId == qotid);// qotH.QotId);
            foreach (var oldqotOther in oldqotOthers)
            {
                if (qotOthers.AsEnumerable().Where(item => item.QotOId == oldqotOther.QotOId).Count() == 0)
                {
                    _context.SrmQotOthers.Remove(oldqotOther);
                }
            }
            #endregion
            _context.SaveChanges();
            return errmsg;
            //    db.Database.CommitTransaction();
            //}
            //catch (Exception ex)
            //{
            //    db.Database.RollbackTransaction();
            //    throw;
            //}
            //}
        }
        //public SrmQotH UpdateQotStatus(int status, SrmQotH qotH)
        //{
        //    var rfq = _srmRfqHRepository.Get(r => r.RfqId == rfqH.RfqId).First();
        //    switch ((Status)status)
        //    {
        //        case Status.啟動:
        //            if ((Status)rfq.Status != Status.初始)
        //            {
        //                throw new Exception($"非初始狀態無法{((Status)status).ToString()}");
        //            }
        //            break;
        //        case Status.作廢:
        //        case Status.刪除:
        //            if ((Status)rfq.Status != Status.初始 && (Status)rfq.Status != Status.啟動)
        //            {
        //                throw new Exception($"非初始或啟動狀態無法{((Status)status).ToString()}");
        //            }
        //            rfq.EndDate = DateTime.Now;
        //            rfq.EndBy = rfqH.EndBy;
        //            break;
        //        case Status.簽核中:
        //            if ((Status)rfq.Status != Status.確認 && (Status)rfq.Status != Status.簽核中 && (Status)rfq.Status != Status.已核發)
        //            {
        //                throw new Exception($"狀態異常無法{((Status)status).ToString()}");
        //            }
        //            if ((Status)rfq.Status == Status.已核發)
        //            {
        //                return rfq;
        //            }
        //            break;
        //        default:
        //            throw new Exception($"未定義{((Status)status).ToString()}");
        //            break;
        //    }
        //    rfq.Status = status;
        //    rfq.LastUpdateDate = rfqH.LastUpdateDate;
        //    rfq.LastUpdateBy = rfqH.LastUpdateBy;
        //    using (SRMContext db = new SRMContext())
        //    {
        //        db.Update(rfq);
        //        db.SaveChanges();
        //        return rfq;
        //    }
        //}
        public void InsertRejectReason(SrmQotUpdateMaterial qotH)
        {

            DateTime now = DateTime.Now;

            using (var db = new SRMContext())
            {
                var qot = new SrmQotH() { QotId = qotH.QotId };
                db.SrmQotHs.Attach(qot);
                qot.LastUpdateDate = now;
                qot.LastUpdateBy = qotH.LastUpdateBy;
                db.Entry(qot).Property(p => p.LastUpdateBy).IsModified = true;
                db.Entry(qot).Property(p => p.LastUpdateDate).IsModified = true;

                var qotmaterial = new SrmQotMaterial() { QotId = qotH.QotId };
                db.SrmQotMaterial.Attach(qotmaterial);
                qotmaterial.Note = "拒絕報價原因:" + qotH.reason;
                db.Entry(qotmaterial).Property(p => p.Note).IsModified = true;

                var result = db.SaveChanges();
                //return result;
            }
        }
        public int GetQotStatus(SrmQotH qotH)
        {
            var qotstatus = 0;
            var qot = _context.SrmQotHs.Where(p => p.QotId == qotH.QotId).ToList();
            //.AndIfCondition(query.status != 0, p => p.QSTATUS == query.status)
            //.AndIfHaveValue(query.matnr, p => p.MATNR == query.matnr)
            //.AndIfHaveValue(query.rfqno, p => p.RFQ_NUM == query.rfqno);
            qotstatus = qot.Select(r => r.Status).First().Value;

            return qotstatus;
        }
        #region
        public int GetRowNum(SrmQotH qotH)
        {
            int qotstatus = GetQotStatus(qotH);
            int index = -1;
            SrmQotH[] qs = _context.SrmQotHs.AsQueryable()
                .Where(p => p.RfqId == qotH.RfqId)
                .Where(p => p.VendorId == qotH.VendorId)
                //.Where(p =>p.Status == qotstatus)
                .OrderBy(p => p.QotId).ToArray();
            foreach (var q in qs)
            {
                index++;
                if (q.QotId == qotH.QotId)
                {

                    return index;
                }

            }
            //檢核該rfq的所有qot loop 檢核
            return index;
        }
        #endregion
        public SrmProcess[] GetProcess()
        {
            return _context.SrmProcesss.ToArray();
        }
        public SrmSurface[] GetSurface()
        {
            return _context.SrmSurfaces.ToArray();
        }

        public SrmMaterial[] GetMaterial()
        {
            return _context.SrmMaterials.ToArray();
        }
        public string GetProcessByNum(int num)
        {
            string processname = string.Empty;
            var p = _context.SrmProcesss.Where(p => p.ProcessNum == num).ToList();
            processname = p.Select(r => r.Process).First();

            return processname;
        }
        #region  檢核rfq、vendor所有初始的qot是否填寫完畢
        public IQueryable GetQotDataByRfqandVendor(int rfqid, int vendorid)
        {
            var qotlist = (from r in _context.SrmRfqHs
                           join q in _context.SrmQotHs on r.RfqId equals q.RfqId
                           join v in _context.SrmVendors on q.VendorId equals v.VendorId
                           select new
                           {
                               RfqNum = r.RfqNum,
                               QotId = q.QotId,
                               QotNum = q.QotNum,
                               RfqId = r.RfqId,
                               VendorId = q.VendorId,
                               mEmptyFlag = q.MEmptyFlag,
                               pEmptyFlag = q.PEmptyFlag,
                               sEmptyFlag = q.SEmptyFlag,
                               oPEmptyFlag = q.OEmptyFlag,
                               Status = q.Status
                           });

            qotlist = qotlist
            .Where(p => p.RfqId == rfqid)
            .Where(p => p.VendorId == vendorid)
            .Where(p => p.Status == 1)
            .OrderBy(p => p.QotId);
            foreach (var qot in qotlist)
            {
                //"X"表不回填
                if (qot.mEmptyFlag != "X")
                { }
            }

            return qotlist;
        }
        public bool CheckMaterialInfo(int qotid)
        {
            bool materialOK = true;
            var Query = (from m in _context.SrmQotMaterial
                         where m.QotId.Equals(qotid)
                         select m).ToList();
            if (Query.Count() == 0)
            {
                materialOK = false;
            }
            return materialOK;
        }
        public bool CheckProcessInfo(int qotid)
        {
            bool processOK = true;
            var Query = (from m in _context.SrmQotProcesses
                         where m.QotId.Equals(qotid)
                         select m).ToList();
            if (Query.Count() == 0)
            {
                processOK = false;
            }
            return processOK;
        }
        public bool CheckOtherInfo(int qotid)
        {
            bool otherOK = true;
            var Query = (from m in _context.SrmQotOthers
                         where m.QotId.Equals(qotid)
                         select m).ToList();
            if (Query.Count() == 0)
            {
                otherOK = false;
            }
            return otherOK;
        }
        public bool CheckSurfaceInfo(int qotid)
        {
            bool surfaceOK = true;
            var Query = (from m in _context.SrmQotSurfaces
                         where m.QotId.Equals(qotid)
                         select m).ToList();
            if (Query.Count() == 0)
            {
                surfaceOK = false;
            }
            return surfaceOK;
        }
        #endregion
        public int GetVendorId(JObject qot)
        {
            string name = qot["q"]["vendorId"].ToString();
            int vendorid = 0;
            var qotquery = (from v in _context.SrmVendors
                            where v.SrmVendor1 == name
                            select new
                            {
                                VendorId = v.VendorId,
                                SrmVendor = v.VendorName
                            });

            vendorid = qotquery.Select(r => r.VendorId).First();
            return vendorid;
        }
        #region 是否可整批送出
        /*public string SendAll(int rfqid, int vendorid) 
        {
            string msg = string.Empty;
            DateTime dtDate;
            bool materialOK = true;
            bool processOK = true;
            bool surfaceOK = true;
            bool otherOK = true;
            int qotid = 0;
            //先找出該rfq vendor 的 初始qot
            //檢查必填資料  LEAD_TIME EXPIRATION_DATE
            //檢核line
            var qotlist = (from r in _context.SrmRfqHs
                           join q in _context.SrmQotHs on r.RfqId equals q.RfqId
                           join v in _context.SrmVendors on q.VendorId equals v.VendorId
                           select new
                           {
                               RfqNum = r.RfqNum,
                               QotId = q.QotId,
                               QotNum = q.QotNum,
                               RfqId = r.RfqId,
                               VendorId = q.VendorId,
                               mEmptyFlag = q.MEmptyFlag,
                               pEmptyFlag = q.PEmptyFlag,
                               sEmptyFlag = q.SEmptyFlag,
                               oEmptyFlag = q.OEmptyFlag,
                               Status = q.Status,
                               LeadTime = q.LeadTime,
                               ExpirationDate= q.ExpirationDate
                           });

            qotlist = qotlist
            .Where(p => p.RfqId == rfqid)
            .Where(p => p.VendorId == vendorid)
            .Where(p => p.Status == 1)
            .OrderBy(p => p.QotId);
            var list = qotlist.Select(s => new { mEmptyFlag = s.mEmptyFlag, pEmptyFlag = s.pEmptyFlag, sEmptyFlag = s.sEmptyFlag, oEmptyFlag = s.oEmptyFlag
            ,LeadTime = s.LeadTime,
                ExpirationDate = s.ExpirationDate,
                QotId = s.QotId,
                QotNum = s.QotNum
            }).ToList();
            foreach (var qot in list)
            {
                qotid = qot.QotId;
                //???leadtime格式
                //if (!string.IsNullOrWhiteSpace(qot.LeadTime)) 
                //{
                //    msg += $"{qot.QotNum}計劃交貨時間未輸入"; 
                //}
                if (qot.ExpirationDate == null)
                {
                    msg += $"{qot.QotNum}有效期限未輸入";
                }

                if (DateTime.TryParse(qot.ExpirationDate.ToString(), out dtDate))
                {
                    msg += $" {qot.QotNum}:有效期限輸入錯誤";
                }              
                //"X"表不回填
                if (qot.mEmptyFlag != "X")
                {
                    materialOK = CheckMaterialInfo(qotid);
                    if (!materialOK)
                    {
                        msg += $" {qot.QotNum}:材料明細未輸入";
                    }
                }
                //"X"表不回填
                if (qot.pEmptyFlag != "X")
                {
                    processOK = CheckProcessInfo(qot.QotId);
                    if (!processOK)
                    {
                        msg += $" {qot.QotNum}:加工明細未輸入";
                    }
                }
                //"X"表不回填
                if (qot.sEmptyFlag != "X")
                {
                    surfaceOK = CheckSurfaceInfo(qot.QotId);
                    if (!surfaceOK)
                    {
                        msg += $" {qot.QotNum}:表面處理明細未輸入";
                    }
                }
                //"X"表不回填
                if (qot.oEmptyFlag != "X")
                {
                    otherOK = CheckOtherInfo(qot.QotId);
                    if (!otherOK)
                    {
                        msg += $" {qot.QotNum}:其他費用明細未輸入";
                    }
                }
            }            
            return msg;
        }*/
        #endregion 上傳下載
        #region 
        public string Upload(Model.Models.SRM.FileUploadViewModel_QOT fileUploadModel)
        {
            Guid g = Guid.NewGuid();
            var file = fileUploadModel.Files.First();
            var path = fileUploadModel.CurrentDirectory?.TrimEnd('/') + '/' + fileUploadModel.CreateBy + '/' + g + '_' + file.FileName;
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
        #region 表頭 
        public DataTable ReadExcel_QotH(string path, UserClaims user)
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
            //dt.Columns.Add("Unit");
            string[] headers = new string[] { "詢價單號", "料號", "短文", "[計畫交貨時間(日曆天)]", "有效期限","備註"};
            string[] cols = new string[] { "RfqNum", "Matnr", "Description", "LeadTime", "ExpirationDate" , "Note" };
            Dictionary<string, int> dtHeader = new Dictionary<string, int>();
            foreach (string header in headers)
            {
                if (!dt.Columns.Contains(header))
                {
                    throw new Exception($"【報價單表頭 】:格式錯誤，沒有欄位{header}");
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
                if (row.GetCell(dtHeader["詢價單號"]) != null)
                {
                    row.GetCell(dtHeader["詢價單號"]).SetCellType(CellType.String);
                    if (string.IsNullOrWhiteSpace(row.GetCell(dtHeader["詢價單號"]).StringCellValue)) { break; }
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
                        dataRow[h.Key] = row.GetCell(h.Value).StringCellValue;
                    }
                }
                #region 先檢查必填             
                if (!string.IsNullOrWhiteSpace(dataRow["詢價單號"].ToString()))
                {
                    if (string.IsNullOrWhiteSpace(dataRow["料號"].ToString()))
                    {
                        throw new Exception($"【報價單表頭 】:詢價單號:{dataRow["詢價單號"].ToString()}料號未填");
                    }
                    if (string.IsNullOrWhiteSpace(dataRow["短文"].ToString()))
                    {
                        throw new Exception($"【報價單表頭 】:料號:{dataRow["料號"].ToString()}短文未填");
                    }
                    if (string.IsNullOrWhiteSpace(dataRow["[計畫交貨時間(日曆天)]"].ToString()))
                    {
                        throw new Exception($"【報價單表頭 】:料號:{dataRow["料號"].ToString()}[計畫交貨時間(日曆天)]未填");
                    }
                    if (string.IsNullOrWhiteSpace(dataRow["有效期限"].ToString()))
                    {
                        throw new Exception($"【報價單表頭 】:料號:{dataRow["料號"].ToString()}有效期限未填");
                    }
                }
                #endregion
                //詢問LEO料號狀態先不卡
                //20211214 檢核該供應商是否有此單號、料號
                int checkdata = CheckMatnrData(user.UserName, dataRow["詢價單號"].ToString(), dataRow["料號"].ToString());
                if (checkdata == 0)
                {
                    dataRow["IsExists"] = false;
                    throw new Exception($"【報價單表頭 】:詢價單號:{dataRow["詢價單號"].ToString()}查無 料號:{dataRow["料號"].ToString()} 資訊");
                }
                else
                {
                    dataRow["IsExists"] = true;
                }
                float day = 0;
                DateTime ExperationDate = new DateTime();
                if (!float.TryParse(dataRow["[計畫交貨時間(日曆天)]"].ToString(), out day) || day <= 0)
                {
                    throw new Exception($"【報價單表頭 】:料號:{dataRow["料號"].ToString()}[計畫交貨時間(日曆天)]格式錯誤");
                }
                //???
                /**/
                if (!DateTime.TryParse(dataRow["有效期限"].ToString(), out ExperationDate))
                {
                    double d = 0;
                    if (!double.TryParse(dataRow["有效期限"].ToString(), out d))
                    {
                        throw new Exception($"【報價單表頭 】:料號:{dataRow["料號"].ToString()}有效期限格式錯誤");
                    }
                    ExperationDate = DateTime.FromOADate(Convert.ToDouble(dataRow["有效期限"].ToString()));
                }
                dataRow["有效期限"] = ExperationDate.ToString("yyyy/MM/dd");
                ///**/
                //if (!DateTime.TryParse(dataRow["有效期限"].ToString(), out ExperationDate))
                //{
                //    throw new Exception($"料號:{dataRow["料號"].ToString()}有效期限格式錯誤");
                //    //double d = 0;
                //    //if (!double.TryParse(dataRow["有效期限"].ToString(), out d))
                //    //{
                //    //    throw new Exception($"料號:{dataRow["料號"].ToString()}有效期限格式錯誤");
                //    //}
                //}
                //ExperationDate = DateTime.FromOADate(Convert.ToDouble(dataRow["有效期限"].ToString()));
                //dataRow["有效期限"] = ExperationDate.ToString("yyyy/MM/dd");
                //dataRow["IsExists"] = _context.SrmMatnrs.Any(r => r.SrmMatnr1.Equals(dataRow["料號"].ToString()) && user.Werks.Contains(r.Werks.Value));//2021/10/19問過LEO 同名字不同廠可能存在多筆
                dt.Rows.Add(dataRow);
                rowIndex++;
            }
            for (int i = 0; i < headers.Count(); i++)
            {
                dt.Columns[headers[i]].ColumnName = cols[i];
            }
            return dt;
        }
        #endregion 
        #region 材料
        public DataTable ReadExcel_Material(string path, UserClaims user)
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
            dt.Columns.Add("MCostPrice"); //小計
            string[] headers = new string[] { "詢價單號", "料號", "短文", "不回填", "素材材質", "長", "寬", "高", "密度", "重量", "材料單價", "備註" };
            string[] cols = new string[] { "RfqNum", "Matnr", "Description", "MEmptyFlag", "MMaterial", "Length", "Width", "Height", "Density", "Weight", "MPrice", "Note" };
            Dictionary<string, int> dtHeader = new Dictionary<string, int>();
            foreach (string header in headers)
            {
                if (!dt.Columns.Contains(header))
                {
                    throw new Exception($"【材料 】:格式錯誤，沒有欄位{header}");
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
                if (row.GetCell(dtHeader["詢價單號"]) != null)
                {
                    row.GetCell(dtHeader["詢價單號"]).SetCellType(CellType.String);
                    if (string.IsNullOrWhiteSpace(row.GetCell(dtHeader["詢價單號"]).StringCellValue)) { break; }
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
                        dataRow[h.Key] = row.GetCell(h.Value).StringCellValue;
                    }
                }

                #region 先檢查必填 詢價單、料號、短文、素材材質、重量、材料單價
                if (!string.IsNullOrWhiteSpace(dataRow["詢價單號"].ToString()))
                {
                    if (string.IsNullOrWhiteSpace(dataRow["料號"].ToString()))
                    {
                        throw new Exception($"【材料 】:詢價單號:{dataRow["詢價單號"].ToString()}料號未填");
                    }
                    if (string.IsNullOrWhiteSpace(dataRow["短文"].ToString()))
                    {
                        throw new Exception($"【材料 】:料號:{dataRow["料號"].ToString()}短文未填");
                    }

                    //if (string.IsNullOrWhiteSpace(dataRow["不回填"].ToString()))
                    //沒有選擇不回填
                    if (dataRow["不回填"].ToString().ToUpper() != "Y")
                    {
                        if (string.IsNullOrWhiteSpace(dataRow["素材材質"].ToString()))
                        {
                            throw new Exception($"【材料 】:料號:{dataRow["料號"].ToString()}素材材質未填");
                        }
                        if (string.IsNullOrWhiteSpace(dataRow["重量"].ToString()))
                        {
                            throw new Exception($"【材料 】:料號:{dataRow["料號"].ToString()}重量未填");
                        }
                        if (string.IsNullOrWhiteSpace(dataRow["材料單價"].ToString()))
                        {
                            throw new Exception($"【材料 】:料號:{dataRow["料號"].ToString()}材料單價未填");
                        }
                    }
                    //20211227 
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(dataRow["素材材質"].ToString()))
                        {
                            throw new Exception($"【材料 】:料號:{dataRow["料號"].ToString()}，已選擇不回填，無需填寫材料明細");
                        }
                        if (!string.IsNullOrWhiteSpace(dataRow["重量"].ToString()))
                        {
                            throw new Exception($"【材料 】:料號:{dataRow["料號"].ToString()}，已選擇不回填，無需填寫材料明細");
                        }
                        if (!string.IsNullOrWhiteSpace(dataRow["材料單價"].ToString()))
                        {
                            throw new Exception($"【材料 】:料號:{dataRow["料號"].ToString()}，已選擇不回填，無需填寫材料明細");
                        }
                    }
                }
                #endregion
                //20211214 檢核該供應商是否有此單號、料號
                int checkdata = CheckMatnrData(user.UserName, dataRow["詢價單號"].ToString(), dataRow["料號"].ToString());
                if (checkdata == 0)
                {
                    dataRow["IsExists"] = false;
                    throw new Exception($"【材料 】:詢價單號:{dataRow["詢價單號"].ToString()}查無 料號:{dataRow["料號"].ToString()} 資訊");
                }
                else
                {
                    dataRow["IsExists"] = true;
                }
                float weight = 0;
                float price = 0;

                //檢核素材材質
                if ((string.IsNullOrWhiteSpace(dataRow["不回填"].ToString())) || (dataRow["不回填"].ToString().ToUpper() == "N"))
                {
                    if (!_context.SrmMaterials.Any(r => r.Material.Equals(dataRow["素材材質"].ToString())))
                    {
                        throw new Exception($"【材料 】:素材材質:{dataRow["素材材質"].ToString()}不存在");
                    }
                    if (!float.TryParse(dataRow["重量"].ToString(), out weight) || weight <= 0)
                    {
                        throw new Exception($"【材料 】:料號:{dataRow["料號"].ToString()}重量格式錯誤");
                    }
                    else
                    {
                        dataRow["重量"] = float.Parse(Math.Round(Convert.ToDecimal(dataRow["重量"].ToString()), 2, MidpointRounding.AwayFromZero).ToString());
                    }
                    if (!float.TryParse(dataRow["材料單價"].ToString(), out price) || price <= 0)
                    {
                        throw new Exception($"【材料 】:料號:{dataRow["料號"].ToString()}材料單價格式錯誤");
                    }
                    else
                    {
                        dataRow["材料單價"] = float.Parse(Math.Round(Convert.ToDecimal(dataRow["材料單價"].ToString()), 2, MidpointRounding.AwayFromZero).ToString());
                    }
                    //新增.
                    dataRow["MCostPrice"] = float.Parse(Math.Round(Convert.ToDecimal((float.Parse(dataRow["重量"].ToString())) * (float.Parse(dataRow["材料單價"].ToString()))), 2, MidpointRounding.AwayFromZero).ToString());
                }
                
                
                dt.Rows.Add(dataRow);
                rowIndex++;
            }
            for (int i = 0; i < headers.Count(); i++)
            {
                dt.Columns[headers[i]].ColumnName = cols[i];
            }
            return dt;
        }
        #endregion
        #region 加工
        public DataTable ReadExcel_Process(string path, UserClaims user)
        {
            IWorkbook workbook;
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(stream);
            }

            ISheet sheet = workbook.GetSheetAt(2); // zero-based index of your target sheet
            DataTable dt = new DataTable(sheet.SheetName);

            // write header row
            IRow headerRow = sheet.GetRow(0);
            for (int i = 0; i < headerRow.Cells.Count; i++)
            {
                headerRow.GetCell(i).SetCellType(CellType.String);
                dt.Columns.Add(headerRow.GetCell(i).StringCellValue);
            }
            dt.Columns.Add("IsExists");
            dt.Columns.Add("PCostsum"); //小計
            dt.Columns.Add("PProcess"); //工序名稱
            string[] headers = new string[] { "詢價單號", "料號", "短文", "不回填", "工序", "[工時(時)]", "[單價(時)]", "機台", "備註" };
            string[] cols = new string[] { "RfqNum", "Matnr", "Description", "PEmptyFlag", "PProcessNum", "PHours", "PPrice", "PMachine", "PNote" };
            Dictionary<string, int> dtHeader = new Dictionary<string, int>();
            foreach (string header in headers)
            {
                if (!dt.Columns.Contains(header))
                {
                    throw new Exception($"【加工 】:格式錯誤，沒有欄位{header}");
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
                if (row.GetCell(dtHeader["詢價單號"]) != null)
                {
                    row.GetCell(dtHeader["詢價單號"]).SetCellType(CellType.String);
                    if (string.IsNullOrWhiteSpace(row.GetCell(dtHeader["詢價單號"]).StringCellValue)) { break; }
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
                        dataRow[h.Key] = row.GetCell(h.Value).StringCellValue;
                    }
                }

                #region 先檢查必填 詢價單、料號、短文、素材材質、重量、材料單價
                if (!string.IsNullOrWhiteSpace(dataRow["詢價單號"].ToString()))
                {
                    if (string.IsNullOrWhiteSpace(dataRow["料號"].ToString()))
                    {
                        throw new Exception($"【加工 】:詢價單號:{dataRow["詢價單號"].ToString()}料號未填");
                    }
                    if (string.IsNullOrWhiteSpace(dataRow["短文"].ToString()))
                    {
                        throw new Exception($"【加工 】:料號:{dataRow["料號"].ToString()}短文未填");
                    }
                    //if (string.IsNullOrWhiteSpace(dataRow["不回填"].ToString()))
                    if (dataRow["不回填"].ToString().ToUpper() != "Y")
                    {
                        if (string.IsNullOrWhiteSpace(dataRow["工序"].ToString()))
                        {
                            throw new Exception($"【加工 】:料號:{dataRow["料號"].ToString()}工序未填");
                        }
                        if (string.IsNullOrWhiteSpace(dataRow["[工時(時)]"].ToString()))
                        {
                            throw new Exception($"【加工 】:料號:{dataRow["料號"].ToString()}[工時(時)]未填");
                        }
                        if (string.IsNullOrWhiteSpace(dataRow["[單價(時)]"].ToString()))
                        {
                            throw new Exception($"【加工 】:料號:{dataRow["料號"].ToString()}[單價(時)]未填");
                        }
                    }
                    //20211227 新增卡控
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(dataRow["工序"].ToString()))
                        {
                            throw new Exception($"【加工 】:料號:{dataRow["料號"].ToString()}，已選擇不回填，無需填寫加工明細");
                        }
                        if (!string.IsNullOrWhiteSpace(dataRow["[工時(時)]"].ToString()))
                        {
                            throw new Exception($"【加工 】:料號:{dataRow["料號"].ToString()}，已選擇不回填，無需填寫加工明細");
                        }
                        if (!string.IsNullOrWhiteSpace(dataRow["[單價(時)]"].ToString()))
                        {
                            throw new Exception($"【加工 】:料號:{dataRow["料號"].ToString()}，已選擇不回填，無需填寫加工明細");
                        }
                    }
                }
               
                #endregion
                //20211214 檢核該供應商是否有此單號、料號
                int checkdata = CheckMatnrData(user.UserName, dataRow["詢價單號"].ToString(), dataRow["料號"].ToString());
                if (checkdata == 0)
                {
                    dataRow["IsExists"] = false;
                    throw new Exception($"【加工 】:詢價單號:{dataRow["詢價單號"].ToString()}查無 料號:{dataRow["料號"].ToString()} 資訊");
                }
                else 
                {
                    dataRow["IsExists"] = true;
                }
                //檢核素材材質
                //20211214
                float hour = 0;
                float price = 0;
                if ((string.IsNullOrWhiteSpace(dataRow["不回填"].ToString())) || (dataRow["不回填"].ToString().ToUpper() == "N")) 
                {
                    if (!_context.SrmProcesss.Any(r => r.Process.Equals(dataRow["工序"].ToString())))
                    {
                        throw new Exception($"【加工 】:工序:{dataRow["工序"].ToString()}不存在");
                    }
                    else
                    {
                        //寫入工序NUM
                        dataRow["PProcess"] = dataRow["工序"].ToString();
                        int processnum = GetProcessNum(dataRow["工序"].ToString());
                        dataRow["工序"] = processnum;
                    }

                    if (!float.TryParse(dataRow["[工時(時)]"].ToString(), out hour) || hour <= 0)
                    {
                        throw new Exception($"【加工 】:料號:{dataRow["料號"].ToString()}[工時(時)]格式錯誤");
                    }
                    else
                    {
                        dataRow["[工時(時)]"] = float.Parse(Math.Round(Convert.ToDecimal(dataRow["[工時(時)]"].ToString()), 2, MidpointRounding.AwayFromZero).ToString());
                    }
                    if (!float.TryParse(dataRow["[單價(時)]"].ToString(), out price) || price <= 0)
                    {
                        throw new Exception($"【加工 】:料號:{dataRow["料號"].ToString()}[單價(時)]格式錯誤");
                    }
                    else
                    {
                        dataRow["[單價(時)]"] = float.Parse(Math.Round(Convert.ToDecimal(dataRow["[單價(時)]"].ToString()), 2, MidpointRounding.AwayFromZero).ToString());
                    }
                    //新增.
                    dataRow["PCostsum"] = float.Parse(Math.Round(Convert.ToDecimal((float.Parse(dataRow["[工時(時)]"].ToString())) * (float.Parse(dataRow["[單價(時)]"].ToString()))), 2, MidpointRounding.AwayFromZero).ToString());
                }

                dt.Rows.Add(dataRow);
                rowIndex++;
            }
            for (int i = 0; i < headers.Count(); i++)
            {
                dt.Columns[headers[i]].ColumnName = cols[i];
            }
            return dt;
        }
        #endregion
        #region 表面處理
        public DataTable ReadExcel_Surface(string path, UserClaims user)
        {
            IWorkbook workbook;
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(stream);
            }

            ISheet sheet = workbook.GetSheetAt(3); // zero-based index of your target sheet
            DataTable dt = new DataTable(sheet.SheetName);

            // write header row
            IRow headerRow = sheet.GetRow(0);
            for (int i = 0; i < headerRow.Cells.Count; i++)
            {
                headerRow.GetCell(i).SetCellType(CellType.String);
                dt.Columns.Add(headerRow.GetCell(i).StringCellValue);
            }
            dt.Columns.Add("IsExists");
            dt.Columns.Add("SCostsum"); //小計
            dt.Columns.Add("SProcessDesc"); //工序名稱
            string[] headers = new string[] { "詢價單號", "料號", "短文", "不回填", "工序", "[單價(時)]", "次數", "備註" };
            string[] cols = new string[] { "RfqNum", "Matnr", "Description", "SEmptyFlag", "SProcess", "SPrice",  "STimes", "PNote" };
            Dictionary<string, int> dtHeader = new Dictionary<string, int>();
            foreach (string header in headers)
            {
                if (!dt.Columns.Contains(header))
                {
                    throw new Exception($"【表面處理 】:格式錯誤，沒有欄位{header}");
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
                if (row.GetCell(dtHeader["詢價單號"]) != null)
                {
                    row.GetCell(dtHeader["詢價單號"]).SetCellType(CellType.String);
                    if (string.IsNullOrWhiteSpace(row.GetCell(dtHeader["詢價單號"]).StringCellValue)) { break; }
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
                        dataRow[h.Key] = row.GetCell(h.Value).StringCellValue;
                    }
                }

                #region 先檢查必填 詢價單、料號、短文、工序、[單價(時)]、次數
                if (!string.IsNullOrWhiteSpace(dataRow["詢價單號"].ToString()))
                {
                    if (string.IsNullOrWhiteSpace(dataRow["料號"].ToString()))
                    {
                        throw new Exception($"【表面處理 】:詢價單號:{dataRow["詢價單號"].ToString()}料號未填");
                    }
                    if (string.IsNullOrWhiteSpace(dataRow["短文"].ToString()))
                    {
                        throw new Exception($"【表面處理 】:料號:{dataRow["料號"].ToString()}短文未填");
                    }
                    //if(string.IsNullOrWhiteSpace(dataRow["不回填"].ToString()))
                    if (dataRow["不回填"].ToString().ToUpper() != "Y")
                    {
                        if (string.IsNullOrWhiteSpace(dataRow["工序"].ToString()))
                        {
                            throw new Exception($"【表面處理 】:料號:{dataRow["料號"].ToString()}工序未填");
                        }
                        if (string.IsNullOrWhiteSpace(dataRow["[單價(時)]"].ToString()))
                        {
                            throw new Exception($"【表面處理 】:料號:{dataRow["料號"].ToString()}[單價(時)]未填");
                        }
                        if (string.IsNullOrWhiteSpace(dataRow["次數"].ToString()))
                        {
                            throw new Exception($"【表面處理 】:料號:{dataRow["料號"].ToString()}次數未填");
                        }
                    }     
                    //20211227
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(dataRow["工序"].ToString()))
                        {
                            throw new Exception($"【表面處理 】:料號:{dataRow["料號"].ToString()}，已選擇不回填，無需填寫表面處理明細");
                        }
                        if (!string.IsNullOrWhiteSpace(dataRow["[單價(時)]"].ToString()))
                        {
                            throw new Exception($"【表面處理 】:料號:{dataRow["料號"].ToString()}，已選擇不回填，無需填寫表面處理明細");
                        }
                        if (!string.IsNullOrWhiteSpace(dataRow["次數"].ToString()))
                        {
                            throw new Exception($"【表面處理 】:料號:{dataRow["料號"].ToString()}，已選擇不回填，無需填寫表面處理明細");
                        }
                    }
                }
                #endregion
                //20211214
                float times = 0;
                float price = 0;
                int checkdata = CheckMatnrData(user.UserName, dataRow["詢價單號"].ToString(), dataRow["料號"].ToString());
                if (checkdata == 0)
                {
                    dataRow["IsExists"] = false;
                    throw new Exception($"【表面處理 】:詢價單號:{dataRow["詢價單號"].ToString()}查無 料號:{dataRow["料號"].ToString()} 資訊");
                }
                else 
                {
                    dataRow["IsExists"] = true;
                }

                if ((string.IsNullOrWhiteSpace(dataRow["不回填"].ToString())) || (dataRow["不回填"].ToString().ToUpper() == "N")) 
                {
                    if (!_context.SrmSurfaces.Any(r => r.SurfaceDesc.Equals(dataRow["工序"].ToString().Trim())))
                    {
                        throw new Exception($"【表面處理 】:工序:{dataRow["工序"].ToString()}不存在");
                    }
                    else
                    {
                        //寫入工序NUM
                        dataRow["SProcessDesc"] = dataRow["工序"].ToString();
                        int surfaceid = GetSurfaceNum(dataRow["工序"].ToString());
                        dataRow["工序"] = surfaceid;
                    }
                    if (!float.TryParse(dataRow["[單價(時)]"].ToString(), out price) || price <= 0)
                    {
                        throw new Exception($"【表面處理 】:料號:{dataRow["料號"].ToString()}[單價(時)]格式錯誤");
                    }
                    else
                    {
                        dataRow["[單價(時)]"] = float.Parse(Math.Round(Convert.ToDecimal(dataRow["[單價(時)]"].ToString()), 2, MidpointRounding.AwayFromZero).ToString());
                    }
                    if (!float.TryParse(dataRow["次數"].ToString(), out times) || times <= 0)
                    {
                        throw new Exception($"【表面處理 】:料號:{dataRow["料號"].ToString()}次數格式錯誤");
                    }
                    else
                    {
                        dataRow["次數"] = float.Parse(Math.Round(Convert.ToDecimal(dataRow["次數"].ToString()), 2, MidpointRounding.AwayFromZero).ToString());
                    }
                    //新增.
                    dataRow["SCostsum"] = float.Parse(Math.Round(Convert.ToDecimal((float.Parse(dataRow["[單價(時)]"].ToString())) * (float.Parse(dataRow["次數"].ToString()))), 2, MidpointRounding.AwayFromZero).ToString());
                }
                dt.Rows.Add(dataRow);
                rowIndex++;
            }
            for (int i = 0; i < headers.Count(); i++)
            {
                dt.Columns[headers[i]].ColumnName = cols[i];
            }
            return dt;
        }
        #endregion

        #region 其他
        public DataTable ReadExcel_Other(string path, UserClaims user)
        {
            IWorkbook workbook;
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(stream);
            }

            ISheet sheet = workbook.GetSheetAt(4); // zero-based index of your target sheet
            DataTable dt = new DataTable(sheet.SheetName);

            // write header row
            IRow headerRow = sheet.GetRow(0);
            for (int i = 0; i < headerRow.Cells.Count; i++)
            {
                headerRow.GetCell(i).SetCellType(CellType.String);
                dt.Columns.Add(headerRow.GetCell(i).StringCellValue);
            }
            dt.Columns.Add("IsExists");
            dt.Columns.Add("SCostsum"); //小計
            string[] headers = new string[] { "詢價單號", "料號", "短文", "不回填", "項目", "單價", "說明", "備註" };
            string[] cols = new string[] { "RfqNum", "Matnr", "Description", "OEmptyFlag", "OItem", "OPrice", "ODescription", "ONote" };
            Dictionary<string, int> dtHeader = new Dictionary<string, int>();
            foreach (string header in headers)
            {
                if (!dt.Columns.Contains(header))
                {
                    throw new Exception($"【其他 】:格式錯誤，沒有欄位{header}");
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
                if (row.GetCell(dtHeader["詢價單號"]) != null)
                {
                    row.GetCell(dtHeader["詢價單號"]).SetCellType(CellType.String);
                    if (string.IsNullOrWhiteSpace(row.GetCell(dtHeader["詢價單號"]).StringCellValue)) { break; }
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
                        dataRow[h.Key] = row.GetCell(h.Value).StringCellValue;
                    }
                }

                #region 先檢查必填 詢價單、料號、短文、工序、[單價(時)]、次數
                if (!string.IsNullOrWhiteSpace(dataRow["詢價單號"].ToString()))
                {
                    if (string.IsNullOrWhiteSpace(dataRow["料號"].ToString()))
                    {
                        throw new Exception($"【其他 】:詢價單號:{dataRow["詢價單號"].ToString()}料號未填");
                    }
                    if (string.IsNullOrWhiteSpace(dataRow["短文"].ToString()))
                    {
                        throw new Exception($"【其他 】:料號:{dataRow["料號"].ToString()}短文未填");
                    }
                    //if(string.IsNullOrWhiteSpace(dataRow["不回填"].ToString()))
                    if (dataRow["不回填"].ToString().ToUpper() != "Y")
                    {
                        if (string.IsNullOrWhiteSpace(dataRow["項目"].ToString()))
                        {
                            throw new Exception($"【其他 】:料號:{dataRow["料號"].ToString()}項目未填");
                        }
                        if (string.IsNullOrWhiteSpace(dataRow["單價"].ToString()))
                        {
                            throw new Exception($"【其他 】:料號:{dataRow["料號"].ToString()}單價未填");
                        }
                    } 
                    //20211227
                    else
                    {
                        
                        if (!string.IsNullOrWhiteSpace(dataRow["項目"].ToString()))
                        {
                            throw new Exception($"【其他 】:料號:{dataRow["料號"].ToString()}，已選擇不回填，無需填寫其他費用明細");
                        }
                        if (!string.IsNullOrWhiteSpace(dataRow["單價"].ToString()))
                        {
                            throw new Exception($"【其他 】:料號:{dataRow["料號"].ToString()}，已選擇不回填，無需填寫其他費用明細");
                        }
                    }
                }
                #endregion
                int checkdata = CheckMatnrData(user.UserName, dataRow["詢價單號"].ToString(), dataRow["料號"].ToString());
                if (checkdata == 0)
                {
                    dataRow["IsExists"] = false;
                    throw new Exception($"【其他 】:詢價單號:{dataRow["詢價單號"].ToString()}查無 料號:{dataRow["料號"].ToString()} 資訊");
                }
                else
                {
                    dataRow["IsExists"] = true;
                }

                float price = 0;
                if ((string.IsNullOrWhiteSpace(dataRow["不回填"].ToString())) || (dataRow["不回填"].ToString().ToUpper() == "N")) 
                {
                    if (!float.TryParse(dataRow["單價"].ToString(), out price) || price <= 0)
                    {
                        throw new Exception($"【其他 】:料號:{dataRow["料號"].ToString()}單價格式錯誤");
                    }
                    else
                    {
                        dataRow["單價"] = float.Parse(Math.Round(Convert.ToDecimal(dataRow["單價"].ToString()), 2, MidpointRounding.AwayFromZero).ToString());
                    }
                }
                dt.Rows.Add(dataRow);
                rowIndex++;
            }
            for (int i = 0; i < headers.Count(); i++)
            {
                dt.Columns[headers[i]].ColumnName = cols[i];
            }
            return dt;
        }
        #endregion

        #region 檢核供應商是否有此料號、此報價單號
        public int  CheckMatnrData(string vendor,string RfqNum,string Matnr) 
        {
            int qotid = 0;
            var Query = (from q in _context.SrmQotHs
                         join r in _context.SrmRfqHs on q.RfqId equals r.RfqId
                         join rm in _context.SrmRfqMs on new { RfqId = q.RfqId, MatnrId = q.MatnrId } equals new { RfqId = rm.RfqId, MatnrId = rm.MatnrId }
                         join m in _context.SrmMatnrs on rm.MatnrId equals m.MatnrId
                         join v in _context.SrmVendors on q.VendorId equals v.VendorId
                         //where r.RfqNum.Equals(RfqNum)
                         select new ViewSrmQotList
                         {
                             RFQ_NUM = r.RfqNum,
                             VENDOR = (!string.IsNullOrWhiteSpace(v.SapVendor)) ? v.SapVendor : v.SrmVendor1,
                             MATNR =(!string.IsNullOrWhiteSpace(m.SapMatnr)) ? m.SapMatnr :m.SrmMatnr1,
                             QOT_ID = q.QotId
                         })
                         .AndIfHaveValue(vendor, p => p.VENDOR == vendor)
                         .AndIfHaveValue(RfqNum, p => p.RFQ_NUM == RfqNum)
                         .AndIfHaveValue(Matnr, p => p.MATNR == Matnr);
       
            if (Query.Count() > 0)
            {
                qotid = Query.Select(r => r.QOT_ID).First();
            }
            return qotid;
        }
        #endregion
        #endregion
        private int GetProcessNum(string Process)
        {
            var processid = 0;
            var process = (from p in _context.SrmProcesss
                          where (p.Process == Process)
                          select new
                          {
                              ProcessNum = p.ProcessNum,
                          });
            processid = process.Select(r => r.ProcessNum).First();
            return processid;

        }
        private int GetSurfaceNum(string Process)
        {
            var surfaceid = 0;
            var surface = (from s in _context.SrmSurfaces
                           where (s.SurfaceDesc == Process)
                           select new
                           {
                               surfaceid = s.SurfaceId,
                           });
            surfaceid = surface.Select(r => r.surfaceid).First();
            return surfaceid;
        }
        public void Delete(string path)
        {
            try
            {
                FileInfo f = new FileInfo(path);
                if (f.Exists)
                {
                    f.Delete();
                }
            }
            catch (Exception ex)
            {
            }
        }
        public IQueryable GetQotInfo(int rfqid, int vendorid, int qotid)
        {
          
            var qotlist = (from r in _context.SrmRfqHs
                           join q in _context.SrmQotHs on r.RfqId equals q.RfqId
                           join rm in _context.SrmRfqMs on new { RfqId = q.RfqId, MatnrId = q.MatnrId } equals new { RfqId = rm.RfqId, MatnrId = rm.MatnrId }
                           join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
                           join mu in _context.SrmMeasureUnits on rm.Unit equals mu.MeasureId into mug
                           from mu in mug.DefaultIfEmpty()
                           join s in _context.SrmStatuses on q.Status equals s.Status
                           join u1 in _context.AspNetUsers on q.CreateBy equals u1.UserName into u1g
                           from u1 in u1g.DefaultIfEmpty()
                           select new
                           {
                               RfqNum = r.RfqNum,
                               Matnr = (!string.IsNullOrWhiteSpace(m.SapMatnr)) ? m.SapMatnr : m.SrmMatnr1,
                               Desc = m.Description,
                               mEmptyFlag = q.MEmptyFlag,
                               pEmptyFlag = q.PEmptyFlag,
                               sEmptyFlag = q.SEmptyFlag,
                               oPEmptyFlag = q.OEmptyFlag,
                               CreateBy = (!string.IsNullOrWhiteSpace(u1.Name)) ? u1.Name : q.CreateBy,//q.CreateBy,
                               CreateDate = DateTime.Parse(q.CreateDate.ToString()).ToString("yyyy/MM/dd HH:mm:ss"),
                               Status = s.StatusDesc,
                               QotId = q.QotId,
                               QotNum = q.QotNum,
                               //Status = q.Status.HasValue ? ((Status)q.Status).ToString() : "",
                               Material = rm.Material,
                               Weight = rm.Weight,
                               MachineName = rm.MachineName,
                               Size = rm.Length + "*" + rm.Width + "*" + rm.Height,
                               Length = rm.Length,
                               Width = rm.Width,
                               Height = rm.Height,
                               RfqId = r.RfqId,
                               VendorId = q.VendorId,
                              
                               Description = m.Description,
                               Qty = rm.Qty,
                               Expiringdate = q.ExpirationDate,
                               Leadtime = q.LeadTime,
                               estDeliveryDate = (!string.IsNullOrWhiteSpace(rm.EstDeliveryDate.ToString())) ? DateTime.Parse(rm.EstDeliveryDate.ToString()).ToString("yyyy/MM/dd") : "",
                               Note = q.Note,
                               Unit = mu.MeasureDesc,//20211203
                           });
            //.AndIfCondition(query.status != 0, p => p.QSTATUS == query.status)
            //.AndIfHaveValue(matnrid, p => p.MATNR == query.matnr).t
            //.AndIfHaveValue(query.rfqno, p => p.RFQ_NUM == query.rfqno);
            qotlist = qotlist
            .Where(p => p.RfqId == rfqid)
            .Where(p => p.VendorId == vendorid)
            .Where(p => p.QotId == qotid);
           
            //.AndIfHaveValue(matnrid , p => p.QotId == qotid);
            //.Where(p => p.QotId == qotid);
            return qotlist;
        }
        public int GetMatnrId(QueryQot query)
        {
            var matnrid = 0;
            var matnr = (from m in _context.SrmMatnrs
                       select new
                       {
                           MatnrId = m.MatnrId,
                           Matnr = (!string.IsNullOrWhiteSpace(m.SapMatnr))? m.SapMatnr:m.SrmMatnr1
                       });
            matnr = matnr
           .Where(p => p.Matnr == query.matnr);
            matnrid = matnr.Select(r => r.MatnrId).First();
            return matnrid;
        }
        public string GetSurfaceProcessByNum(int num)
        {
            string processname = string.Empty;
            var p = _context.SrmSurfaces.Where(p => p.SurfaceId == num).ToList();
            processname = p.Select(r => r.SurfaceDesc).First();

            return processname;
        }
        public SrmQotH GetQotById(QueryQot query)
        {
            var qot = _context.SrmQotHs.AsQueryable()
                .AndIfHaveValue(query.qotId, r => r.QotId == query.qotId).ToList().First();
            return qot;
        }
        public HttpResponseMessage GetFile(string fileName, string localFilePath)
        {
            fileName = @"123.txt";
            localFilePath = @"D:\Excel\RFQ0000213批次報價.xlsx";

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(localFilePath, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileName;
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/excel");

            //byte[] excelData = File.ReadAllBytes(path); result.Content = new StreamContent(excelData);
            //byte[] excelData = File.ReadAllBytes(localFilePath);
            //MemoryStream stream = new MemoryStream(excelData);
            //response.Content = new StreamContent(stream);

            //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            //HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //var stream = new FileStream(localFilePath, FileMode.Open);
            //result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            //{
            //    FileName = fileName
            //};
            //result.Content.Headers.ContentLength = stream.Length;
            ////tried with "application/ms-excel" also
            //result.Content.Headers.ContentType =
            //    new MediaTypeHeaderValue("application/octet-stream");
            //return result;

            return response;
        }
       
    }
}
