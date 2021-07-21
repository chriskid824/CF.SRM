using Convience.Entity.Data;
using Convience.EntityFrameWork.Infrastructure;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#nullable disable
namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmVendor
    {
        public int VendorId { get; set; }
        public string SrmVendor1 { get; set; }
        public string Vendor { get; set; }
        public string VendorName { get; set; }
        public string Person { get; set; }
        public string Address { get; set; }
        public string TelPhone { get; set; }
        public string Ext { get; set; }
        public string FaxNumber { get; set; }
        public string CellPhone { get; set; }
        public string Mail { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
    }
}
