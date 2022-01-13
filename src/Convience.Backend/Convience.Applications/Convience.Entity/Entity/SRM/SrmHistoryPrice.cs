using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmHistoryPrice
    {
        public int HistoryId { get; set; }
        public int? Ekorg { get; set; }
        public string Matnr { get; set; }
        public string Vendor { get; set; }
        public string Essay { get; set; }
        public string Ekgry { get; set; }
        public string Inforecord { get; set; }
        public double? OrderQty { get; set; }
        public string OrderUnit { get; set; }
        public decimal? HistoryPrice { get; set; }
        public string Currency { get; set; }
        public int? PriceUnit { get; set; }
        public double? UnpaidQty { get; set; }
        public decimal? UnpaidPrice { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? TargetPrice { get; set; }
    }
}
