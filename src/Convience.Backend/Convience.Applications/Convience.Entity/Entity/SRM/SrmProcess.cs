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
    public partial class SrmProcess
    {
        public int ProcessNum { get; set; }
        public string Process { get; set; }
        public int? Staus { get; set; }
    }
}
