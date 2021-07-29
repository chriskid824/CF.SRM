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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public interface ISrmVendorService
    {
        /// <summary>
        /// 取得全部角色
        /// </summary>
        public PagingResultModel<SrmVendor> GetVendor(QueryVendorModel vendorQuery);
    }
    class SrmVendorService:ISrmVendorService
    {
        private readonly IRepository<SrmVendor> _srmVendorRepository;
        //private readonly SystemIdentityDbUnitOfWork _systemIdentityDbUnitOfWork;

        public SrmVendorService(
            //IMapper mapper,
            IRepository<SrmVendor> srmVendorRepository)
        //SystemIdentityDbUnitOfWork systemIdentityDbUnitOfWork)
        {
            //_mapper = mapper;
            _srmVendorRepository = srmVendorRepository;
            //_systemIdentityDbUnitOfWork = systemIdentityDbUnitOfWork;
        }

        public IEnumerable<SrmVendor> GetVendor(string vendor, int page, int size)
        {
            int skip = (page - 1) * size;
            return _srmVendorRepository.Get(r => string.IsNullOrWhiteSpace(vendor) ? true : r.Vendor.IndexOf(vendor) >= 0).ToList().Skip(skip).Take(size).AsQueryable(); ;
        }
        public PagingResultModel<SrmVendor> GetVendor(QueryVendorModel vendorQuery)
        {
            int skip = (vendorQuery.Page - 1) * vendorQuery.Size;
            var resultQuery = _srmVendorRepository.Get().AndIfHaveValue(vendorQuery.Vendor, r => r.Vendor.Contains(vendorQuery.Vendor));
            var vendors =resultQuery.Skip(skip).Take(vendorQuery.Size).ToArray(); ;
            return new PagingResultModel<SrmVendor>
            {
                Data = vendors,
                Count = resultQuery.Count()
            };
        }

        public IEnumerable<SrmVendor> GetVendorById(int id)
        {
            return _srmVendorRepository.Get(r => r.VendorId == id);
        }

    }
}
