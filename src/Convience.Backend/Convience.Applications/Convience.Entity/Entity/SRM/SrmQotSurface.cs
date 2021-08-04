using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmQotSurface
    {
        public int QotSId { get; set; }
        public int? QotId { get; set; }
        public string SProcess { get; set; }
        public double? STimes { get; set; }
        public decimal? SPrice { get; set; }
        public string SNote { get; set; }
    }
}
