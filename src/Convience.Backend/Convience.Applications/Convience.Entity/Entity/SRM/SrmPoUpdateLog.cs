using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmPoUpdateLog
    {
        public int UId { get; set; }
        public string PoNum { get; set; }
        public int PoLId { get; set; }
        public string SapVendor { get; set; }
        public string SapMatnr { get; set; }
        public string Description { get; set; }
        public string Filename { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
    }
}
