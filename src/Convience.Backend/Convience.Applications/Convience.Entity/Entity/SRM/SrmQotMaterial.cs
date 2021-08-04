using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmQotMaterial
    {
        public int QotMId { get; set; }
        public int? QotId { get; set; }
        public string MMaterial { get; set; }
        public decimal? MPrice { get; set; }
        public decimal? MCostPrice { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
        public double? Density { get; set; }
        public double? Weight { get; set; }
        public string Note { get; set; }
    }
}
