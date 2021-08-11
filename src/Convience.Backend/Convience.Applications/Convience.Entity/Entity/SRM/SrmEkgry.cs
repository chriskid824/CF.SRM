using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmEkgry
    {
        public string Ekgry { get; set; }
        public string EkgryDesc { get; set; }
        public int Werks { get; set; }
        public string Empid { get; set; }
    }
}
