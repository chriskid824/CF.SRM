using Convience.Entity.Entity.SRM;
using Convience.Model.Models.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    public class QueryPoList
    {
        public string buyer { get; set; }
        public int deliveryLId { get; set; }
        public string deliveryNum { get; set; }
        public string poNum { get; set; }
        public DateTime? replyDeliveryDate_e { get; set; }
        public DateTime? replyDeliveryDate_s { get; set; }
        public int status { get; set; }
        public string host { get; set; }
    }

    public class ViewSrmDeliveryH : SrmDeliveryH
    {
        public string Address { get; set; }
        public string DeliveryPlace { get; set; }
        public string PoNum { get; set; }
        new public virtual ICollection<ViewSrmDeliveryL> SrmDeliveryLs { get; set; }
        public string TelPhone { get; set; }
        public string VendorName { get; set; }
    }

    public class ViewSrmDeliveryL : SrmDeliveryL
    {
        public string Description { get; set; }
        public string Matnr { get; set; }
        public string PoNum { get; set; }
        public float? Qty { get; set; }
        public string WoItem { get; set; }
        public string WoNum { get; set; }
        public string Url { get; set; }
    }

    public class ViewSrmPoL : SrmPoL
    {
        private float? _deliveryQty;
        public string Buyer { get; set; }
        public float? DeliveryQty { get { return _deliveryQty.HasValue ? _deliveryQty.Value : RemainQty; } set { _deliveryQty = value; } }
        public string Matnr { get; set; }
        public string PoNum { get; set; }
        public float? RemainQty { get; set; }
        public int? Status { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? VendorId { get; set; }
        public string VendorName { get; set; }
    }
}