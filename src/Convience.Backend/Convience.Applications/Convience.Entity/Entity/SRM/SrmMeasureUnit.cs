using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmMeasureUnit
    {
        public int MeasureId { get; set; }
        public string MeasureDesc { get; set; }
    }
}
