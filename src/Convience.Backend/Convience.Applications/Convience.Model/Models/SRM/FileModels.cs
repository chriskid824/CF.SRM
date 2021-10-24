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
    public class ViewSrmFileUploadTemplate : SrmFileUploadTemplate
    {
        public string Address { get; set; }
        public string DeliveryPlace { get; set; }
        public string PoNum { get; set; }
        new public virtual ICollection<ViewSrmDeliveryL> SrmDeliveryLs { get; set; }
        public string TelPhone { get; set; }
        public string VendorName { get; set; }
        public string StatusDesc { get; set; }
    }
}