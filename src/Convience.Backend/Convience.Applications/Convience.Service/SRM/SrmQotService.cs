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
        //public IEnumerable<ViewQot> GetDataBQotId(int QotId);
        public IQueryable GetQotData(int QotId);
        public IQueryable GetMatnrData(int rfqid,int vendorid);
        public ViewSrmPriceDetail GetDetail(SrmQotH[] query);

        //public ViewQotListL GetDataByRfqId(int RfqId);
    }
    public class SrmQotService : ISrmQotService
    {
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

                              join r in _context.SrmRfqHs on q.RfqId equals r.RfqId
                             

                              join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
                              join v in _context.SrmVendors on q.VendorId equals v.VendorId
                              select new ViewQotListL
                              {
                                  QRfqId = q.RfqId,
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

                              join r in _context.SrmRfqHs on q.RfqId equals r.RfqId

                              join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
                              join v in _context.SrmVendors on q.VendorId equals v.VendorId
                              select new ViewQotListL
                              {
                                  QRfqId = q.RfqId,
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
        public IQueryable GetMatnrData(int rfqid, int vendorid) 
        {
            var qotlist = (from q in _context.SrmQotHs
                           join r in _context.SrmRfqHs on q.RfqId equals r.RfqId
                           join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
                           select new
                           {
                               QotId = q.QotId,
                               RfqId = r.RfqId,
                               VendorId = q.VendorId,
                               RfqNum = r.RfqNum,
                               Matnr = m.SapMatnr,
                               MatnrId = q.MatnrId
                           });
            //.AndIfCondition(query.status != 0, p => p.QSTATUS == query.status)
            //.AndIfHaveValue(query.matnr, p => p.MATNR == query.matnr)
            //.AndIfHaveValue(query.rfqno, p => p.RFQ_NUM == query.rfqno);
            qotlist = qotlist.Where(p => p.RfqId == rfqid && p.VendorId == vendorid);
            return qotlist;
        }
        public IQueryable GetQotData(int QotId)
        {
            var qotlist = (from q in _context.SrmQotHs
                           join r in _context.SrmRfqHs on q.RfqId equals r.RfqId
                           join rm in _context.SrmRfqMs on r.RfqId equals rm.RfqId
                           join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId

                           //where e.OwnerID == user.UID
                           select new 
                           {
                              
                               RfqNum = r.RfqNum,
                               CreateBy = q.CreateBy,
                               CreateDate = q.CreateDate,
                               QotId = q.QotId,
                               QotNum = q.QotNum,
                               Status = q.Status.HasValue ? ((Status)q.Status).ToString() : "",
                               Matnr = m.SapMatnr,
                               Material = rm.Material,
                               Weight = rm.Weight,
                               MachineName = rm.MachineName,
                               Size = rm.Length + "*" + rm.Width + "*" + rm.Height,
                               Length = rm.Length,
                               Width = rm.Width,
                               Height = rm.Height
                           });
            //.AndIfCondition(query.status != 0, p => p.QSTATUS == query.status)
            //.AndIfHaveValue(query.matnr, p => p.MATNR == query.matnr)
            //.AndIfHaveValue(query.rfqno, p => p.RFQ_NUM == query.rfqno);
            qotlist = qotlist.Where(p => p.QotId == QotId) ;
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
        public ViewSrmPriceDetail GetDetail(SrmQotH[] query)
        {
            //.AsEnumerable().Select(r => r.Field<string>("Name")).ToArray();
            int[] qotIds = query.AsEnumerable().Select(r => r.QotId).ToArray();
            int[] vendorIds = query.AsEnumerable().Select(r => r.VendorId.Value).Distinct().ToArray();
            var temp = query.AsEnumerable().Select(r => new { a = r.RfqId, b = r.VendorId }).ToArray();
            ViewSrmPriceDetail price = new ViewSrmPriceDetail();
            price.qot = query;
            //using (SRMContext _context = new SRMContext())
            //{

            price.material = (from material in _context.SrmQotMaterial
                              join qot in _context.SrmQotHs
                              on material.QotId equals qot.QotId
                              join vendor in _context.SrmVendors
                              on qot.VendorId equals vendor.VendorId
                              where qotIds.Contains(material.QotId.Value)
                              select new viewSrmQotMaterial(material)
                              {
                                  //QotMId = material.QotMId,
                                  //QotId = material.QotId,
                                  //MMaterial = material.MMaterial,
                                  //MPrice = material.MPrice,
                                  //MCostPrice = material.MCostPrice,
                                  //Length = material.Length,
                                  //Width = material.Width,
                                  //Height = material.Height,
                                  //Density = material.Density,
                                  //Weight = material.Weight,
                                  //Note = material.Note,
                                  VendorId = vendor.VendorId,
                                  VendorName = vendor.VendorName
                              }).ToArray();
            //price.material = (from material in db.SrmQotMaterial
            //           where qotIds.Contains(material.QotId.Value)
            //           select material).ToArray();
            price.process = (from process in _context.SrmQotProcesses
                             join qot in _context.SrmQotHs
                             on process.QotId equals qot.QotId
                             join vendor in _context.SrmVendors
                             on qot.VendorId equals vendor.VendorId
                             where qotIds.Contains(process.QotId.Value)
                             select new viewSrmQotProcess(process)
                             {
                                 VendorId = vendor.VendorId,
                                 VendorName = vendor.VendorName,
                                 SubTotal = process.PPrice.Value * (decimal)process.PHours.Value
                             }).ToArray();
            //price.process = (from process in db.SrmQotProcesses
            //                             where qotIds.Contains(process.QotId.Value)
            //                             select process).ToArray();
            price.surface = (from surface in _context.SrmQotSurfaces
                             join qot in _context.SrmQotHs
             on surface.QotId equals qot.QotId
                             join vendor in _context.SrmVendors
                             on qot.VendorId equals vendor.VendorId
                             where qotIds.Contains(surface.QotId.Value)
                             select new viewSrmQotSurface(surface)
                             {
                                 VendorId = vendor.VendorId,
                                 VendorName = vendor.VendorName,
                                 SubTotal = surface.SPrice.Value * (decimal)surface.STimes.Value
                             }).ToArray();
            price.other = (from other in _context.SrmQotOthers
                           join qot in _context.SrmQotHs
             on other.QotId equals qot.QotId
                           join vendor in _context.SrmVendors
                           on qot.VendorId equals vendor.VendorId
                           where qotIds.Contains(other.QotId.Value)
                           select new viewSrmQotOther(other)
                           {
                               VendorId = vendor.VendorId,
                               VendorName = vendor.VendorName
                           }).ToArray();


            //(from material in db.SrmQotMaterial
            // join qot in db.SrmQotHs
            // on material.QotId equals qot.QotId
            // join vendor in db.SrmVendors
            // on qot.VendorId equals vendor.VendorId
            // where qotIds.Contains(material.QotId.Value)
            // select new viewSrmQotMaterial(material)
            // {
            //     VendorId = vendor.VendorId,
            //     VendorName = vendor.VendorName
            // })
            // .GroupBy(r => r.VendorName).Select(g => new
            // {
            //     VendorId = g.Key,
            //     count = g.Sum(s => s.MCostPrice)
            // });

            viewSrmInfoRecord[] infos = new viewSrmInfoRecord[query.Count()];
            for (int i = 0; i < infos.Length; i++)
            {
                viewSrmInfoRecord info = new viewSrmInfoRecord();
                info.VendorId = query[i].VendorId;
                info.VendorName = _context.SrmVendors.Where(r => r.VendorId == info.VendorId).Select(r => r.VendorName).FirstOrDefault();
                info.QotId = query[i].QotId;
                info.MatnrId = query[i].MatnrId;
                info.Atotal = price.material.Where(r => r.QotId == info.QotId && r.MCostPrice.HasValue).Select(r => r.MCostPrice).Sum().Value;
                info.Btotal = price.process.Where(r => r.QotId == info.QotId).Select(r => r.SubTotal).Sum();
                info.Ctotal = price.surface.Where(r => r.QotId == info.QotId).Select(r => r.SubTotal).Sum();
                info.Dtotal = price.other.Where(r => r.QotId == info.QotId && r.OPrice.HasValue).Select(r => r.OPrice).Sum().Value;
                infos[i] = info;
            }
            price.infoRecord = infos;
            //price.infoRecord = price.material.GroupBy(r => r.VendorName).Select(g => new
            //{
            //    VendorId = g.Key,
            //    count = g.Sum(s => s.MCostPrice)
            //}).GroupJoin(
            //price.process.GroupBy(r => r.VendorName).Select(g => new
            //{
            //    VendorId = g.Key,
            //    count = g.Sum(s => s.SubTotal)
            //}), a => a.VendorId, b => b.VendorId, (material, process) => new
            //{
            //    material = material,
            //    process = process.DefaultIfEmpty()
            //}).GroupJoin(
            //price.surface.GroupBy(r => r.VendorName).Select(g => new
            //{
            //    VendorId = g.Key,
            //    count = g.Sum(s => s.SubTotal)
            //}), r => r.material.VendorId, c => c.VendorId, (infoRecord, surface) => new
            //{
            //    infoRecord,
            //    surface = surface.DefaultIfEmpty()
            //}).GroupJoin(
            //price.other.GroupBy(r => r.VendorName).Select(g => new
            //{
            //    VendorId = g.Key,
            //    count = g.Sum(s => s.OPrice)
            //}), r => r.infoRecord.material.VendorId, d => d.VendorId, (infoRecord, other) => new viewSrmQotInfoRecord
            //{
            //    Atotal = infoRecord.infoRecord.material.count.Value,
            //    Btotal = infoRecord.infoRecord.process.,
            //    Ctotal = infoRecord.surface.count,
            //    Dtotal = other.count.Value
            //}).DefaultIfEmpty().ToArray();

            //}
            return price;
        }
    }
}
