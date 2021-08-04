using Convience.Entity.Entity.SRM;
using Convience.Model.Models.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    public class QueryPoList {
        public string poNum { get; set; }
        public int status { get; set; }
        public string buyer { get; set; }
    }
    public class ViewSrmPoL : SrmPoL
    {

        public string PoNum { get; set; }
    }
}
