using Convience.Entity.Data;
using Convience.EntityFrameWork.Infrastructure;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmRfqH
    {
        public int RfqId { get; set; }
        public string RfqNum { get; set; }
        public int? Status { get; set; }
        public string Sourcer { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? EndDate { get; set; }
        public string EndBy { get; set; }
        public int? Werks { get; set; }
    }
}
