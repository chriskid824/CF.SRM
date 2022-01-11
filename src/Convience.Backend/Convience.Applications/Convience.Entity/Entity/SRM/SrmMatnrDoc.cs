using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmMatnrDoc
    {
        public int MdocId { get; set; }
        public int? MatnrId { get; set; }
        public string Description { get; set; }
        public int? VendorId { get; set; }
        public string Filename { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
    }
}
