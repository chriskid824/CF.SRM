using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmTaxcode
    {
        public string Taxcode { get; set; }
        public string TaxcodeName { get; set; }
    }
}
