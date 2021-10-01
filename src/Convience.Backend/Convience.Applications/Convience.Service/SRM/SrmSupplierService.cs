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
using Convience.Model.Models.SRM;
using Newtonsoft.Json;
using Convience.Service.SystemManage;

namespace Convience.Service.SRM
{
    public interface ISrmSupplierService
    {
        /// <summary>
        /// 取得全部角色
        /// </summary>
        //public PagingResultModel<ViewSrmSupplier> GetVendor();
        public PagingResultModel<ViewSrmSupplier> GetVendor(QueryVendorModel vendorQuery);

        public ViewSrmSupplier GetSupplierDetail(QueryVendorModel query);

        public bool UpdateSupplier(ViewSrmSupplier data);
        public bool CheckVendor(ViewSrmSupplier data);
        public bool AddVendor(ViewSrmSupplier data);

    }
    public class SrmSupplierService : ISrmSupplierService
    {
        private readonly SRMContext _context;

        private readonly IRepository<SrmSupplier> _srmVendorRepository;




        public SrmSupplierService(IRepository<SrmSupplier> srmVendorRepository, SRMContext context)
        {
            //_mapper = mapper;
            _srmVendorRepository = srmVendorRepository;
            _context = context;
        }


        public PagingResultModel<ViewSrmSupplier> GetVendor(QueryVendorModel vendorQuery)
        {
            int skip = (vendorQuery.Page-1) * vendorQuery.Size;

            var resultQuery = (from vendor in _context.SrmVendors
                          join status in _context.SrmStatuses on vendor.Status equals status.Status
                          select new ViewSrmSupplier
                          {
                              SrmVendor1 = vendor.SrmVendor1,
                              VendorName = vendor.VendorName,
                              Org = vendor.Org,
                              Ekorg = vendor.Ekorg,
                              Person = vendor.Person,
                              Address = vendor.Address,
                              TelPhone = vendor.TelPhone,
                              Ext = vendor.Ext,
                              FaxNumber = vendor.FaxNumber,
                              CellPhone = vendor.CellPhone,
                              Mail = vendor.Mail,
                              StatusDesc = status.StatusDesc, 
                          })
                          .AndIfHaveValue(vendorQuery.Code, r => r.SrmVendor1.Contains(vendorQuery.Code))
                          .AndIfHaveValue(vendorQuery.Vendor, r => r.VendorName.Contains(vendorQuery.Vendor))
                .Where(r => vendorQuery.Werks.Contains(r.Ekorg.Value));
            
            var vendors = resultQuery.Skip(skip).Take(vendorQuery.Size).ToArray();

            return new PagingResultModel<ViewSrmSupplier>
            {
                Data = JsonConvert.DeserializeObject<ViewSrmSupplier[]>(JsonConvert.SerializeObject(vendors)),
                Count = resultQuery.Count()
            };
        }
        public ViewSrmSupplier GetSupplierDetail(QueryVendorModel query)
        {
            var supplier = (from vendor in _context.SrmVendors
                            join status in _context.SrmStatuses on vendor.Status equals status.Status
                            select new ViewSrmSupplier
                            {
                                SrmVendor1 = vendor.SrmVendor1,
                                VendorName = vendor.VendorName,
                                Org = vendor.Org,
                                Ekorg = vendor.Ekorg,
                                Person = vendor.Person,
                                Address = vendor.Address,
                                TelPhone = vendor.TelPhone,
                                Ext = vendor.Ext,
                                FaxNumber = vendor.FaxNumber,
                                CellPhone = vendor.CellPhone,
                                Mail = vendor.Mail,
                                StatusDesc = status.StatusDesc,
                            })
                          .Where(r => r.SrmVendor1 == query.Code).FirstOrDefault()
                          //.Where(r => r.Org == query.Org)
                          //.Where(r => r.Ekorg == query.Ekorg)
                          ;

            //var suppliers = supplier.ToArray();
            //
            return new ViewSrmSupplier
            {
                SrmVendor1 = supplier.SrmVendor1,
                VendorName = supplier.VendorName,
                Org = supplier.Org,
                Ekorg = supplier.Ekorg,
                Person = supplier.Person,
                Address = supplier.Address,
                TelPhone = supplier.TelPhone,
                Ext = supplier.Ext,
                FaxNumber = supplier.FaxNumber,
                CellPhone = supplier.CellPhone,
                Mail = supplier.Mail,
                StatusDesc = supplier.StatusDesc,
            };
        }
        public bool UpdateSupplier(ViewSrmSupplier data)
        {

            SrmVendor vendor = _context.SrmVendors.Where(p => p.SrmVendor1 == data.SrmVendor1).FirstOrDefault();
            SrmStatus status = _context.SrmStatuses.Where(p => p.StatusDesc == data.StatusDesc).FirstOrDefault();

            if (status == null || vendor==null)
            {
                return false;
            }


            vendor.SrmVendor1 = data.SrmVendor1;
            vendor.VendorName = data.VendorName;
            vendor.Org = data.Org;
            vendor.Ekorg = data.Ekorg;
            vendor.Person = data.Person;
            vendor.Address = data.Address;
            vendor.TelPhone = data.TelPhone;
            vendor.Ext = data.Ext;
            vendor.FaxNumber = data.FaxNumber;
            vendor.CellPhone = data.CellPhone;
            vendor.Mail = data.Mail;
            vendor.Status = status.Status;
            vendor.LastUpdateDate = DateTime.Now;
            vendor.LastUpdateBy = data.User;


            _context.SrmVendors.Update(vendor);

            _context.SaveChanges();

            return true;
        }
        public bool CheckVendor(ViewSrmSupplier data)
        {
            SrmVendor vendor = _context.SrmVendors.Where(p => p.SrmVendor1 == data.SrmVendor1).FirstOrDefault();
            if (vendor == null)
            {
                return true;
            }

            return false;
        }
        public bool AddVendor(ViewSrmSupplier data)
        {
            SrmVendor vendor = _context.SrmVendors.Where(p => p.SrmVendor1 == data.SrmVendor1).FirstOrDefault();
            //SrmStatus status = _context.SrmStatuses.Where(p => p.StatusDesc == data.StatusDesc).FirstOrDefault();

            if (vendor != null)
            {
                return false;
            }


            SrmVendor srmvendor = new SrmVendor()
            {
                SrmVendor1 = data.SrmVendor1,
                VendorName = data.VendorName,
                Org = data.Org,
                Ekorg = data.Ekorg,
                Person = data.Person,
                Address = data.Address,
                TelPhone = data.TelPhone,
                Ext = data.Ext,
                FaxNumber = data.FaxNumber,
                CellPhone = data.CellPhone,
                Mail = data.Mail,
                Status = 1,

                CreateDate = DateTime.Now,
                CreateBy = data.User,
                LastUpdateDate = DateTime.Now,
                LastUpdateBy = data.User,
            };


            _context.SrmVendors.Add(srmvendor);
            _context.SaveChanges();


            return true;
        }

    }
}
