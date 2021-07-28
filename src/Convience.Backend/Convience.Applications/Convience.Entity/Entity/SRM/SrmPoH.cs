using Convience.EntityFrameWork.Infrastructure;

using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmPoH
    {
        public SrmPoH()
        {
            SrmPoLs = new HashSet<SrmPoL>();
        }

        public int PoId { get; set; }
        public string PoNum { get; set; }
        public int VendorId { get; set; }
        public int? Status { get; set; }
        public string Buyer { get; set; }
        public int? Org { get; set; }
        public DateTime? DocDate { get; set; }
        public DateTime? ReplyDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
        public decimal? TotalAmount { get; set; }

        public virtual ICollection<SrmPoL> SrmPoLs { get; set; }
    }
}
