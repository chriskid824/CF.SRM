using Convience.Entity.Data;
using Convience.EntityFrameWork.Infrastructure;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmRfqV
    {
        public int RfqVId { get; set; }
        public int? RfqId { get; set; }
        public int? VendorId { get; set; }
    }
}
