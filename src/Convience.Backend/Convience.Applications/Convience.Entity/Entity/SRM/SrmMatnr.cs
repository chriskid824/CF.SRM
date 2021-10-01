using Convience.Entity.Data;
using Convience.EntityFrameWork.Infrastructure;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmMatnr
    {
        public int MatnrId { get; set; }
        public string SrmMatnr1 { get; set; }
        public string SapMatnr { get; set; }
        public string MatnrGroup { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Material { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
        public string Density { get; set; }
        public double? Weight { get; set; }
        public int? Werks { get; set; }
        public int? Status { get; set; }
        public string Note { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
        public string Gewei { get; set; }
        public string Ekgrp { get; set; }
    }
}
