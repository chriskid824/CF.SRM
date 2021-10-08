using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmMaterialGroup
    {
        public string GroupId { get; set; }
        public string GroupDesc { get; set; }
    }
}
