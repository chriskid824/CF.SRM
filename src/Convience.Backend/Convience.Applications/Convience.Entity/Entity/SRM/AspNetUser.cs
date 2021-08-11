using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class AspNetUser
    {
        public AspNetUser()
        {
            AspNetUserRoles = new HashSet<AspNetUserRole>();
        }

        public int Id { get; set; }
        public string Avatar { get; set; }
        public int Sex { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedTime { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string CostNo { get; set; }

        public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; }
    }
}
