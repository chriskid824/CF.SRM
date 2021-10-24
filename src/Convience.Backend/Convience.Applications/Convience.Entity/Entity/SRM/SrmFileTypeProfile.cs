using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmFileTypeProfile
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
    }
}
