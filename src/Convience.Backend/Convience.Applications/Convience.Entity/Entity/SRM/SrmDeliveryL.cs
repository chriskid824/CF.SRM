using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmDeliveryL
    {
        public int DeliveryLId { get; set; }
        public int? DeliveryId { get; set; }
        public int? PoId { get; set; }
        public int? PoLId { get; set; }
        public float? DeliveryQty { get; set; }
        public float? QmQty { get; set; }

        public virtual SrmDeliveryH Delivery { get; set; }
    }
}
