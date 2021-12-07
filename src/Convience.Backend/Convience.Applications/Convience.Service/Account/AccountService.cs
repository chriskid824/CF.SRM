using Convience.Entity.Entity.Identity;
using Convience.Entity.Entity.SRM;
using Convience.EntityFrameWork.Repositories;
using Convience.JwtAuthentication;
using Convience.Model.Models.Account;
using Convience.Util.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Convience.Service.Account
{
    public interface IAccountService
    {
        public CaptchaResultModel GetCaptcha();

        public string ValidateCaptcha(string captchaKey, string captchaValue);

        public bool IsStopUsing(string userName);

        public Task<ValidateCredentialsResultModel> ValidateCredentialsAsync(string userName, string password);

        public Task<bool> ChangePasswordAsync(string userName, string oldPassword, string newPassword);
    }

    public class AccountService : IAccountService
    {
        private readonly UserManager<SystemUser> _userManager;

        private readonly IRepository<SystemUserRole> _userRoleRepository;

        private readonly IMemoryCache _cachingProvider;

        private readonly IJwtFactory _jwtFactory;

        private readonly SRMContext _srmContext;

        public AccountService(
            UserManager<SystemUser> userManager,
            IRepository<SystemUserRole> userRoleRepository,
            IMemoryCache cachingProvider,
            IOptionsSnapshot<JwtOption> jwtOptionAccessor,
            SRMContext srmContext)
        {
            var option = jwtOptionAccessor.Get(JwtAuthenticationSchemeConstants.DefaultAuthenticationScheme);
            _userManager = userManager;
            _userRoleRepository = userRoleRepository;
            _cachingProvider = cachingProvider;
            _jwtFactory = new JwtFactory(option);
            _srmContext = srmContext;
        }

        public bool IsStopUsing(string userName)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.UserName == userName && u.IsActive);
            return user != null;
        }

        public async Task<ValidateCredentialsResultModel> ValidateCredentialsAsync(string userName, string password)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.UserName == userName && u.IsActive);
            var roles = _userRoleRepository.Get(ur => ur.UserId == user.Id);
            var roleIds = string.Join(',',
                _userRoleRepository.Get(ur => ur.UserId == user.Id).Select(ur => ur.RoleId));
            if (user != null)
            {
                var isValid = await _userManager.CheckPasswordAsync(user, password);
                if (isValid)
                {
                    //int[] werks = _srmContext.SrmEkgries.Where(r => r.Empid == user.UserName).Select(r => r.Werks).ToArray();
                    List<string> roleidarr = roleIds.Split(',').ToList();
                    //var rolenames = _srmContext.AspNetRoles.Where(p => (',' + roleIds + ',').IndexOf(',' + p.Id.ToString() + ',') > -1).Select(p => p.Name).ToList();
                    string rolenames = string.Join(',', _srmContext.AspNetRoles.Where(p => roleidarr.Contains(p.Id.ToString())).Select(p => p.Name));
                    int[] werks = GetWerks(rolenames);
                    string isVendor = CheckIsVendor(user.SapId);
                    string vendorId = isVendor == "1" ? user.SapId : string.Empty;
                    var pairs = new List<(string, string)>
                    {
                        (CustomClaimTypes.UserName,user.UserName),
                        (CustomClaimTypes.UserRoleIds,roleIds),
                        (CustomClaimTypes.Name,user.Name),
                        (CustomClaimTypes.Werks,string.Join(',',werks)),
                        (CustomClaimTypes.VendorId,vendorId),
                        (CustomClaimTypes.IsVendor,isVendor)
                    };

                    return new ValidateCredentialsResultModel(_jwtFactory.GenerateJwtToken(pairs),
 user.Name, user.Avatar, roleIds, user.CostNo, werks, vendorId);

                }
            }
            return null;
        }

        public async Task<bool> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.UserName == userName);
            return user == null ? false :
                (await _userManager.ChangePasswordAsync(user, oldPassword, newPassword)).Succeeded;

        }

        public CaptchaResultModel GetCaptcha()
        {
            var randomValue = CaptchaHelper.GetValidateCode(5);
            var imageData = CaptchaHelper.CreateBase64Image(randomValue);
            var key = Guid.NewGuid().ToString();
            _cachingProvider.Set(key, randomValue, TimeSpan.FromMinutes(2));
            return new CaptchaResultModel(key, imageData);
        }

        public string ValidateCaptcha(string captchaKey, string captchaValue)
        {
            var value = _cachingProvider.Get(captchaKey);
            if (value != null)
            {
                return captchaValue == value.ToString() ? string.Empty : "验证码错误！";
            }
            return "验证码已过期！";
        }
        public string CheckIsVendor(string id)
        {
            //if (Int32.TryParse(id, out int numValue))
            // {
            return _srmContext.SrmVendors.Any(p => p.SapVendor == id &&p.SapVendor!=null) ? "1" : "0";
            //}
            //return "0";
        }
        public int[] GetWerks(string rolenames)
        {
            if (string.IsNullOrEmpty(rolenames)) return null;
            string[] rrolenamelist = rolenames.Split(',');
            List<int> werklist = new List<int>();
            foreach (string name in rrolenamelist)
            {
                if (!string.IsNullOrWhiteSpace(name) && name.Length >= 4)
                {
                    string werk = name.Substring(0, 4);
                    if (werk.All(char.IsDigit))
                    {
                        werklist.Add(Convert.ToInt32(werk));
                    }
                }
            }
            if (werklist.Count == 0) return new int[] { 1100, 1200, 3100 };
            return werklist.ToArray();
        }
    }
}
