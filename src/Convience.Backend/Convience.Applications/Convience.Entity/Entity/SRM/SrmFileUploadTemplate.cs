using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmFileUploadTemplate
    {
        public int TemplateId { get; set; }
        public int TemplateType { get; set; }
        public int? Werks { get; set; }
        public int Type { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? Deadline { get; set; }
        public string Filetype { get; set; }
        public int? Filetype1 { get; set; }
        public int? Filetype2 { get; set; }
        public int? Filetype3 { get; set; }
        public int? Filetype4 { get; set; }
        public int? Filetype5 { get; set; }
        public int? Filetype6 { get; set; }
        public int? Filetype7 { get; set; }
        public int? Filetype8 { get; set; }
        public int? Filetype9 { get; set; }
        public int? Filetype10 { get; set; }
        public int? Filetype11 { get; set; }
        public int? Filetype12 { get; set; }
        public int? Filetype13 { get; set; }
        public int? Filetype14 { get; set; }
        public int? Filetype15 { get; set; }
        public int? Filetype16 { get; set; }
        public int? Filetype17 { get; set; }
        public int? Filetype18 { get; set; }
        public int? Filetype19 { get; set; }
        public int? Filetype20 { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
    }
}
