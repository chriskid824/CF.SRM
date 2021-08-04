using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmInforecord
    {
        public int InfoId { get; set; }
        public int? InfoNum { get; set; }
        public int? QotId { get; set; }
        public int? MatnrId { get; set; }
        public int? VendorId { get; set; }
        public decimal? Price { get; set; }
        public int? Unit { get; set; }
        public string Currency { get; set; }
        public double? LeadTime { get; set; }
        public double? StandQty { get; set; }
        public double? MinQty { get; set; }
        public string Ekgry { get; set; }
        public string Taxcode { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
    }
}
