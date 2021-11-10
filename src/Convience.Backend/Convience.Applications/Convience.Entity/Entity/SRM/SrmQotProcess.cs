using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmQotProcess
    {
        public int QotPId { get; set; }
        public int? QotId { get; set; }
        public string PProcessNum { get; set; }
        public double? PHours { get; set; }
        public decimal? PPrice { get; set; }
        public string PMachine { get; set; }
        public string PNote { get; set; }
        public decimal? PCostsum { get; set; }
        
    }
}
