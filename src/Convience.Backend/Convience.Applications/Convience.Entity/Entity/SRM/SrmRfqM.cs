using Convience.Entity.Data;
using Convience.EntityFrameWork.Infrastructure;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
        public string Density { get; set; }
        public double? Weight { get; set; }
        public string MachineName { get; set; }
        public double? Qot { get; set; }
        public string Note { get; set; }
    }
}
