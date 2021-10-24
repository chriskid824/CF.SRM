using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmFunctionList
    {
        public int FunctionId { get; set; }
        public string FunctionName { get; set; }
    }
}
