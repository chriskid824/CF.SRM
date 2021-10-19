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
        public ViewSrmRfqM GetRfqMData(SrmRfqM rfq);
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
                            srmMatnr1 = b.SrmMatnr1,
                            sapMatnr = b.SapMatnr,
                            version = a.Version,
                            material = a.Material,
                            volume = a.Length+"*"+a.Width+"*"+a.Height,
                            density = a.Density,
                            weight = a.Weight,
                            status = b.Status,
                            qty = a.Qty,
                            machineName = a.MachineName,
                            note = a.Note,
                            length = a.Length,
                            width = a.Width,
                            height = a.Height,
                            rfqId = a.RfqId,
                            rfqMId = a.RfqMId,
                            matnrId = a.MatnrId,
                            viewstatus = ((Status)b.Status.Value).ToString(),
                            Description = a.Description,
                            Bn_num = a.Bn_num,
                            Major_diameter = a.Major_diameter,
                            Minor_diameter = a.Minor_diameter
                        };
            return query; //_srmRfqMRepository.Get(r => r.RfqId == RfqId);
        }
        public ViewSrmRfqM GetRfqMData(SrmRfqM rfq) {
            var query = from a in _srmRfqMRepository.Get(false)
                        join b in _srmSrmMatnrRepository.Get(false)
                            on a.MatnrId equals b.MatnrId
                        where a.RfqId == rfq.RfqId && a.MatnrId == rfq.MatnrId
                        select new ViewSrmRfqM
                        {
                            matnr = b.SapMatnr,
                            Version = a.Version,
                            Material = a.Material,
                            volume = a.Length + "*" + a.Width + "*" + a.Height,
                            Density = a.Density,
                            Weight = a.Weight,
                            status = b.Status.Value,
                            Qty = a.Qty,
                            MachineName = a.MachineName,
                            Note = a.Note,
                            Length = a.Length,
                            Width = a.Width,
                            Height = a.Height,
                            RfqId = a.RfqId,
                            RfqMId = a.RfqMId,
                            MatnrId = a.MatnrId,
                            srmMatnr = b.SrmMatnr1,
                            Description = a.Description,
                            Major_diameter = a.Major_diameter,
                            Minor_diameter = a.Minor_diameter,
                            Bn_num = a.Bn_num
                        };
            return query.FirstOrDefault();
        }
    }
}
