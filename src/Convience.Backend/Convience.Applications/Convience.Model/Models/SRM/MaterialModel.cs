using Convience.Entity.Entity.SRM;
using Convience.Model.Models.SystemManage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    public class ViewSrmMatnr1 : SrmMatnr
    {
        public string StatusDesc { get; set; }
        public string User { get; set; }
    }
    public record QueryMaterial : PageQueryModel
    {
        public string material { get; set; }
        public string name { get; set; }
        public int[] Werks { get; set; }
    }
}
