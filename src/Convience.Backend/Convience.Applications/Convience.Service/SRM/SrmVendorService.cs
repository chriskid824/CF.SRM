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
using Convience.Service.SystemManage;
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
        public Task<string> AddUserFromVendor();
        public Task<string> DefaultPassword();
        public List<AspNetUser> GetUsersByVendor(UserClaims user);
    }
    class SrmVendorService:ISrmVendorService
    {
        private readonly IRepository<SrmVendor> _srmVendorRepository;
        //private readonly SystemIdentityDbUnitOfWork _systemIdentityDbUnitOfWork;
        private readonly SRMContext _context;
        private readonly IUserService _userService;
        public SrmVendorService(
            //IMapper mapper,
            IRepository<SrmVendor> srmVendorRepository, SRMContext context, IUserService userService)
        //SystemIdentityDbUnitOfWork systemIdentityDbUnitOfWork)
        {
            //_mapper = mapper;
            _srmVendorRepository = srmVendorRepository;
            _context = context;
            _userService = userService;
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
        public async Task<string> AddUserFromVendor()
        {
            List<UserViewModel> users = (from vendors in _context.SrmVendors
                                         select new UserViewModel()
                                         {
                                             Avatar="",
                                             UserName= vendors.SrmVendor1,
                                             PhoneNumber=vendors.TelPhone,
                                             Email=vendors.Mail,
                                             Sex=0,
                                             Name=vendors.VendorName,
                                             RoleIds="4",
                                             IsActive=true,
                                             Password="Ab123456",
                                             SapId= vendors.SrmVendor1,
                                             PositionIds="",
                                         }).ToList();
            foreach (var user in users)
            {
                if (!_context.AspNetUsers.Any(p => p.UserName == user.UserName))
                {
                    var result = _userService.AddUserAsync(user);
                }
            }
            return null;
        }
        public async Task<string> DefaultPassword()
        {
            List<int> users = _context.AspNetUsers.Where(p => p.PasswordHash == null).Select(p => p.Id).ToList();

            //_userService.SetPasswordAsync(new UserPasswordModel(user, "Ab123456"));
            foreach (var id in users)
            {
                var result = await _userService.SetPasswordAsync(new UserPasswordModel(id, "Ab123456"));

            }
            return null;
        }
        public List<AspNetUser> GetUsersByVendor(UserClaims user)
        {
            return null;
            //from u in _context.AspNetUsers
            //join v in _context.SrmVendors on u.SapId equals v.VendorId
            //select u;
        }
    }
}
