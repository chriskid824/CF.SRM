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
        /// <summary>
        /// 取得全部角色
        /// </summary>
        public IEnumerable<SrmPoH> GetAll();
        public IEnumerable<SrmPoH> GetAll(QueryPoList query);
        public IEnumerable<ViewSrmPoL> GetPoL(QueryPoList query);
        public IEnumerable<SrmDeliveryH> GetDelivery(QueryPoList query);
        public bool UpdateReplyDeliveryDate(SrmPoL data);
        public bool UpdateStatus(int id, int status);
        public bool AddDelivery(List<ViewSrmPoL> data);
        public bool CheckAllReply(int id);
    }
    public class SrmPoService : ISrmPoService
    {
        //private readonly IMapper _mapper;
        private readonly IRepository<SrmPoH> _srmPohRepository;
        private readonly IRepository<SrmPoL> _srmPolRepository;
        private readonly SRMContext _context;
        IMapper _mapper;

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

        public IEnumerable<SrmPoH> GetAll()
        {
            IEnumerable<SrmPoH> result = _context.SrmPoHs.ToList();
            //foreach (var item in result)
            //{
            //    item.SrmPoLs = (ICollection<SrmPoL>)_srmPolRepository.Get(r => r.PoId == item.PoId);
            //}
            return _context.SrmPoHs.Include(m => m.SrmPoLs).ToList();
        }
        public IEnumerable<SrmPoH> GetAll(QueryPoList query)
        {
            var result = _context.SrmPoHs
                .AndIfCondition(!string.IsNullOrWhiteSpace(query.buyer), p => p.Buyer.IndexOf(query.buyer) > -1)
                .AndIfCondition(!string.IsNullOrWhiteSpace(query.poNum), p => p.PoNum.IndexOf(query.poNum) > -1)
                .AndIfCondition(query.status != 0, p => p.Status == query.status);

            return result.Include(m => m.SrmPoLs).ToList();
        }
        public IEnumerable<ViewSrmPoL> GetPoL(QueryPoList query)
        {
            var result = _context.SrmPoLs.Join(
                _context.SrmPoHs,
                l => l.PoId,
                h => h.PoId,
                (l, h) => new ViewSrmPoL
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
                    TotalAmount = h.TotalAmount,
                    Buyer = h.Buyer,                    
                }
                )
                //.AndIfCondition(!string.IsNullOrWhiteSpace(query.buyer), p => p.Buyer.IndexOf(query.buyer) > -1)
                .AndIfCondition(!string.IsNullOrWhiteSpace(query.poNum), p => p.PoNum.IndexOf(query.poNum) > -1)
                .AndIfHaveValue(query.replyDeliveryDate_s, p => p.DeliveryDate >= query.replyDeliveryDate_s.Value.Date)
                .AndIfHaveValue(query.replyDeliveryDate_e, p => p.DeliveryDate <= query.replyDeliveryDate_e.Value.AddDays(1).Date)
                .AndIfCondition(query.status != 0, p => p.Status == query.status).ToList();

            result.ForEach(p =>
            {
                p.Matnr = _context.SrmMatnrs.Find(p.MatnrId).SapMatnr;
                p.VendorName = _context.SrmVendors.Find(p.VendorId).VendorName;
                p.RemainQty = p.Qty - _context.SrmDeliveryLs.Where(q=>q.PoId==p.PoId &&q.PoLId==p.PoLId).Sum(q => q.DeliveryQty);
            });


            //.AndIfCondition(query.status != 0, p => p.Status == query.status);
            return result.Where(p=>p.RemainQty>0).ToList();
        }
        public IEnumerable<SrmDeliveryH> GetDelivery(QueryPoList query)
        {
            var result = _context.SrmDeliveryHs
    .AndIfCondition(!string.IsNullOrWhiteSpace(query.deliveryNum), p => p.DeliveryNum.IndexOf(query.deliveryNum) > -1)
    .AndIfCondition(query.status != 0, p => p.Status == query.status).ToList();
            result.ForEach(p => {
                p.SrmDeliveryLs = _context.SrmDeliveryLs.Where(m => m.DeliveryId == p.DeliveryId).ToList();
            });
            return result;
        }
        public IEnumerable<SrmPoH> GetMatnrById(int id)
        {
            return _srmPohRepository.Get(r => r.PoId == id);
        }
        public IEnumerable<SrmPoL> GetPolById(int id)
        {
            return _srmPolRepository.Get(r => r.PoId == id);
        }
        public bool UpdateReplyDeliveryDate(SrmPoL data)
        {
            _context.SrmPoLs.Update(data);
            _context.SaveChanges();
            return true;
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
        public bool AddDelivery(List<ViewSrmPoL> data){
            var neworder = _context.SrmDeliveryHs
    .FromSqlRaw("EXECUTE dbo.GetNum {0}", 1).ToList().FirstOrDefault();
            if(neworder==null) return false;
            data.ForEach(p =>
            {
                SrmDeliveryL l = new SrmDeliveryL() {
                    DeliveryId = neworder.DeliveryId,
                    PoId=p.PoId,
                    PoLId=p.PoLId,
                    DeliveryQty=p.DeliveryQty,
                    QmQty=0
                };
                _context.SrmDeliveryLs.Add(l);
            });
            _context.SaveChanges();
            return true;
        }
        public bool CheckAllReply(int id)
        {
            //必須是待接收 或是已接收狀態
            if (_context.SrmPoHs.Find(id).Status == 21 || _context.SrmPoHs.Find(id).Status==11)
            {
                if (_context.SrmPoLs.Any(p => p.PoId == id && p.ReplyDeliveryDate == null))
                {
                    return false;
                }
                return true;
            }            
            return false;
        }
    }
}
