using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    public partial class ViewSrmFileTemplate
    {
        public int? RecordHId { get; set; }
        public int TemplateId { get; set; }
        public int? Number { get; set; }
        public int TemplateType { get; set; }
        public string FunctionName { get; set; }
        public int? Werks { get; set; }
        public int Type { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? Deadline { get; set; }
        public string Filetype { get; set; }
        public string TypeName { get; set; }
        public int? RecordLId { get; set; }
        public int? Url { get; set; }
        public int? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        public int? LastUpdateDate { get; set; }
        public int? LastUpdateBy { get; set; }
    }
}
