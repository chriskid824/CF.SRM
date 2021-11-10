using AutoMapper;

using Convience.Entity.Data;
using Convience.Entity.Entity;
using Convience.Entity.Entity.Identity;
using Convience.Entity.Entity.SRM;
using Convience.EntityFrameWork.Repositories;
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
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
        public void Save(SrmQotH qotH, SrmQotMaterial[] qotMaterials, SrmQotSurface[] qotSurfaces, SrmQotProcess[] qotProcesses, SrmQotOther[] qotOthers);
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

            if ((Status)qot.Status != Status.初始)
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
                              VVendor = vendor.VendorName

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
            return result;

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
                          join q in _context.SrmQotHs on r.RfqId equals q.RfqId
                          join status in _context.SrmStatuses on r.Status equals status.Status
                          //join vendor in _context.SrmVendors on q.VendorId equals vendor.VendorId
                          join u1 in _context.AspNetUsers on r.CreateBy equals u1.UserName
                          join u2 in _context.AspNetUsers on r.LastUpdateBy equals u2.UserName
                          select new ViewQotListH
                          {
                              VRfqId = r.RfqId,
                              VRfqNum = r.RfqNum,
                              VStatus = r.Status,
                              VStatusDesc = status.StatusDesc,
                              VCreateDate = r.CreateDate,
                              VCreateBy = u1.Name,
                              VLastUpdateDate = r.LastUpdateDate,
                              VLastUpdateBy = u2.Name,
                              VEndDate = r.EndDate,
                              //VVendor = vendor.SapVendor,


                          })
                        //.Where(p => p.VVendor == query.vendor)
                        .Where(p => p.VStatus == 7)
                            //.Where()
                            //.AndIfCondition(query.status != 0, p => p.Status == 7)
                            //.AndIfCondition(!string.IsNullOrWhiteSpace(query.vendor), p => p.VVendor == query.vendor)
                            .AndIfCondition(!string.IsNullOrWhiteSpace(query.rfqno), p => p.VRfqNum == query.rfqno)
                        .Distinct()
                        .ToList();

            result.ForEach(p =>
            {
                p.SrmQotHs = (from q in _context.SrmQotHs

                              join r in _context.SrmRfqHs on q.RfqId equals r.RfqId
                              join status in _context.SrmStatuses on q.Status equals status.Status
                              join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
                              join u3 in _context.AspNetUsers on r.CreateBy equals u3.UserName
                              join u4 in _context.AspNetUsers on r.LastUpdateBy equals u4.UserName
                              //join v in _context.SrmVendors on q.VendorId equals v.VendorId
                              select new ViewQotListL
                              {
                                  QRfqId = q.RfqId,
                                  QStatus = q.Status,
                                  QQotId = q.QotId,
                                  QQotNum = q.QotNum,
                                  QMatnr = (!string.IsNullOrWhiteSpace(m.SapMatnr)) ? m.SapMatnr : m.SrmMatnr1,
                                  QCreateBy = u3.Name,
                                  QCreateDate = q.CreateDate,
                                  QLastUpdateBy = u4.Name,
                                  QLastUpdateDate = q.LastUpdateDate,
                                  QVendorId = q.VendorId,
                                  //QVendor = v.SapVendor,
                                  QStatusDesc = status.StatusDesc
                              })
                              //.ToList();
                              //.Where(p => p.QVendorId.Value == query.vendor)

                              //.Where(p => p.QVendor == query.vendor) //供應商登入帳號為供應商代碼
                              .Where(l => l.QRfqId.Value == p.VRfqId)

                              .AndIfCondition(!string.IsNullOrWhiteSpace(query.matnr), p => p.QMatnr == query.matnr)
                              //.AndIfCondition(query.status != 0, p => p.QStatus.Value == query.status)
                              .ToList();
            });
            return result;
        }


        public IEnumerable<ViewQotListH> GetQotList(QueryQotList query)
        {
            #region getvendorid 20211101
            int venderid = GetVendorId(query);
            #endregion
            //int venderid = query.vendor;
            var result = (from r in _context.SrmRfqHs
                          join q in _context.SrmQotHs on r.RfqId equals q.RfqId
                          join status in _context.SrmStatuses on r.Status equals status.Status
                          join vendor in _context.SrmVendors on q.VendorId equals vendor.VendorId
                          join u1 in _context.AspNetUsers on r.CreateBy equals u1.UserName
                          join u2 in _context.AspNetUsers on r.LastUpdateBy equals u2.UserName
                          select new ViewQotListH
                          {
                              VRfqId = r.RfqId,
                              VRfqNum = r.RfqNum,
                              VStatus = r.Status,
                              VStatusDesc = status.StatusDesc,
                              VCreateDate = r.CreateDate,
                              VCreateBy = u1.Name,
                              VLastUpdateDate = r.LastUpdateDate,
                              VLastUpdateBy = u2.Name,
                              VEndDate = r.EndDate,
                              VVendor = (!string.IsNullOrWhiteSpace(vendor.SapVendor))? vendor.SapVendor:vendor.SrmVendor1,
                              VVendorId = vendor.VendorId

                          })
                        //.Where(p => p.VVendor == query.vendor)
                        .Where(p => p.VVendorId == venderid)
                            //.Where(p => p.VStatus == 7) //0827
                            //.Where()
                            //.AndIfCondition(query.status != 0, p => p.Status == 7)
                            //.AndIfCondition(!string.IsNullOrWhiteSpace(query.vendor), p => p.VVendor == query.vendor)
                            .AndIfCondition(!string.IsNullOrWhiteSpace(query.rfqno), p => p.VRfqNum == query.rfqno)
                        .Distinct()
                        .ToList();

            result.ForEach(p =>
            {
                p.SrmQotHs = (from q in _context.SrmQotHs

                              join r in _context.SrmRfqHs on q.RfqId equals r.RfqId
                              join status in _context.SrmStatuses on q.Status equals status.Status
                              join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
                              join v in _context.SrmVendors on q.VendorId equals v.VendorId
                              join u1 in _context.AspNetUsers on r.CreateBy equals u1.UserName
                              join u2 in _context.AspNetUsers on r.LastUpdateBy equals u2.UserName
                              select new ViewQotListL
                              {
                                  QRfqId = q.RfqId,
                                  QStatus = q.Status,
                                  QQotId = q.QotId,
                                  QQotNum = q.QotNum,
                                  QMatnr = (!string.IsNullOrWhiteSpace(m.SapMatnr)) ? m.SapMatnr : m.SrmMatnr1,
                                  QCreateBy = u1.Name,
                                  QCreateDate = q.CreateDate,
                                  QLastUpdateBy = u2.Name,
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
                               Status = s.StatusDesc
                           });
            //.AndIfCondition(query.status != 0, p => p.QSTATUS == query.status)
            //.AndIfHaveValue(query.matnr, p => p.MATNR == query.matnr)
            //.AndIfHaveValue(query.rfqno, p => p.RFQ_NUM == query.rfqno);
            qotlist = qotlist.Where(p => p.RfqId == rfqid && p.VendorId == vendorid).OrderBy(p => p.QotId);
            return qotlist;
        }
        public IQueryable GetQotData(int rfqid, int vendorid, int qotid)
        {
            var qotlist = (from r in _context.SrmRfqHs
                           join q in _context.SrmQotHs on r.RfqId equals q.RfqId
                           join rm in _context.SrmRfqMs on new { RfqId = q.RfqId, MatnrId = q.MatnrId } equals new { RfqId = rm.RfqId, MatnrId = rm.MatnrId }
                           join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
                           join s in _context.SrmStatuses on q.Status equals s.Status
                           join u1 in _context.AspNetUsers on q.CreateBy equals u1.UserName


                           //where e.OwnerID == user.UID
                           select new
                           {

                               CreateBy = u1.Name,//q.CreateBy,
                               CreateDate = DateTime.Parse(q.CreateDate.ToString()).ToString("yyyy/MM/dd HH:mm:ss"),
                               Status = s.StatusDesc,
                               RfqNum = r.RfqNum,
                               QotId = q.QotId,
                               QotNum = q.QotNum,
                               //Status = q.Status.HasValue ? ((Status)q.Status).ToString() : "",
                               Matnr = (!string.IsNullOrWhiteSpace(m.SapMatnr)) ? m.SapMatnr : m.SrmMatnr1,
                               Material = rm.Material,
                               Weight = rm.Weight,
                               MachineName = rm.MachineName,
                               Size = rm.Length + "*" + rm.Width + "*" + rm.Height,
                               Length = rm.Length,
                               Width = rm.Width,
                               Height = rm.Height,
                               RfqId = r.RfqId,
                               VendorId = q.VendorId,
                               mEmptyFlag = q.MEmptyFlag,
                               pEmptyFlag = q.PEmptyFlag,
                               sEmptyFlag = q.SEmptyFlag,
                               oPEmptyFlag = q.OEmptyFlag,
                               Description = m.Description,
                               Qty = rm.Qty,
                               Expiringdate = q.ExpirationDate,
                               Leadtime = q.LeadTime
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
        public int GetVendorId(QueryQotList query)
        {
            var vendorid = 0;
            var vendor = (from v in _context.SrmVendors
                          where (v.SapVendor == query.vendor || v.SrmVendor1 == query.vendor)
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
                                    VendorName = vendor.VendorName
                                })
                                .Where(p => p.QotId == qotid)
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
                                   //SubTotal = process.PPrice.Value * (decimal)process.PHours.Value,
                                   QotId = process.QotId,
                                   PCostsum = process.PCostsum

                               })
                               .Where(p => p.QotId == qotid)
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
                                   //SubTotal = surface.SPrice.Value * (decimal)surface.STimes.Value,
                                   QotId = surface.QotId,
                                   SCostsum = surface.SCostsum
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
                                 ODescription = other.ODescription,
                                 OItem = other.OItem,
                                 ONote = other.ONote,
                                 OPrice = other.OPrice,
                                 VendorId = vendor.VendorId,
                                 VendorName = vendor.VendorName,
                                 QotId = other.QotId
                             }).Where(p => p.QotId == qotid)
                               .ToArray();




            return qotinfo;
        }

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


        public void Save(SrmQotH qotH, SrmQotMaterial[] qotMaterials, SrmQotSurface[] qotSurfaces, SrmQotProcess[] qotProcesses, SrmQotOther[] qotOthers)
        {
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
                db.Entry(qot).Property(p => p.LastUpdateBy).IsModified = true;
                db.Entry(qot).Property(p => p.LastUpdateDate).IsModified = true;
                db.Entry(qot).Property(p => p.MEmptyFlag).IsModified = true;
                db.Entry(qot).Property(p => p.PEmptyFlag).IsModified = true;
                db.Entry(qot).Property(p => p.SEmptyFlag).IsModified = true;
                db.Entry(qot).Property(p => p.OEmptyFlag).IsModified = true;
                db.Entry(qot).Property(p => p.LeadTime).IsModified = true;
                db.Entry(qot).Property(p => p.ExpirationDate).IsModified = true;

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

            if (!_context.SrmQotHs.Any(r => r.RfqId == qotH.RfqId && r.Status == (int)Status.初始))
            {
                throw new Exception("非初始無法修改");
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
        #endregion
    }
}
