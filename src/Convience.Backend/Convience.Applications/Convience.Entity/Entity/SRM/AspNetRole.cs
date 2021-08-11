using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class AspNetRole
    {
        public AspNetRole()
        {
            AspNetUserRoles = new HashSet<AspNetUserRole>();
        }

        public int Id { get; set; }
        public string Remark { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; }

        public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; }
    }
}
