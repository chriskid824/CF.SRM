using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmRfqM
    {
        public int RfqMId { get; set; }
        public int? RfqId { get; set; }
        public int? MatnrId { get; set; }
        public string Version { get; set; }
        public string Material { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Density { get; set; }
        public double? Weight { get; set; }
        public string Gewei { get; set; }
        public string MachineName { get; set; }
        public double? Qty { get; set; }
        public string Note { get; set; }
        public string Bn_num { get; set; }
        public string Major_diameter { get; set; }
        public string Minor_diameter { get; set; }
        public string Description { get; set; }
    }
}
