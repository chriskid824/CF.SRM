using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmMaterialTrend
    {
        public int TrendId { get; set; }
        public string Material { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? Deadline { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
    }
}
