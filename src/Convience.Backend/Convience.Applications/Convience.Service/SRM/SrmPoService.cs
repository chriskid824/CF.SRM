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
        public bool UpdateReplyDeliveryDate(SrmPoL data);
        public bool UpdateStatus(int id, int status);
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
            return _context.SrmPoHs.Include(m=>m.SrmPoLs).ToList();
        }
        public IEnumerable<SrmPoH> GetAll(QueryPoList query)
        {
            var result = _context.SrmPoHs
                .AndIfCondition(!string.IsNullOrWhiteSpace(query.buyer), p => p.Buyer.IndexOf(query.buyer) >-1)
                .AndIfCondition(!string.IsNullOrWhiteSpace(query.poNum), p => p.PoNum.IndexOf(query.poNum) >-1)
                .AndIfCondition(query.status != 0, p=>p.Status == query.status);
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
                    InspectionTime = l.InspectionTime
                }
                )
                //.AndIfCondition(!string.IsNullOrWhiteSpace(query.buyer), p => p.Buyer.IndexOf(query.buyer) > -1)
                .AndIfCondition(!string.IsNullOrWhiteSpace(query.poNum), p => p.PoNum.IndexOf(query.poNum) > -1);
                //.AndIfCondition(query.status != 0, p => p.Status == query.status);
            return result.ToList();
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
        public bool UpdateStatus(int id,int status)
        {
            SrmPoH data = _context.SrmPoHs.Find(id);
            data.Status = status;
            if (status == 11)
            {
                data.ReplyDate = DateTime.Now;
            }
            _context.SrmPoHs.Update(data);
            _context.SaveChanges();
            return true;
        }
        public bool CheckAllReply(int id)
        {
            if (_context.SrmPoLs.Any(p => p.PoId == id && p.ReplyDeliveryDate == null))
            {
                return false;
            }
            return true;
        }
    }
}
