using Convience.Entity.Data;
using Convience.Entity.Entity.SRM;
using Convience.EntityFrameWork.Infrastructure;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmMaterial
    {
        public int MaterialNum { get; set; }
        public string Material { get; set; }
        public int? Staus { get; set; }
    }
}
