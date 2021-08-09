using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmPoL
    {
        public int PoLId { get; set; }
        public int PoId { get; set; }
        public int MatnrId { get; set; }
        public string Description { get; set; }
        public float? Qty { get; set; }
        public decimal? Price { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ReplyDeliveryDate { get; set; }
        public string DeliveryPlace { get; set; }
        public string CriticalPart { get; set; }
        public float? InspectionTime { get; set; }
        public int? Status { get; set; }

        public virtual SrmPoH Po { get; set; }
    }
}
