using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmQotOther
    {
        public int QotOId { get; set; }
        public int? QotId { get; set; }
        public string OItem { get; set; }
        public string ODescription { get; set; }
        public decimal? OPrice { get; set; }
        public string ONote { get; set; }
    }
}
