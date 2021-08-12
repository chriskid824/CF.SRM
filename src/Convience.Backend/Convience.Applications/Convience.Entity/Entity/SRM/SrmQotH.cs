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
        public SrmQotH()
        {
            //SrmMatnrs = new HashSet<SrmMatnr>();
            //SrmRfqHs = new HashSet<SrmRfqH>();
        }

        public string CreateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public string Currency { get; set; }

        public string LastUpdateBy { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        public double? LeadTime { get; set; }

        public int? MatnrId { get; set; }

        public float? MinQty { get; set; }

        [Key]
        public int QotId { get; set; }

        public string QotNum { get; set; }
        public int? RfqId { get; set; }
        public int? Status { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? VendorId { get; set; }
        //public virtual ICollection<SrmQotH> SrmQotHs { get; set; }
        //public virtual ICollection<SrmMatnr> SrmMatnrs { get; set; }
        // public virtual ICollection<SrmRfqH> SrmRfqHs { get; set; }
    }
}