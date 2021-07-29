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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public interface ISrmRfqMService
    {
        public void Save(SrmRfqM rfqM, SRMContext db);
        public IQueryable GetDataByRfqId(int RfqId);
    }
    public class SrmRfqMService:ISrmRfqMService
    {
        private readonly IRepository<SrmRfqM> _srmRfqMRepository;
        private readonly IRepository<SrmMatnr> _srmSrmMatnrRepository;

        public SrmRfqMService(
            //IMapper mapper,
            IRepository<SrmRfqM> srmRfqMRepository, IRepository<SrmMatnr> srmSrmMatnrRepository)
        //SystemIdentityDbUnitOfWork systemIdentityDbUnitOfWork)
        {
            //_mapper = mapper;
            _srmRfqMRepository = srmRfqMRepository;
            _srmSrmMatnrRepository = srmSrmMatnrRepository;
            //_systemIdentityDbUnitOfWork = systemIdentityDbUnitOfWork;
        }
        public void Save(SrmRfqM rfqM, SRMContext db)
        {
            //using (var db = new SRMContext()) {
            if (rfqM.RfqMId == 0)
            {
                db.SrmRfqMs.Add(rfqM);
            }
            else
            {
                db.SrmRfqMs.Update(rfqM);
            }
            //    db.SaveChanges();    
            //}
        }
        public IQueryable GetDataByRfqId(int RfqId)
        {
            var query = from a in _srmRfqMRepository.Get(false)
                        join b in _srmSrmMatnrRepository.Get(false)
                            on a.MatnrId equals b.MatnrId
                        where a.RfqId == RfqId
                        select new
                        {
                            matnr = b.Matnr,
                            version = a.Version,
                            material = a.Material,
                            volume = a.Length+"X"+a.Width+"X"+a.Height,
                            density = a.Density,
                            weight = a.Weight,
                            status = b.Status,
                            qty = a.Qty,
                            machineName = a.MachineName,
                            note = a.Note,
                            length = a.Length,
                            width = a.Width,
                            height = a.Height,
                            rgqId = a.RfqId,
                            rfqMId = a.RfqMId,
                            matnrId = a.MatnrId
                        };
            return query; //_srmRfqMRepository.Get(r => r.RfqId == RfqId);
        }
    }
}
