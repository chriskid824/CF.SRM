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
    public interface ISrmVendorService
    {
        /// <summary>
        /// 取得全部角色
        /// </summary>
        public PagingResultModel<ViewSrmVendor> GetVendor(QueryVendorModel vendorQuery);
        public SrmVendor GetVendorById(int id);
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

        public PagingResultModel<ViewSrmVendor> GetVendor(QueryVendorModel vendorQuery)
        {
            int skip = (vendorQuery.Page-1) * vendorQuery.Size;
            var resultQuery = _srmVendorRepository.Get()
                .AndIfHaveValue(vendorQuery.Vendor, r => r.SrmVendor1.Contains(vendorQuery.Vendor) || r.VendorName.Contains(vendorQuery.Vendor))
                .AndIfHaveValue(vendorQuery.VendorEquals,r=>r.SrmVendor1.Equals(vendorQuery.VendorEquals))
                .AndIfHaveValue(vendorQuery.withoutStatus, r => !vendorQuery.withoutStatus.Contains(r.Status.Value))
                .Where(r => vendorQuery.Werks.Contains(r.Ekorg.Value));
            var vendors =resultQuery.Skip(skip).Take(vendorQuery.Size).ToArray();
            return new PagingResultModel<ViewSrmVendor> 
            {
                Data = JsonConvert.DeserializeObject<ViewSrmVendor[]>(JsonConvert.SerializeObject(vendors)) ,
                Count = resultQuery.Count()
            };
        }

        public SrmVendor GetVendorById(int id)
        {
            return _srmVendorRepository.Get(r => r.VendorId == id).FirstOrDefault();
        }

    }
}
