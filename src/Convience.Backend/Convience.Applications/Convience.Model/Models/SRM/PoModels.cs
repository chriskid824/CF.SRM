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
    public class QueryPoList
    {
        public string buyer { get; set; }
        public int deliveryLId { get; set; }
        public string deliveryNum { get; set; }
        public string poNum { get; set; }
        public int poLId { get; set; }
        public DateTime? replyDeliveryDate_e { get; set; }
        public DateTime? replyDeliveryDate_s { get; set; }
        public int status { get; set; }
        public string host { get; set; }
        public int dataStatus { get; set; }
        public System.Security.Claims.ClaimsPrincipal user { get; set; }
    }

    public class ViewSrmDeliveryH : SrmDeliveryH
    {
        public string Address { get; set; }
        public string DeliveryPlace { get; set; }
        public string PoNum { get; set; }
        new public virtual ICollection<ViewSrmDeliveryL> SrmDeliveryLs { get; set; }
        public string TelPhone { get; set; }
        public string VendorName { get; set; }
        public string StatusDesc { get; set; }
    }

    public class ViewSrmDeliveryL : SrmDeliveryL
    {
        public string DeliveryNum { get; set; }
        public new int? DeliveryLId { get; set; }
        public string Description { get; set; }
        public string Matnr { get; set; }
        public string PoNum { get; set; }
        public float? Qty { get; set; }
        public string WoItem { get; set; }
        public string WoNum { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string SapVendor { get; set; }
        public string Url { get { return this.DeliveryId + "/" + this.DeliveryLId; } }
        public int? Org { get; set; }
        public string DeliveryPlace { get; set; }
        public string LastUpdateBy { get; set; }
    }

    public class ViewSrmPoL : SrmPoL
    {
        private float? _deliveryQty;
        public string Buyer { get; set; }
        public float? DeliveryQty { get { return _deliveryQty.HasValue ? _deliveryQty.Value : RemainQty; } set { _deliveryQty = value; } }
        public string Matnr { get; set; }
        public string PoNum { get; set; }
        public float? RemainQty { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? VendorId { get; set; }
        public string VendorName { get; set; }
        public string StatusDesc { get; set; }
        public string SapVendor { get; set; }
        public int? Org { get; set; }
        public string Number { get { return this.PoNum + "-" + this.PoLId.ToString(); } }
    }
    public class ViewSrmPoH : SrmPoH
    {
        public string StatusDesc { get; set; }
        public string VendorName { get; set; }
        public string OrgName { get; set; }
        public string SapVendor { get; set; }
        new public virtual ICollection<ViewSrmPoL> SrmPoLs { get; set; }
    }

    public class ViewSrmPoPoL : ViewSrmPoL
    {
        public string OrgName { get; set; }
        public DateTime? DocDate { get; set; }
        public DateTime? ReplyDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }


    public class SapPoData
    {
        //[JsonProperty("T_EKKO")]
        public List<T_EKKO> T_EKKO = new List<T_EKKO>();
        //[JsonProperty("T_EKKO")]
        public List<T_EKPO> T_EKPO = new List<T_EKPO>();
    }
    public class SapResultList
    {
        public List<SapResultData> List = new List<SapResultData>();
        public string err { get; set; }
    }
    public class SapResultData
    {
        public string Id { get; set; }
        public int LId { get; set; }
        public string Type { get; set; }
        public string OutCome { get; set; }
        public string Reason { get; set; }
    }

    public class T_EKKO
    {
        public string EBELN { get; set; }
        public string LIFNR { get; set; }
        public string RLWRT { get; set; }
        public string WAERS { get; set; }
        public string EKGRP { get; set; }
        public int? EKORG { get; set; }
        public DateTime BEDAT { get; set; }
    }
    public class T_EKPO
    {
        public int EBELP { get; set; }
        public string EBELN { get; set; }
        public string MATNR { get; set; }
        public string MAKTX { get; set; }
        public string MENGE { get; set; }
        public decimal? NETPR { get; set; }
        public DateTime? EINDT { get; set; }
        public string LGOBE { get; set; }
        public string KZKRI { get; set; }
        public string AUFNR { get; set; }

    }
}