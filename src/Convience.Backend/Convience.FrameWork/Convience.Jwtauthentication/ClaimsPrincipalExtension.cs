
using System.Linq;
using System.Security.Claims;

namespace Convience.JwtAuthentication
{
    public class UserClaims
    {
        public int[] Werks;
        public string UserRoleIds;
        public string Name;
        public string UserName;
        public string VendorId;
        public bool IsVendor;
    }
    public static class ClaimsPrincipalExtension
    {
        //名稱
        public static string GetName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Name)
                ?.Value ?? string.Empty;
        }
        //帳號
        public static string GetUserName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserName)
                ?.Value ?? string.Empty;
        }

        public static string GetUserRoleIds(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserRoleIds)
                ?.Value ?? string.Empty;
        }
        public static string GetUserWerks(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Werks)
                ?.Value ?? string.Empty;
        }
        public static string GetVendorId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.VendorId)
                ?.Value ?? string.Empty;
        }
        public static bool GetIsVendor(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.IsVendor)
                ?.Value == "1";
        }

        public static string GetUserSchema(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserSchema)
                ?.Value ?? string.Empty;
        }
        public static UserClaims GetUserClaims(this ClaimsPrincipal claimsPrincipal)
        {
            var userclaim = new UserClaims()
            {
                Name = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Name)
                ?.Value ?? string.Empty,
                UserName = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserName)
                ?.Value ?? string.Empty,
                UserRoleIds = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserRoleIds)
                ?.Value ?? string.Empty,
                Werks = null,
                VendorId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.VendorId)
                ?.Value ?? string.Empty,
                IsVendor = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.IsVendor)
                ?.Value == "1"
            };
            string Werks = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Werks)
 ?.Value ?? null;
            if (!string.IsNullOrWhiteSpace(Werks))
            {
                userclaim.Werks = Werks.Split(',').Select(n => System.Convert.ToInt32(n)).ToArray();
            }
            return userclaim;
        }
    }
}
