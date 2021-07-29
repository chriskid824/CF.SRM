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
        public SrmQotH[] Get(SrmRfqH rfqH);
    }
   public class SrmQotService:ISrmQotService
    {
        private readonly IRepository<SrmQotH> _srmQotHRepository;
        private readonly SRMContext db;

        public SrmQotService(
            IRepository<SrmQotH> srmQotHRepository, IUnitOfWork<SystemIdentityDbContext> unitOfWork)
        {
            _srmQotHRepository = srmQotHRepository;
        }
        public void Add(SrmQotH[] qoths) {

            using (SRMContext db = new SRMContext())
            {
                db.SrmQotHs.AddRange(qoths);
                db.SaveChanges();
            }

            //db.SrmQotHs.AddRange(qoths);
            //db.SaveChanges();
        }
        public void UpdateStatus(int status, SrmRfqH rfqH) {
            var qots = _srmQotHRepository.Get(r => r.RfqId == rfqH.RfqId);
            foreach (var qot in qots) {
                qot.Status = status;
            }
            using (SRMContext db = new SRMContext())
            {
                db.UpdateRange(qots);
                db.SaveChanges();
            }
        }

        public SrmQotH[] Get(SrmRfqH rfqH) {
            using (SRMContext db = new SRMContext())
            {
                return db.SrmQotHs.Where(r => r.RfqId == rfqH.RfqId).ToArray();
            }
        }
    }
}
