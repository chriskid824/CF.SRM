using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmDisscussionH
    {
        public SrmDisscussionH()
        {
            SrmDisscussionCs = new HashSet<SrmDisscussionC>();
        }

        public int DisscussionId { get; set; }
        public int? TemplateType { get; set; }
        public string Number { get; set; }
        public string Title { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }

        public virtual ICollection<SrmDisscussionC> SrmDisscussionCs { get; set; }
    }
}
