using Convience.Entity.Entity.SRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    public class  QueryInfoRecordModels: SrmInforecord
    {
        public int[] werks { get; set; }
        public int[] qotIds { get; set; }
        public int page { get; set; }=1;
        public int size { get; set; }=10;
    }

    public class ViewSrmInfoRecord : SrmInforecord
    {
        public ViewSrmInfoRecord() { }
        public ViewSrmInfoRecord(SrmInforecord parent)
        {
            if (parent != null)
            {
                foreach (PropertyInfo prop in parent.GetType().GetProperties())
                    GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
            }
        }
        //public new int? InfoId { get; set; }
        //    get { return InfoId == 0 ? null : InfoId; }
        //}
        public string currencyName { get; set; }
        public string taxcodeName { get; set; }
        public SrmMatnr matnrObject { get; set; }
        public string srmMatnr1 { get; set; }
        public SrmVendor vendorObject { get; set; }
        public string srmVendor1 { get; set; }
        public string VendorName { get; set; }
        public string MatnrName { get; set; }
        public decimal Atotal { get; set; }
        public decimal Btotal { get; set; }
        public decimal Ctotal { get; set; }
        public decimal Dtotal { get; set; }
        public decimal total {
            get {
                return Atotal + Btotal + Ctotal + Dtotal;
            } } 
        public string rfqId { get; set; }
        public string rfqNum { get; set; }
        public string qotNum { get; set; }
        public string viewQotStatus { get; set; }
        public string viewstatus { get; set; }
        public string EffectiveDate_str { get { return EffectiveDate?.ToString("yyyy/MM/dd"); } }
        public string ExpirationDate_str { get { return ExpirationDate?.ToString("yyyy/MM/dd"); } }
        public string infoKindName {
            get {
                return InfoKind.HasValue ? ((INFO_KIND)InfoKind.Value).ToString() : "";
            }
        }
        public string typeName {
            get {
                return string.IsNullOrWhiteSpace(Type) ? "" : ((TYPE)Convert.ToChar(Type)).ToString();
            }
        }
        public string Description { get; set; }
    }


    public class ViewSummary : ViewSrmRfqH
    {
        public int InfoId { get; set; }
        public int? caseId { get; set; }
        public bool canEdit { get; set; }
        public bool isStarted { get; set; }
        public Status qotStatus { get; set; }
        public string viewQotStatus {
            get { return ((qotStatus == 0)? "" : qotStatus.ToString()); }
        }
        public string vendor { get; set; }
        public int vendorId { get; set; }
        public string vendorName { get; set; }
        public SrmMatnr matnrObject { get; set; }
        public SrmVendor vendorObject { get; set; }
        public string matnr { get; set; }
        public int matnrId { get; set; }
        public string material { get; set; }
        public string volume { get; set; }
        public string weight { get; set; }
        public string gewei { get; set; }
        public string machineName { get; set; }
        public string qotId { get; set; }
        public string qotNum { get; set; }
        public string mMaterial { get; set; }
        public string mPrice { get; set; }
        public string mLength { get; set; }
        public string mWidth { get; set; }
        public string mHeight { get; set; }
        public string mDensity { get; set; }
        public string mWeight { get; set; }
        public string mCostPrice { get; set; }
        public string mNote { get; set; }
        public string pMachine { get; set; }
        public string pProcessNum { get; set; }
        public string pHours { get; set; }
        public string pPrice { get; set; }
        public string pSubTotal { get; set; }
        public string pNote { get; set; }
        public string sProcess { get; set; }
        public string sTimes { get; set; }
        public string sPrice { get; set; }
        public string sSubTotal { get; set; }
        public string sMethod { get; set; }
        public string sNote { get; set; }
        public string oItem { get; set; }
        public string oDescription { get; set; }
        public string oPrice { get; set; }
        public string oNote { get; set; }
        public string aTotal { get; set; }
        public string bTotal { get; set; }
        public string cTotal { get; set; }
        public string dTotal { get; set; }
        public string total { get; set; }
        public string price { get; set; }
        public string unit { get; set; }
        public string currency { get; set; }
        public string currencyName { get; set; }
        public string leadTime { get; set; }
        public string standQty { get; set; }
        public string minQty { get; set; }
        public string taxcode { get; set; }
        public string taxcodeName { get; set; }
        public string effectiveDate { get; set; }
        public string expirationDate { get; set; }
        public string note { get; set; }
        public int? org { get; set; }
        public string infoKind { get; set; }
        public string infoKindName { get; set; }
        public string type { get; set; }
        public string typeName { get; set; }
        public string sortl { get; set; }
        public string description { get; set; }
        public string minor_diameter { get; set; }
        public string major_diameter { get; set; }
        public string bn_num { get; set; }
    }

    public enum INFO_KIND {
        標準 = 0,
        分包 = 3
    }
    public enum TYPE {
        物料 = 'M',
        工單件 = 'W'
    }
}
