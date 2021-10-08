using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmWeightUnit
    {
        public string UnitId { get; set; }
        public string UnitDesc { get; set; }
    }
}
