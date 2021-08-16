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
    }
    public class SrmQotService : ISrmQotService
    {
        private readonly IRepository<SrmQotH> _srmQotHRepository;
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

        public SrmQotH[] Get(QueryQot query)
        {
            //using (SRMContext db = new SRMContext())
            //{
            var qotQurty = _context.SrmQotHs.AsQueryable()
                .AndIfHaveValue(query.rfqId, r => r.RfqId == query.rfqId)
                .AndIfHaveValue(query.matnrId, r => r.MatnrId == query.matnrId)
                .OrderBy(r => r.MatnrId);
            return qotQurty.ToArray();
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
            var result = _context.SrmRfqHs
                .Select(p => new ViewQotListH
                {
                    VRfqId = p.RfqId,
                    VRfqNum = p.RfqNum,
                    VStatus = p.Status,
                    VCreateDate = p.CreateDate,
                    VCreateBy = p.CreateBy,
                    VLastUpdateDate = p.LastUpdateDate,
                    VLastUpdateBy = p.LastUpdateBy,
                    VEndDate = p.EndDate
                })
                .ToList();

            result.ForEach(p =>
            {
                p.SrmQotHs = (from q in _context.SrmQotHs
                              join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
                              join v in _context.SrmVendors on q.VendorId equals v.VendorId
                              select new ViewQotListL
                              {
                                  QStatus = q.Status,
                                  QQotId = q.QotId,
                                  QQotNum = q.QotNum,
                                  QMatnr = m.SapMatnr,
                                  QCreateBy = q.CreateBy,
                                  QCreateDate = q.CreateDate,
                                  QLastUpdateBy = q.LastUpdateBy,
                                  QLastUpdateDate = q.LastUpdateDate,
                              }).Where(p => p.Status !=5)             
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

        public IEnumerable<ViewQotListH> GetQotList(QueryQotList query)
        {
            //int venderid = query.vendor;
            var result = _context.SrmRfqHs
                .AndIfHaveValue(query.rfqno, p => p.RfqNum == query.rfqno)
                .Select(p => new ViewQotListH
                {
                    VRfqId = p.RfqId,
                    VRfqNum =p.RfqNum,
                    VStatus = p.Status,
                    VCreateDate = p.CreateDate,
                    VCreateBy = p.CreateBy,
                    VLastUpdateDate = p.LastUpdateDate,
                    VLastUpdateBy = p.LastUpdateBy,
                    VEndDate = p.EndDate
                })
                .ToList();

            result.ForEach(p =>
            {
                p.SrmQotHs = (from q in _context.SrmQotHs
                              join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
                              join v in _context.SrmVendors on q.VendorId equals v.VendorId
                              select new ViewQotListL
                              {
                                  QStatus = q.Status,
                                  QQotId = q.QotId,
                                  QQotNum = q.QotNum,
                                  QMatnr = m.SapMatnr,
                                  QCreateBy = q.CreateBy,
                                  QCreateDate = q.CreateDate,
                                  QLastUpdateBy = q.LastUpdateBy,
                                  QLastUpdateDate = q.LastUpdateDate,
                                  QVendorId = q.VendorId,
                                  QVendor = v.SapVendor
                              })
                              //.ToList();
                              //.Where(p => p.QVendorId.Value == query.vendor)
                              .Where(p => p.QVendor == query.vendor) //供應商登入帳號為供應商代碼
                              .AndIfCondition(!string.IsNullOrWhiteSpace(query.matnr), p => p.QMatnr == query.matnr)
                              .AndIfCondition(query.status != 0, p => p.QStatus.Value == query.status)
                              .ToList();            
            });
            return result;
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
    }
}
