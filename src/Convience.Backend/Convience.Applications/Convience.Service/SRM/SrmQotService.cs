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
        public ViewSrmPriceDetail GetDetail(SrmQotH[] query);
    }
    public class SrmQotService : ISrmQotService
    {
        private readonly IRepository<SrmQotH> _srmQotHRepository;
        private readonly SRMContext db;

        public SrmQotService(
            IRepository<SrmQotH> srmQotHRepository, IUnitOfWork<SystemIdentityDbContext> unitOfWork)
        {
            _srmQotHRepository = srmQotHRepository;
        }
        public void Add(SrmQotH[] qoths)
        {

            using (SRMContext db = new SRMContext())
            {
                db.SrmQotHs.AddRange(qoths);
                db.SaveChanges();
            }

            //db.SrmQotHs.AddRange(qoths);
            //db.SaveChanges();
        }
        public void UpdateStatus(int status, SrmRfqH rfqH)
        {
            var qots = _srmQotHRepository.Get(r => r.RfqId == rfqH.RfqId);
            foreach (var qot in qots)
            {
                qot.Status = status;
            }
            using (SRMContext db = new SRMContext())
            {
                db.UpdateRange(qots);
                db.SaveChanges();
            }
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
        public ViewSrmPriceDetail GetDetail(SrmQotH[] query)
        {
            //.AsEnumerable().Select(r => r.Field<string>("Name")).ToArray();
            int[] qotIds = query.AsEnumerable().Select(r => r.QotId).ToArray();
            int[] vendorIds = query.AsEnumerable().Select(r => r.VendorId.Value).ToArray();
            var temp = query.AsEnumerable().Select(r => new { a = r.RfqId, b = r.VendorId }).ToArray();
            ViewSrmPriceDetail price = new ViewSrmPriceDetail();
            price.qot = query;
            using (SRMContext db = new SRMContext())
            {

                price.material = (from material in db.SrmQotMaterial
                                  join qot in db.SrmQotHs
                                  on material.QotId equals qot.QotId
                                  join vendor in db.SrmVendors
                                  on qot.VendorId equals vendor.VendorId
                                  where qotIds.Contains(material.QotId.Value)
                                  select new viewSrmQotMaterial(material)
                                  {
                                      //QotMId = material.QotMId,
                                      //QotId = material.QotId,
                                      //MMaterial = material.MMaterial,
                                      //MPrice = material.MPrice,
                                      //MCostPrice = material.MCostPrice,
                                      //Length = material.Length,
                                      //Width = material.Width,
                                      //Height = material.Height,
                                      //Density = material.Density,
                                      //Weight = material.Weight,
                                      //Note = material.Note,
                                      VendorName = vendor.VendorName
                                  }).ToArray();
                //price.material = (from material in db.SrmQotMaterial
                //           where qotIds.Contains(material.QotId.Value)
                //           select material).ToArray();
                price.process = (from process in db.SrmQotProcesses
                                 join qot in db.SrmQotHs
                                 on process.QotId equals qot.QotId
                                 join vendor in db.SrmVendors
                                 on qot.VendorId equals vendor.VendorId
                                 where qotIds.Contains(process.QotId.Value)
                                 select new viewSrmQotProcess(process)
                                 {
                                     VendorName = vendor.VendorName
                                 }).ToArray();
                //price.process = (from process in db.SrmQotProcesses
                //                             where qotIds.Contains(process.QotId.Value)
                //                             select process).ToArray();
                price.surface = (from surface in db.SrmQotSurfaces
                                 join qot in db.SrmQotHs
                 on surface.QotId equals qot.QotId
                                 join vendor in db.SrmVendors
                                 on qot.VendorId equals vendor.VendorId
                                 where qotIds.Contains(surface.QotId.Value)
                                 select new viewSrmQotSurface(surface)
                                 {
                                     VendorName = vendor.VendorName
                                 }).ToArray();
                price.other = (from other in db.SrmQotOthers
                               join qot in db.SrmQotHs
                 on other.QotId equals qot.QotId
                               join vendor in db.SrmVendors
                               on qot.VendorId equals vendor.VendorId
                               where qotIds.Contains(other.QotId.Value)
                               select new viewSrmQotOther(other) { 
                               VendorName = vendor.VendorName
                               }).ToArray();
                
            }
            return price;
        }
    }
}
