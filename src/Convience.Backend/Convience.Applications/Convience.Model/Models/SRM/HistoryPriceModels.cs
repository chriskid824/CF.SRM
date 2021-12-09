using Convience.Entity.Entity.SRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    public class QuerySrmHistoryPrice : SrmHistoryPrice
    {
        public bool orderASC { get; set; }
        public int? year { get; set; }
    }
}
