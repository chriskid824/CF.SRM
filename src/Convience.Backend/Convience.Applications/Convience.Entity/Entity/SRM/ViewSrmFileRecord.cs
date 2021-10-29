using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    public partial class ViewSrmFileRecord
    {
        public int RecordHId { get; set; }
        public int? TemplateId { get; set; }
        public string Number { get; set; }
        public int? TemplateType { get; set; }
        public string FunctionName { get; set; }
        public int? Werks { get; set; }
        public int? Type { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? Deadline { get; set; }
        public string Filetype { get; set; }
        public string TypeName { get; set; }
        public int? RecordLId { get; set; }
        public string Url { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
    }
}
