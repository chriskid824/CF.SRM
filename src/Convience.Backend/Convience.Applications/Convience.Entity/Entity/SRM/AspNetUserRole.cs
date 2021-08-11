using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class AspNetUserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public virtual AspNetRole Role { get; set; }
        public virtual AspNetUser User { get; set; }
    }
}
