using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

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
        public string Length { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
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
        public string Bn_num { get; set; }
        public string Major_diameter { get; set; }
        public string Minor_diameter { get; set; }
        public int? Unit { get; set; }
    }
}
