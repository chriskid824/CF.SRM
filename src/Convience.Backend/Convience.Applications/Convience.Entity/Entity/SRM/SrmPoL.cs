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
        public DateTime? OriginalDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ReplyDeliveryDate { get; set; }
        public string DeliveryPlace { get; set; }
        public string Storage { get; set; }
        public string StorageDesc { get; set; }
        public string Cell { get; set; }
        public string CriticalPart { get; set; }
        public float? InspectionTime { get; set; }
        public float? InspectionQty { get; set; }
        public int? Status { get; set; }
        public string WoNum { get; set; }
        public int? WoItem { get; set; }
        public string OtherDesc { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
        public int? WoQty { get; set; }
        public DateTime? LastDeliveryDate { get; set; }
        public DateTime? LastReplyDeliveryDate { get; set; }
        public string ChangeDateReason { get; set; }
        public DateTime? EstDeliveryDate { get; set; }
        public string EstDeliveryDays { get; set; }
        public string InspectionReport { get; set; }
        public string SipReport { get; set; }
        public string Bednr { get; set; }
        public string Ebban { get; set; }
        public string Wzb04 { get; set; }
        public string Neindt { get; set; }
        public string Tdline { get; set; }
        public string Zpano { get; set; }
        public string Bedat { get; set; }

        public virtual SrmPoH Po { get; set; }
    }
}
