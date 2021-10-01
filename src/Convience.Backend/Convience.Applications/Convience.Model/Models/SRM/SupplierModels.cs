using Convience.Entity.Entity.SRM;
using Convience.Model.Models.SystemManage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    public class ViewSrmSupplier : SrmSupplier
    {
        public string StatusDesc { get; set; }
        public string User { get; set; }
    }
}
