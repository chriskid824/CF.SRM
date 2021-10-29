using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmFileuploadRecordH
    {
        public SrmFileuploadRecordH()
        {
            SrmFileuploadRecordLs = new HashSet<SrmFileuploadRecordL>();
        }

        public int RecordHId { get; set; }
        public int? TemplateId { get; set; }
        public string Number { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }

        public virtual ICollection<SrmFileuploadRecordL> SrmFileuploadRecordLs { get; set; }
    }
}
