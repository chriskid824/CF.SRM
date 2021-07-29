using Convience.Entity.Data;
using Convience.EntityFrameWork.Infrastructure;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmQotH
    {
        [Key]
        public int QotId { get; set; }
        public string QotNum { get; set; }
        public int? VendorId { get; set; }
        public int? MatnrId { get; set; }
        public int? RfqId { get; set; }
        public string Currency { get; set; }
        public double? LeadTime { get; set; }
        public double? MinQty { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
    }
}
