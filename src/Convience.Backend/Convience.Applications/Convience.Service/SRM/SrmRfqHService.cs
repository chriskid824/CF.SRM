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
    public interface ISrmRfqHService
    {
        public void Save(SrmRfqH rfqH);
        public void Save(SrmRfqH rfqH, SrmRfqM[] rfqMs, SrmRfqV[] rfqVs);
        public SrmRfqH GetDataByRfqId(int RfqId);
    }
    public class SrmRfqHService:ISrmRfqHService
    {
        private readonly IRepository<SrmRfqH> _srmRfqHRepository;
        public SrmRfqHService(
            //IMapper mapper,
            IRepository<SrmRfqH> srmRfqHRepository)
        //SystemIdentityDbUnitOfWork systemIdentityDbUnitOfWork)
        {
            //_mapper = mapper;
            _srmRfqHRepository = srmRfqHRepository;
            //_systemIdentityDbUnitOfWork = systemIdentityDbUnitOfWork;
        }
        public void Save(SrmRfqH rfqH)
        {
            using (var db = new SRMContext()) {
                if (rfqH.RfqId == 0)
                {
                    db.SrmRfqHs.Add(rfqH);
                }
                else
                {
                    db.SrmRfqHs.Update(rfqH);
                }
                db.SaveChanges();
            }
        }

        public void Save(SrmRfqH rfqH, SrmRfqM[] rfqMs, SrmRfqV[] rfqVs)
        {
            using (var db = new SRMContext())
            {
                db.Database.BeginTransaction();
                try
                {
                    if (rfqH.RfqId == 0)
                    {
                        db.SrmRfqHs.Add(rfqH);
                        db.SaveChanges();
                        rfqH.RfqNum = "V" + rfqH.RfqId.ToString().PadLeft(6,'0');
                        rfqH.CreateDate = rfqH.LastUpdateDate;
                        foreach (var rfqM in rfqMs)
                        {
                            rfqM.RfqId = rfqH.RfqId;
                        }
                        foreach (var rfqV in rfqVs)
                        {
                            rfqV.RfqId = rfqH.RfqId;
                        }
                        db.SrmRfqHs.Update(rfqH);
                        db.SrmRfqMs.AddRange(rfqMs);
                        db.SrmRfqVs.AddRange(rfqVs);
                    }
                    else
                    {
                        db.SrmRfqHs.Update(rfqH);
                        var oldRfqMs = db.SrmRfqMs.Where(r => r.RfqId == rfqH.RfqId);
                        foreach (var oldrfqM in oldRfqMs)
                        {
                            if (rfqMs.AsEnumerable().Where(item => item.RfqMId == oldrfqM.RfqMId).Count() == 0)
                            {
                                db.SrmRfqMs.Remove(oldrfqM);
                            }
                        }
                        foreach (var rfqM in rfqMs) {
                            rfqM.RfqId = rfqH.RfqId;
                            if (rfqM.RfqMId == 0)
                            {
                                db.SrmRfqMs.Add(rfqM);
                            }
                            else
                            {
                                db.SrmRfqMs.Update(rfqM);
                            }
                        }
                        var oldRfqVs = db.SrmRfqVs.Where(r => r.RfqId == rfqH.RfqId);
                        foreach (var oldrfqV in oldRfqVs)
                        {
                            if (rfqVs.AsEnumerable().Where(item => item.RfqVId == oldrfqV.RfqVId).Count() == 0)
                            {
                                db.SrmRfqVs.Remove(oldrfqV);
                            }
                        }
                        foreach (var rfqV in rfqVs)
                        {
                            rfqV.RfqId = rfqH.RfqId;
                            if (rfqV.RfqVId == 0)
                            {
                                db.SrmRfqVs.Add(rfqV);
                            }
                            else
                            {
                                db.SrmRfqVs.Update(rfqV);
                            }
                        }
                    }
                    db.SaveChanges();
                    db.Database.CommitTransaction();
                }
                catch (Exception ex)
                {
                    db.Database.RollbackTransaction();
                    throw ex;
                }
            }
        }
        //public SrmRfqH GetDataByRfqId()
        //{
        //    return _srmRfqHRepository.Get();
        //}

        public SrmRfqH GetDataByRfqId(int RfqId)
        {
            return _srmRfqHRepository.Get(r => r.RfqId == RfqId).First();
        }
    }
}
