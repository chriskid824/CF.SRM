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
        //private readonly SystemIdentityDbUnitOfWork _systemIdentityDbUnitOfWork;

        public SrmPoService(
            //IMapper mapper,
            IRepository<SrmPoH> srmPohRepository, IRepository<SrmPoL> srmPolRepository, SRMContext context)
            //SystemIdentityDbUnitOfWork systemIdentityDbUnitOfWork)
        {
            //_mapper = mapper;
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
