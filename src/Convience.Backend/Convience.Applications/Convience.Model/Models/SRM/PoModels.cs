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
        public string deliveryNum { get; set; }
        public int status { get; set; }
        public string buyer { get; set; }
        public DateTime? replyDeliveryDate_s { get; set; }
        public DateTime? replyDeliveryDate_e { get; set; }
    }
    public class ViewSrmPoL : SrmPoL
    {

        public string PoNum { get; set; }
        public int? Status { get; set; }
        public int? VendorId { get; set; }
        public string VendorName { get; set; }
        public decimal? TotalAmount { get; set; }
        public string Buyer { get; set; }
        public string Matnr { get; set; }
        public float? RemainQty { get; set; }
        private float? _deliveryQty;
        public float? DeliveryQty { get { return _deliveryQty.HasValue ? _deliveryQty.Value : RemainQty; } set { _deliveryQty = value; } }
    }
}
