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
            using (SRMContext db = new SRMContext())
            {
                var qotQurty = db.SrmQotHs.AsQueryable()
                    .AndIfHaveValue(query.rfqId, r => r.RfqId == query.rfqId)
                    .AndIfHaveValue(query.matnrId, r => r.MatnrId == query.matnrId);
                return qotQurty.ToArray();
            }
        }
        
    }
}
