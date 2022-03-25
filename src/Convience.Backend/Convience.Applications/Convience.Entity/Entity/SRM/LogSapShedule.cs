using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    [Entity(DbContextType = typeof(SRMContext))]
    public partial class LogSapShedule
    {
        public int LogId { get; set; }
        public string Type { get; set; }
        public string SrmId { get; set; }
        public string Message { get; set; }
        public DateTime? Datetime { get; set; }
        public string Werks { get; set; }
    }
}
