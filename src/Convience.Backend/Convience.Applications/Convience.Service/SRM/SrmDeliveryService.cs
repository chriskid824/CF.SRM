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
        public string AddDelivery(AddDeliveryModel data);

        public IEnumerable<ViewSrmDeliveryH> GetDelivery(QueryPoList query);
        public int UpdateReplyDeliveryDateH(string ponum, DateTime date);
        public int UpdateReplyDeliveryDateWithReason(int poid, int polid, DateTime date, string reason);
        public bool UpdateReplyDeliveryDate(SrmPoL data);
        public bool UpdateDeliveryL(ViewSrmDeliveryL data);
        public bool DeleteDeliveryL(ViewSrmDeliveryL data);
        public bool DeleteDeliveryH(ViewSrmDeliveryH data);
        public string ReceiveDeliveryL(List<ViewSrmDeliveryL> datalist);
        public string SapDelivery(string userid);
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

        public string AddDelivery(AddDeliveryModel data)
        {
            var neworder = _context.SrmDeliveryHs
    .FromSqlRaw("EXECUTE dbo.GetNum {0}", 1).ToList().FirstOrDefault();
            SrmDeliveryH dh = _context.SrmDeliveryHs.Find(neworder.DeliveryId);
            dh.DeliveryDate = data.date;
            dh.DeliveryVendorsn = data.vendorsn;
            dh.DeliveryManager = data.manager;
            _context.SrmDeliveryHs.Update(dh);
            if (neworder == null) return null;
            data.data.ForEach(m =>
            {
                SrmDeliveryL l = new SrmDeliveryL()
                {
                    DeliveryId = neworder.DeliveryId,
                    PoId = m.PoId,
                    PoLId = m.PoLId,
                    DeliveryQty = m.DeliveryQty,
                    QmQty = 0,
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
            data.data.ForEach(m =>
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
            return dh.DeliveryNum;
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
                    DeliveryDate = p.DeliveryDate,
                    DeliveryNum = p.DeliveryNum,
                    DeliveryVendorsn = p.DeliveryVendorsn,
                    DeliveryManager = p.DeliveryManager,
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
                                   join matnr in _context.SrmMatnrs on pol.MatnrId equals matnr.MatnrId into matnrInfo
                                   from matnr in matnrInfo.DefaultIfEmpty()
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
                                       TelPhone = vendor.TelPhone,
                                       Address = vendor.Address,
                                       Cell = pol.Cell,
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
                    p.TelPhone = p.SrmDeliveryLs.First().TelPhone;
                    p.Address = p.SrmDeliveryLs.First().Address;
                    p.DeliveryPlace = p.SrmDeliveryLs.First().DeliveryPlace;
                }
            });
            return result.Where(p => p.SrmDeliveryLs.Count > 0).ToList();
        }
        public int UpdateReplyDeliveryDateH(string ponum, DateTime date)
        {
            int poid = _context.SrmPoHs.Where(p => p.PoNum == ponum).FirstOrDefault().PoId;
            List<SrmPoL> pols = _context.SrmPoLs.Where(p => p.PoId == poid).ToList();
            foreach (var item in pols)
            {
                item.Status = 15;
                item.ReplyDeliveryDate = date;
            }
            _context.SrmPoLs.UpdateRange(pols);
            _context.SaveChanges();
            return poid;
        }
        public int UpdateReplyDeliveryDateWithReason(int poid, int polid, DateTime date, string reason)
        {
            SrmPoL pol = _context.SrmPoLs.Where(p => p.PoId == poid && p.PoLId == polid).FirstOrDefault();
            pol.LastReplyDeliveryDate = pol.ReplyDeliveryDate;
            pol.ReplyDeliveryDate = date;
            pol.ChangeDateReason = reason;
            if (pol.LastReplyDeliveryDate != pol.ReplyDeliveryDate)
            {
                pol.Status = 11;
            }
            _context.SrmPoLs.Update(pol);

            if (pol.LastReplyDeliveryDate != pol.ReplyDeliveryDate)
            {
                SrmPoH poh = _context.SrmPoHs.Find(poid);
                poh.Status = 11;
                _context.SrmPoHs.Update(poh);
            }

            _context.SaveChanges();
            return poid;
        }
        public bool UpdateReplyDeliveryDate(SrmPoL data)
        {
            bool is3100 = false;
            SrmPoL pol = _context.SrmPoLs.Where(p => p.PoId == data.PoId && p.PoLId == data.PoLId).AsNoTracking().FirstOrDefault();
            data.LastReplyDeliveryDate = pol.ReplyDeliveryDate;
            SrmPoH poh = _context.SrmPoHs.Find(data.PoId);
            if (poh != null && poh.Org == 3100)
            {
                is3100 = true;
                //poh.
            }
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
                return DeleteDeliveryL(dl);
            }
            return false;
        }
        public bool DeleteDeliveryL(SrmDeliveryL dl)
        {
            if (dl != null)
            {
                SrmPoL pol = _context.SrmPoLs.Find(dl.PoId, dl.PoLId);
                pol.Status = 15;
                pol.DeliveryDate = null;
                _context.SrmPoLs.Update(pol);
                SrmPoH poh = _context.SrmPoHs.Find(dl.PoId);
                poh.Status = 15;
                _context.SrmPoHs.Update(poh);
                _context.SrmDeliveryLs.Remove(dl);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool DeleteDeliveryH(ViewSrmDeliveryH data)
        {
            List<SrmDeliveryL> dllist = _context.SrmDeliveryLs.Where(p => p.DeliveryId == data.DeliveryId).ToList();
            if (dllist != null && dllist.Count() > 0)
            {
                foreach (var item in dllist)
                {
                    DeleteDeliveryL(item);
                }
                SrmDeliveryH dh = _context.SrmDeliveryHs.Find(data.DeliveryId);
                if (dh != null)
                {
                    _context.SrmDeliveryHs.Remove(dh);
                    _context.SaveChanges();
                }
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
            if (dh.Status == 12)
            {
                return "此交貨單已交貨!";
            }
            dh.Status = 12;
            dh.LastUpdateDate = DateTime.Now;
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
        public string SapDelivery(string userid)
        {
            #region 全部交貨
            var query = _context.Zmmr008s.Where(p => Convert.ToDecimal(p.Weiqi1) == 0);
            //選出所有的採購單號後
            List<string> numlist = query.Select(p => p.Ebeln).Distinct().ToList();
            //List<SrmPoH> updatepohs = new List<SrmPoH>();
            //List<SrmPoL> updatepols = new List<SrmPoL>();

            foreach (var num in numlist)
            {
                if (_context.SrmPoHs.Any(p => p.PoNum ==num))
                {
                    //選出採購單號底下的項次
                    List<int> polidlist = query.Where(p => p.Ebeln==num).Select(p=> int.Parse(p.Ebelp)).ToList();
                    int poid = _context.SrmPoHs.Where(p => p.PoNum == num).AsNoTracking().FirstOrDefault().PoId;
                    foreach (var polid in polidlist)
                    {
                        if (_context.SrmPoLs.Any(p => p.PoId == poid && p.PoLId == Convert.ToInt16(polid) && p.Status != 12))
                        {
                            SrmPoL pol = _context.SrmPoLs.Where(p => p.PoId == poid && p.PoLId == Convert.ToInt16(polid)).FirstOrDefault();
                            pol.Status = 12;
                            _context.SrmPoLs.Update(pol);
                        }
                    }
                    if (!_context.SrmPoLs.Any(p => p.PoId == poid && !polidlist.Contains(p.PoLId) &&p.Status!=12))
                    {
                        SrmPoH poh= _context.SrmPoHs.Where(p => p.PoNum == num).FirstOrDefault();
                        poh.Status = 12;
                        _context.SrmPoHs.Update(poh);
                    }
                }
            }
            _context.SaveChanges();
            #endregion
            #region 部分交貨
            List<Zmmr008> list = _context.Zmmr008s.Where(p => p.Poqty != p.Weiqi1 && Convert.ToDecimal(p.Weiqi1) > 0).ToList();
            int d_id = 0;
            //先找到系統採購單 如果沒有就創建一個
            if (_context.SrmDeliveryHs.Any(p => p.DeliveryNum == "0000000000000000"))
            {
                d_id = _context.SrmDeliveryHs.Where(p => p.DeliveryNum == "0000000000000000").FirstOrDefault().DeliveryId;
                _context.SrmDeliveryLs.RemoveRange(_context.SrmDeliveryLs.Where(p => p.DeliveryId == d_id));
                _context.SaveChanges();
            }
            else
            {
                SrmDeliveryH dh = new SrmDeliveryH()
                {
                    DeliveryDate = DateTime.Now,
                    DeliveryNum = "0000000000000000",
                    Status = 12,
                    CreateBy = userid,
                    CreateDate = DateTime.Now,
                };
                _context.SrmDeliveryHs.Add(dh);
                _context.SaveChanges();
                d_id = dh.DeliveryId;
            }
            List<SrmDeliveryL> dls = new List<SrmDeliveryL>();
            foreach (var item in list)
            {
                if (_context.SrmPoHs.Any(p => p.PoNum == item.Ebeln))
                {
                    int poid = _context.SrmPoHs.Where(p => p.PoNum == item.Ebeln).FirstOrDefault().PoId;
                    SrmDeliveryL dl = new SrmDeliveryL()
                    {
                        DeliveryId = d_id,
                        PoId = poid,
                        PoLId = Convert.ToInt32(item.Ebelp),
                        DeliveryQty = (float?)(Convert.ToDecimal(item.Poqty) - Convert.ToDecimal(item.Weiqi1)),
                        QmQty=0,
                    };
                    dls.Add(dl);
                }

            }
            if (dls.Count > 0)
            {
                _context.SrmDeliveryLs.AddRange(dls);
                _context.SaveChanges();
            }
            #endregion


            return null;
        }
    }
}