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
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public interface ISrmDeliveryService
    {
        public bool AddDelivery(List<ViewSrmPoL> data);

        public IEnumerable<ViewSrmDeliveryH> GetDelivery(QueryPoList query);

        public bool UpdateReplyDeliveryDate(SrmPoL data);
        public bool UpdateDeliveryL(ViewSrmDeliveryL data);
        public bool DeleteDeliveryL(ViewSrmDeliveryL data);
        public string ReceiveDeliveryL(List<ViewSrmDeliveryL> datalist);
    }

    public class SrmDeliveryService : ISrmDeliveryService
    {
        private readonly SRMContext _context;

        //private readonly IMapper _mapper;
        private readonly IRepository<SrmPoH> _srmPohRepository;

        private readonly IRepository<SrmPoL> _srmPolRepository;
        private IMapper _mapper;

        public SrmDeliveryService(
            //IMapper mapper,
            IRepository<SrmPoH> srmPohRepository, IRepository<SrmPoL> srmPolRepository, SRMContext context, IMapper mapper)
        {
            _mapper = mapper;
            _srmPohRepository = srmPohRepository;
            _srmPolRepository = srmPolRepository;
            _context = context;
            //_systemIdentityDbUnitOfWork = systemIdentityDbUnitOfWork;
        }

        public bool AddDelivery(List<ViewSrmPoL> data)
        {
            var neworder = _context.SrmDeliveryHs
    .FromSqlRaw("EXECUTE dbo.GetNum {0}", 1).ToList().FirstOrDefault();
            if (neworder == null) return false;
            data.ForEach(m =>
            {
                SrmDeliveryL l = new SrmDeliveryL()
                {
                    DeliveryId = neworder.DeliveryId,
                    PoId = m.PoId,
                    PoLId = m.PoLId,
                    DeliveryQty = m.DeliveryQty,
                    QmQty = 0
                };
                _context.SrmDeliveryLs.Add(l);
                //如果都未發貨數量0了才改狀態
                if (m.RemainQty == l.DeliveryQty)
                {
                    SrmPoL pol = _context.SrmPoLs.Find(m.PoId, m.PoLId);
                    pol.Status = 14;
                    _context.SrmPoLs.Update(pol);
                }
            });
            _context.SaveChanges();
            data.ForEach(m =>
            {
                //如果所有項次狀態都發貨了就改主檔狀態
                if (!_context.SrmPoLs.Any(p => p.PoId == m.PoId && p.Status != 14))
                {
                    SrmPoH poh = _context.SrmPoHs.Where(p => p.PoId == m.PoId).FirstOrDefault();
                    poh.Status = 14;
                    _context.SrmPoHs.Update(poh);
                }
            });
            _context.SaveChanges();
            return true;
        }
        public IEnumerable<ViewSrmDeliveryH> GetDelivery(QueryPoList query)
        {
            var result = _context.SrmDeliveryHs
                .AndIfHaveValue(query.replyDeliveryDate_s, p => p.CreateDate >= query.replyDeliveryDate_s.Value.Date)
                .AndIfHaveValue(query.replyDeliveryDate_e, p => p.CreateDate <= query.replyDeliveryDate_e.Value.AddDays(1).Date)
                .AndIfCondition(!string.IsNullOrWhiteSpace(query.deliveryNum), p => p.DeliveryNum.IndexOf(query.deliveryNum) > -1)
                .AndIfCondition(query.status != 0, p => p.Status == query.status)
                .Select(p => new ViewSrmDeliveryH
                {
                    DeliveryId = p.DeliveryId,
                    DeliveryNum = p.DeliveryNum,
                    Status = p.Status,
                    CreateDate = p.CreateDate,
                    CreateBy = p.CreateBy,
                    LastUpdateDate = p.LastUpdateDate,
                    LastUpdateBy = p.LastUpdateBy
                }).ToList();
            result.ForEach(p =>
            {
                p.SrmDeliveryLs = (from l in _context.SrmDeliveryLs
                                   join h in _context.SrmDeliveryHs on l.DeliveryId equals h.DeliveryId
                                   join pol in _context.SrmPoLs on new { PoId = l.PoId.Value, PoLId = l.PoLId.Value } equals new { PoId = pol.PoId, PoLId = pol.PoLId }
                                   join poh in _context.SrmPoHs on l.PoId equals poh.PoId
                                   join matnr in _context.SrmMatnrs on pol.MatnrId equals matnr.MatnrId
                                   join vendor in _context.SrmVendors on poh.VendorId equals vendor.VendorId
                                   select new ViewSrmDeliveryL
                                   {
                                       DeliveryNum = h.DeliveryNum,
                                       DeliveryLId = l.DeliveryLId,
                                       DeliveryId = l.DeliveryId,
                                       PoId = l.PoId,
                                       PoLId = l.PoLId,
                                       DeliveryQty = l.DeliveryQty,
                                       QmQty = l.QmQty,
                                       Description = pol.Description,
                                       Matnr = matnr.SapMatnr,
                                       PoNum = poh.PoNum,
                                       Qty = pol.Qty,
                                       SapVendor = vendor.SapVendor,
                                       VendorName = vendor.VendorName,
                                       VendorId = vendor.VendorId,
                                       Org = poh.Org,
                                       DeliveryPlace = pol.DeliveryPlace,
                                       //Url = query.host + "/" + l.DeliveryLId.ToString() + "/" + p.DeliveryNum,
                                       //WoItem = pol.WoItem,
                                       //WoNum = pol.WoNum,
                                   })
                                   .Where(l => l.DeliveryId == p.DeliveryId)
                                   .AndIfCondition(!query.user.GetIsVendor(), p => query.user.GetUserWerks().Contains(p.Org.ToString()))
                                   .AndIfCondition(query.user.GetIsVendor(), p => p.SapVendor == query.user.GetVendorId())
                                   .AndIfCondition(!string.IsNullOrWhiteSpace(query.poNum), l => l.PoNum.IndexOf(query.poNum) > -1).ToList()
                                   .AndIfCondition(query.deliveryLId != 0, l => l.DeliveryLId == query.deliveryLId)
                                   .ToList();
                if (p.SrmDeliveryLs.Count > 0)
                {
                    p.VendorName = p.SrmDeliveryLs.First().VendorName;
                }
            });
            return result.Where(p => p.SrmDeliveryLs.Count > 0).ToList();
        }

        public bool UpdateReplyDeliveryDate(SrmPoL data)
        {
            _context.SrmPoLs.Update(data);
            _context.SaveChanges();
            return true;
        }
        public bool UpdateDeliveryL(ViewSrmDeliveryL data)
        {
            if (!data.DeliveryId.HasValue || !data.PoId.HasValue || !data.PoLId.HasValue || !data.DeliveryQty.HasValue) return false;
            if (!data.DeliveryLId.HasValue)
            {
                SrmDeliveryL dl = new SrmDeliveryL()
                {
                    DeliveryId = data.DeliveryId,
                    PoId = data.PoId,
                    PoLId = data.PoLId,
                    DeliveryQty = data.DeliveryQty,
                    QmQty = 0
                };
                _context.SrmDeliveryLs.Add(dl);
            }
            else
            {
                SrmDeliveryL dl = _context.SrmDeliveryLs.Find(data.DeliveryLId);
                dl.DeliveryQty = data.DeliveryQty;
                _context.SrmDeliveryLs.Update(dl);
            }
            _context.SaveChanges();
            //如果都未發貨數量0了才改狀態
            if (CheckAllDelivery(data.PoId.Value, data.PoLId.Value))
            {
                SrmPoL pol = _context.SrmPoLs.Find(data.PoId, data.PoLId);
                pol.Status = 14;
                _context.SrmPoLs.Update(pol);

            }
            //如果沒有全發貨 改回去狀態
            else
            {
                SrmPoL pol = _context.SrmPoLs.Find(data.PoId, data.PoLId);
                pol.Status = 15;
                _context.SrmPoLs.Update(pol);

            }
            _context.SaveChanges();
            //如果所有項次狀態都發貨了就改主檔狀態
            if (!_context.SrmPoLs.Any(p => p.PoId == data.PoId && p.Status != 14))
            {
                SrmPoH poh = _context.SrmPoHs.Where(p => p.PoId == data.PoId).FirstOrDefault();
                poh.Status = 14;
                _context.SrmPoHs.Update(poh);
            }
            //如果所有項次狀態都發貨了就改主檔狀態
            else if (_context.SrmPoLs.Any(p => p.PoId == data.PoId && p.Status != 14))
            {
                SrmPoH poh = _context.SrmPoHs.Where(p => p.PoId == data.PoId).FirstOrDefault();
                poh.Status = 15;
                _context.SrmPoHs.Update(poh);
            }
            _context.SaveChanges();
            return true;
        }
        public bool DeleteDeliveryL(ViewSrmDeliveryL data)
        {
            SrmDeliveryL dl = _context.SrmDeliveryLs.Find(data.DeliveryLId);
            if (dl != null)
            {
                _context.SrmDeliveryLs.Remove(dl);
                SrmPoL pol = _context.SrmPoLs.Find(data.PoId, data.PoLId);
                pol.Status = 14;
                _context.SrmPoLs.Update(pol);
                SrmPoH poh = _context.SrmPoHs.Find(data.PoId);
                poh.Status = 14;
                _context.SrmPoHs.Update(poh);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public string ReceiveDeliveryL(List<ViewSrmDeliveryL> datalist)
        {
            #region 1.修改status
            ViewSrmDeliveryL dl = datalist.FirstOrDefault();
            #region 1.1 dh
            SrmDeliveryH dh = _context.SrmDeliveryHs.Find(dl.DeliveryId);
            dh.Status = 12;
            _context.SrmDeliveryHs.Update(dh);
            #endregion
            #region 1.2 pol
            datalist.ForEach(m =>
                {
                    SrmPoL pl = _context.SrmPoLs.Find(m.PoId, m.PoLId);
                    pl.Status = 12;
                    _context.SrmPoLs.Update(pl);
                });
            #endregion
            _context.SaveChanges();
            #region poh
            List<int?> poidList = datalist.Select(p => p.PoId).ToList();
            poidList.ForEach(m =>
            {
                if (_context.SrmPoLs.Any(p => p.PoId == m && p.Status != 12))
                { }
                else
                {
                    SrmPoH ph = _context.SrmPoHs.Find(m);
                    ph.Status = 12;
                    _context.SrmPoHs.Update(ph);
                }
            });
            _context.SaveChanges();
            #endregion
            #endregion
            //2.rfc


            return null;
        }
        public bool CheckAllDelivery(int poid, int polid)
        {
            float deliveryQty = _context.SrmDeliveryLs.Where(p => p.PoId == poid && p.PoLId == polid).Sum(p => p.DeliveryQty).Value;
            float Qty = _context.SrmPoLs.Where(p => p.PoId == poid && p.PoLId == polid).Select(p => p.Qty).FirstOrDefault().Value;
            return deliveryQty == Qty;
        }
    }
}