using Convience.Entity.Entity.SRM;
using Convience.JwtAuthentication;
using Convience.Model.Models.SystemManage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    //public class QueryFile
    //{
    //    public int? id { get; set; }
    //    public int? werk { get; set; }
    //    public string? number { get; set; }
    //    public int? functionId { get; set; }
    //    public int? type { get; set; }
    //    public System.Security.Claims.ClaimsPrincipal user { get; set; }
    //}
    public class ViewSelects
    {
        public string id { get; set; }
        public string name { get; set; }
    }
    public class ViewSrmDisscussionH: SrmDisscussionH
    {
        public string content { get; set; }
        public string FunctionName { get; set; }
        public string UserName { get; set; }
        public int? Werks { get; set; }
        public virtual ICollection<ViewSrmDisscussionC> ViewSrmDisscussionCs { get; set; }
    }
    public class ViewSrmDisscussionC : SrmDisscussionC
    {
        public string UserName { get; set; }
        public bool isEdit { get; set; }
    }
}