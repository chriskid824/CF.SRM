using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmSurface
    {
        public int SurfaceId { get; set; }
        public string SurfaceDesc { get; set; }
        public int Staus { get; set; }
    }
}
