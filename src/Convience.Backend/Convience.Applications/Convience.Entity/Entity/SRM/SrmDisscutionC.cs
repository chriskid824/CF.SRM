using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmDisscutionC
    {
        public int DisscustionId { get; set; }
        public int DisscustionIdC { get; set; }
        public string DisscustionContent { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }

        public virtual SrmDisscutionH Disscustion { get; set; }
    }
}
