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
    public interface ISrmRfqVService
    {
        public void Save(SrmRfqV rfqV, SRMContext db);
        public ViewSrmRfqV[] GetDataByRfqId(int RfqId);
    }
    public class SrmRfqVService : ISrmRfqVService
    {
        private readonly IRepository<SrmRfqV> _srmRfqVRepository;
        private readonly IRepository<SrmVendor> _srmVendorRepository;

        public SrmRfqVService(
            //IMapper mapper,
            IRepository<SrmRfqV> srmRfqVRepository, IRepository<SrmVendor> srmVendorRepository)
        //SystemIdentityDbUnitOfWork systemIdentityDbUnitOfWork)
        {
            //_mapper = mapper;
            _srmRfqVRepository = srmRfqVRepository;
            _srmVendorRepository = srmVendorRepository;
            //_systemIdentityDbUnitOfWork = systemIdentityDbUnitOfWork;
        }
        public void Save(SrmRfqV rfqV, SRMContext db)
        {
            //using (var db = new SRMContext()) {
                if (rfqV.RfqVId == 0)
                {
                    db.SrmRfqVs.Add(rfqV);
                }
                else
                {
                    db.SrmRfqVs.Update(rfqV);
                }
                //db.SaveChanges();
            //}
        }
        public ViewSrmRfqV[] GetDataByRfqId(int RfqId)
        {
            //var z = _srmRfqVRepository.Get(r => r.RfqId == RfqId).Join(new SRMContext().SrmVendors, a => a.RfqVId, b => b.VendorId, (a, b) => new
            //{
            //    vendor = b.Vendor,
            //    vendorName = b.VendorName,
            //    person = b.Person,
            //    address = b.Address,
            //    telphone = b.TelPhone,
            //    ext = b.Ext,
            //    faxnumber = b.FaxNumber,
            //    cellphone = b.CellPhone,
            //    mail = b.Mail,
            //    status = b.Status,
            //    rfqVId = a.RfqVId,
            //    rfqId = a.RfqId,
            //    vendorId = a.VendorId
            //});
            var query = from a in _srmRfqVRepository.Get(false)
                        join b in _srmVendorRepository.Get(false)
                            on a.VendorId equals b.VendorId
                        where a.RfqId == RfqId
                        select new ViewSrmRfqV
                        {
                            Vendor = b.Vendor,
                            VendorName = b.VendorName,
                            Person = b.Person,
                            Address = b.Address,
                            TelPhone = b.TelPhone,
                            Ext = b.Ext,
                            FaxNumber = b.FaxNumber,
                            CellPhone = b.CellPhone,
                            Mail = b.Mail,
                            Status = b.Status,
                            rfqVId = a.RfqVId,
                            rfqId = a.RfqId.Value,
                            vendorId = a.VendorId.Value
                        };
            return query.ToArray();
        }
    }
}
