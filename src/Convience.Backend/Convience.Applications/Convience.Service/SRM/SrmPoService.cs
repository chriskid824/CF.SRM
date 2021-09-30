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
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public interface ISrmPoService
    {
        public bool CheckAllReply(int id);

        /// <summary>
        /// 取得全部角色
        /// </summary>
        public IEnumerable<ViewSrmPoH> GetAll();

        public IEnumerable<ViewSrmPoH> GetAll(QueryPoList query);

        public IEnumerable<ViewSrmPoL> GetPoL(QueryPoList query);
        public PagingResultModel<ViewSrmPoPoL> GetPoPoL(QueryPoList query, int page, int size);

        public bool UpdateStatus(int id, int status);
        public List<SapResultData> UpdateSapData(SapPoData data,string userName);
    }

    public class SrmPoService : ISrmPoService
    {
        private readonly SRMContext _context;

        //private readonly IMapper _mapper;
        private readonly IRepository<SrmPoH> _srmPohRepository;

        private readonly IRepository<SrmPoL> _srmPolRepository;
        private IMapper _mapper;

        public SrmPoService(
            //IMapper mapper,
            IRepository<SrmPoH> srmPohRepository, IRepository<SrmPoL> srmPolRepository, SRMContext context, IMapper mapper)
        {
            _mapper = mapper;
            _srmPohRepository = srmPohRepository;
            _srmPolRepository = srmPolRepository;
            _context = context;
            //_systemIdentityDbUnitOfWork = systemIdentityDbUnitOfWork;
        }

        public bool CheckAllReply(int id)
        {
            //必須是待接收 或是已接收狀態
            if (_context.SrmPoHs.Find(id).Status == 21 || _context.SrmPoHs.Find(id).Status == 11)
            {
                if (_context.SrmPoLs.Any(p => p.PoId == id && p.ReplyDeliveryDate == null))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public IEnumerable<ViewSrmPoH> GetAll()
        {
            return GetAll(new QueryPoList());
        }

        public IEnumerable<ViewSrmPoH> GetAll(QueryPoList query)
        {
            var result = (from poh in _context.SrmPoHs
                          join status in _context.SrmStatuses on poh.Status equals status.Status
                          join vendor in _context.SrmVendors on poh.VendorId equals vendor.VendorId
                          select new ViewSrmPoH
                          {
                              PoId = poh.PoId,
                              PoNum = poh.PoNum,
                              VendorId = poh.VendorId,
                              Status = poh.Status,
                              Buyer = poh.Buyer,
                              Org = poh.Org,
                              DocDate = poh.DocDate,
                              ReplyDate = poh.ReplyDate,
                              CreateDate = poh.CreateDate,
                              CreateBy = poh.CreateBy,
                              LastUpdateDate = poh.LastUpdateDate,
                              LastUpdateBy = poh.LastUpdateBy,
                              TotalAmount = poh.TotalAmount,
                              StatusDesc = status.StatusDesc,
                              VendorName = vendor.VendorName,
                              SapVendor = vendor.SapVendor,
                              //SrmPoLs = poh.SrmPoLs,
                          })
                          .AndIfCondition(!query.user.GetIsVendor(), p => query.user.GetUserWerks().Contains(p.Org.ToString()))
                          .AndIfCondition(query.user.GetIsVendor(), p => p.SapVendor == query.user.GetUserName())
                .AndIfCondition(!string.IsNullOrWhiteSpace(query.buyer), p => p.Buyer.IndexOf(query.buyer) > -1)
                .AndIfCondition(!string.IsNullOrWhiteSpace(query.poNum), p => p.PoNum.IndexOf(query.poNum) > -1)
                .AndIfCondition(query.status != 0, p => p.Status == query.status).ToList();
            result.ForEach(p =>
            {
                p.SrmPoLs = (from l in _context.SrmPoLs
                             join h in _context.SrmPoHs on l.PoId equals h.PoId
                             join status in _context.SrmStatuses on l.Status equals status.Status
                             join vendor in _context.SrmVendors on h.VendorId equals vendor.VendorId
                             join matnr in _context.SrmMatnrs on l.MatnrId equals matnr.MatnrId
                             select new ViewSrmPoL
                             {
                                 PoNum = h.PoNum,
                                 PoLId = l.PoLId,
                                 PoId = l.PoId,
                                 MatnrId = l.MatnrId,
                                 Description = l.Description,
                                 Qty = l.Qty,
                                 Price = l.Price,
                                 DeliveryDate = l.DeliveryDate,
                                 ReplyDeliveryDate = l.ReplyDeliveryDate,
                                 DeliveryPlace = l.DeliveryPlace,
                                 CriticalPart = l.CriticalPart,
                                 InspectionTime = l.InspectionTime,
                                 Status = h.Status,
                                 VendorId = h.VendorId,
                                 VendorName = vendor.VendorName,
                                 TotalAmount = h.TotalAmount,
                                 Buyer = h.Buyer,
                                 StatusDesc = status.StatusDesc,
                                 Matnr = matnr.SapMatnr
                             }).Where(l => l.PoId == p.PoId).ToList();
            });
            return result.ToList();
        }

        public IEnumerable<SrmPoH> GetMatnrById(int id)
        {
            return _srmPohRepository.Get(r => r.PoId == id);
        }

        public IEnumerable<ViewSrmPoL> GetPoL(QueryPoList query)
        {
            var result = (from l in _context.SrmPoLs
                          join h in _context.SrmPoHs on l.PoId equals h.PoId
                          join status in _context.SrmStatuses on l.Status equals status.Status
                          join vendor in _context.SrmVendors on h.VendorId equals vendor.VendorId
                          join matnr in _context.SrmMatnrs on l.MatnrId equals matnr.MatnrId
                          select new ViewSrmPoL
                          {
                              PoNum = h.PoNum,
                              PoLId = l.PoLId,
                              PoId = l.PoId,
                              MatnrId = l.MatnrId,
                              Description = l.Description,
                              Qty = l.Qty,
                              Price = l.Price,
                              DeliveryDate = l.DeliveryDate,
                              ReplyDeliveryDate = l.ReplyDeliveryDate,
                              DeliveryPlace = l.DeliveryPlace,
                              CriticalPart = l.CriticalPart,
                              InspectionTime = l.InspectionTime,
                              Status = h.Status,
                              VendorId = h.VendorId,
                              VendorName = vendor.VendorName,
                              SapVendor = vendor.SapVendor,
                              TotalAmount = h.TotalAmount,
                              Buyer = h.Buyer,
                              StatusDesc = status.StatusDesc,
                              Matnr = matnr.SapMatnr,
                              Org = h.Org
                          })
                          .AndIfCondition(!query.user.GetIsVendor(), p => query.user.GetUserWerks().Contains(p.Org.ToString()))
                          .AndIfCondition(query.user.GetIsVendor(), p => p.SapVendor == query.user.GetUserName())
                              .AndIfCondition(!string.IsNullOrWhiteSpace(query.poNum), p => p.PoNum.IndexOf(query.poNum) > -1)
                              .AndIfCondition(query.poLId != 0, p => p.PoLId == query.poLId)
                .AndIfHaveValue(query.replyDeliveryDate_s, p => p.DeliveryDate >= query.replyDeliveryDate_s.Value.Date)
                .AndIfHaveValue(query.replyDeliveryDate_e, p => p.DeliveryDate <= query.replyDeliveryDate_e.Value.AddDays(1).Date)
                .AndIfCondition(query.status != 0, p => p.Status == query.status).ToList();

            //var result = _context.SrmPoLs.Join(
            //    _context.SrmPoHs,
            //    l => l.PoId,
            //    h => h.PoId,
            //    (l, h) => new ViewSrmPoL
            //    {
            //        PoNum = h.PoNum,
            //        PoLId = l.PoLId,
            //        PoId = l.PoId,
            //        MatnrId = l.MatnrId,
            //        Description = l.Description,
            //        Qty = l.Qty,
            //        Price = l.Price,
            //        DeliveryDate = l.DeliveryDate,
            //        ReplyDeliveryDate = l.ReplyDeliveryDate,
            //        DeliveryPlace = l.DeliveryPlace,
            //        CriticalPart = l.CriticalPart,
            //        InspectionTime = l.InspectionTime,
            //        Status = h.Status,
            //        VendorId = h.VendorId,
            //        TotalAmount = h.TotalAmount,
            //        Buyer = h.Buyer,
            //    }
            //    )
            //    //.AndIfCondition(!string.IsNullOrWhiteSpace(query.buyer), p => p.Buyer.IndexOf(query.buyer) > -1)
            //    .AndIfCondition(!string.IsNullOrWhiteSpace(query.poNum), p => p.PoNum.IndexOf(query.poNum) > -1)
            //    .AndIfHaveValue(query.replyDeliveryDate_s, p => p.DeliveryDate >= query.replyDeliveryDate_s.Value.Date)
            //    .AndIfHaveValue(query.replyDeliveryDate_e, p => p.DeliveryDate <= query.replyDeliveryDate_e.Value.AddDays(1).Date)
            //    .AndIfCondition(query.status != 0, p => p.Status == query.status).ToList();

            result.ForEach(p =>
            {
                p.RemainQty = p.Qty - _context.SrmDeliveryLs.Where(q => q.PoId == p.PoId && q.PoLId == p.PoLId).Sum(q => q.DeliveryQty);
            });

            //.AndIfCondition(query.status != 0, p => p.Status == query.status);
            return result.Where(p => p.RemainQty > 0).ToList();
        }

        public IEnumerable<SrmPoL> GetPolById(int id)
        {
            return _srmPolRepository.Get(r => r.PoId == id);
        }

        public bool UpdateStatus(int id, int status)
        {
            SrmPoH data = _context.SrmPoHs.Find(id);
            data.Status = status;
            if (status == 11)
            {
                data.ReplyDate = DateTime.Now;
            }
            _context.SrmPoHs.Update(data);
            var LList = _context.SrmPoLs.Where(p => p.PoId == data.PoId);
            foreach (var item in LList)
            {
                item.Status = status;
                _context.SrmPoLs.Update(item);
            }
            _context.SaveChanges();
            return true;
        }
        public PagingResultModel<ViewSrmPoPoL> GetPoPoL(QueryPoList query, int page, int size)
        {
            int skip = (page - 1) * size;

            var result = (from l in _context.SrmPoLs
                          join h in _context.SrmPoHs on l.PoId equals h.PoId
                          join status in _context.SrmStatuses on l.Status equals status.Status
                          join vendor in _context.SrmVendors on h.VendorId equals vendor.VendorId
                          join matnr in _context.SrmMatnrs on l.MatnrId equals matnr.MatnrId
                          select new ViewSrmPoPoL
                          {
                              PoNum = h.PoNum,
                              PoLId = l.PoLId,
                              PoId = l.PoId,
                              MatnrId = l.MatnrId,
                              Description = l.Description,
                              Qty = l.Qty,
                              Price = l.Price,
                              DeliveryDate = l.DeliveryDate,
                              ReplyDeliveryDate = l.ReplyDeliveryDate,
                              DeliveryPlace = l.DeliveryPlace,
                              CriticalPart = l.CriticalPart,
                              InspectionTime = l.InspectionTime,
                              Status = h.Status,
                              VendorId = h.VendorId,
                              VendorName = vendor.VendorName,
                              SapVendor = vendor.SapVendor,
                              TotalAmount = h.TotalAmount,
                              Buyer = h.Buyer,
                              StatusDesc = status.StatusDesc,
                              Matnr = matnr.SapMatnr,
                              Org = h.Org,
                              DocDate = h.DocDate,
                              ReplyDate = h.ReplyDate,
                              CreateDate=h.CreateDate,
                          })
              .AndIfCondition(!query.user.GetIsVendor(), p => query.user.GetUserWerks().Contains(p.Org.ToString()))
              .AndIfCondition(query.user.GetIsVendor(), p => p.SapVendor == query.user.GetUserName())
                  .AndIfCondition(!string.IsNullOrWhiteSpace(query.poNum), p => p.PoNum.IndexOf(query.poNum) > -1)
    //.AndIfCondition(query.poLId != 0, p => p.PoLId == query.poLId)
    //.AndIfHaveValue(query.replyDeliveryDate_s, p => p.DeliveryDate >= query.replyDeliveryDate_s.Value.Date)
    //.AndIfHaveValue(query.replyDeliveryDate_e, p => p.DeliveryDate <= query.replyDeliveryDate_e.Value.AddDays(1).Date)
    .AndIfCondition(query.status != 0, p => p.Status == query.status)
                .AndIfHaveValue(query.buyer, p => p.Buyer == query.buyer).ToList();

            var r = result.AsQueryable().Skip(skip).Take(size).ToArray();//result.Skip(skip).Take(size);
            return new PagingResultModel<ViewSrmPoPoL>
            {
                Data = r,
                Count = result.Count()
            };
        }
        public List<SapResultData> UpdateSapData(SapPoData data, string userName)
        {
            List<SapResultData> result = new List<SapResultData>();
            data.T_EKKO.ForEach(po =>
            {
                SapResultData r = new SapResultData() { Id = po.EBELN, Type = "採購單" };
                if (!_context.SrmPoHs.Any(p => p.PoNum == po.EBELN))
                {
                    if (_context.SrmVendors.Any(v => v.SapVendor == po.LIFNR))
                    {
                        int vendorid = _context.SrmVendors.FirstOrDefault(p => p.SapVendor == po.LIFNR).VendorId;
                        DateTime now = DateTime.Now;
                        SrmPoH poH = new SrmPoH()
                        {
                            PoNum = po.EBELN,
                            Status = 21,
                            VendorId = vendorid,
                            TotalAmount = Convert.ToInt32(Convert.ToDouble(po.RLWRT)),
                            Buyer = po.EKGRP,
                            Org = po.EKORG,
                            DocDate = po.BEDAT,
                            CreateDate=now,
                            CreateBy= userName,
                            LastUpdateDate=now,
                            LastUpdateBy= userName,
                        };
                        _context.SrmPoHs.Add(poH);
                        r.OutCome = "成功";
                    }
                    else
                    {
                        r.OutCome = "失敗";
                        r.Reason = "供應商 " + po.LIFNR + " 不存在";
                    }
                }
                else
                {
                    r.OutCome = "失敗";
                    r.Reason = "該採購單號已存在";
                }
                result.Add(r);
            });
            _context.SaveChanges();
            data.T_EKPO.ForEach(pol =>
            {
                SapResultData r = new SapResultData() { Id = pol.EBELN, LId = pol.EBELP, Type = "採購項次" };
                //採購單號
                if (_context.SrmPoHs.Any(p => p.PoNum == pol.EBELN))
                {
                    SrmPoH poH = _context.SrmPoHs.FirstOrDefault(h => h.PoNum == pol.EBELN);
                    //採購項次
                    if (!_context.SrmPoLs.Any(l => l.PoId == poH.PoId && l.PoLId == pol.EBELP))
                    {
                        //料號
                        if (_context.SrmMatnrs.Any(m => m.SapMatnr == pol.MATNR))
                        {
                            int matnrid = _context.SrmMatnrs.FirstOrDefault(p => p.SapMatnr == pol.MATNR).MatnrId;
                            SrmPoL poL = new SrmPoL()
                            {
                                PoLId = pol.EBELP,
                                PoId = poH.PoId,
                                MatnrId = matnrid,
                                Description = pol.MAKTX,
                                Qty = Convert.ToInt32(Convert.ToDouble(pol.MENGE)),
                                Price = pol.NETPR,
                                DeliveryDate = pol.EINDT,
                                DeliveryPlace = pol.LGOBE,
                                CriticalPart = pol.KZKRI,
                                InspectionTime = 1,
                                Status = 21,
                                WoNum = pol.AUFNR,
                            };
                            _context.SrmPoLs.Add(poL);
                            r.OutCome = "成功";
                        }
                        else
                        {
                            r.OutCome = "失敗";
                            r.Reason = "料號 " + pol.MATNR + " 不存在";
                        }
                    }
                    else
                    {
                        r.OutCome = "失敗";
                        r.Reason = "該採購項次已存在";
                    }
                }
                else
                {
                    r.OutCome = "失敗";
                    r.Reason = "該採購單號不存在";
                }
                result.Add(r);
            });
            _context.SaveChanges();
            return result;
        }
    }
}