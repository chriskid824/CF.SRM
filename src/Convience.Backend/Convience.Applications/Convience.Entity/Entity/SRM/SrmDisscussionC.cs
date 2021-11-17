using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmDisscussionC
    {
        public int DisscussionId { get; set; }
        public int DisscussionIdC { get; set; }
        public string DisscustionContent { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
        //[JsonIgnore]
        //[IgnoreDataMember]
    }
}
