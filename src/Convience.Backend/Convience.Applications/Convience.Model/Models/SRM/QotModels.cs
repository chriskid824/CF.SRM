using Convience.Entity.Entity.SRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    public class QueryQot
    {
        public int? rfqId { get; set; }
        public int? matnrId { get; set; }
        public Status? status { get; set; }
    }
    public class ViewSrmPriceDetail {
        public ViewSrmRfqM matnr { get; set; }
        public SrmQotH[] qot { get; set; }
        public viewSrmQotMaterial[] material { get; set; }
        public viewSrmQotProcess[] process { get; set; }
        public viewSrmQotSurface[] surface { get; set; }
        public viewSrmQotOther[] other { get; set; }
        public viewSrmInfoRecord[] infoRecord { get; set; }
    }
    public class viewSrmQotMaterial: SrmQotMaterial
    {
        public viewSrmQotMaterial() { }
        public viewSrmQotMaterial(SrmQotMaterial parent)
        {
            foreach (PropertyInfo prop in parent.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
        }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
    }
    public class viewSrmQotProcess : SrmQotProcess {
        public viewSrmQotProcess() { }
        public viewSrmQotProcess(SrmQotProcess parent) {
            foreach (PropertyInfo prop in parent.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
        }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public decimal SubTotal { get; set; }
    }
    public class viewSrmQotSurface : SrmQotSurface
    {
        public viewSrmQotSurface() { }
        public viewSrmQotSurface(SrmQotSurface parent)
        {
            foreach (PropertyInfo prop in parent.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
        }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public decimal SubTotal { get; set; }
    }
    public class viewSrmQotOther : SrmQotOther
    {
        public viewSrmQotOther() { }
        public viewSrmQotOther(SrmQotOther parent)
        {
            foreach (PropertyInfo prop in parent.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
        }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
    }
    public class viewSrmInfoRecord : SrmInforecord {
        public viewSrmInfoRecord() { }
        public viewSrmInfoRecord(SrmInforecord parent)
        {
            foreach (PropertyInfo prop in parent.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
        }
        public string VendorName { get; set; }
        public decimal Atotal { get; set; }
        public decimal Btotal { get; set; }
        public decimal Ctotal { get; set; }
        public decimal Dtotal { get; set; }
    }
    public class ViewSrmQotList : SrmQotH
    {
        public ViewSrmQotList()
        {
            SrmQotHs = new HashSet<SrmQotH>();
        }
        public virtual ICollection<SrmQotH> SrmQotHs { get; set; }

        public string RFQ_NUM { get; set; }
        public int? RFQ_STATUS { get; set; }
        public int QOT_ID { get; set; }
        public string QOT_NUM { get; set; }
        public int? VENDOR_ID { get; set; }
        public int? MATNR_ID { get; set; }
        public int? RFQ_ID { get; set; }
        public string CURRENCY { get; set; }
        public double? LEAD_TIME { get; set; }
        public float? MIN_QTY { get; set; }
        public decimal? TOTAL_AMOUNT { get; set; }
        public int? QSTATUS { get; set; }
        public DateTime? QCREATE_DATE { get; set; }
        public string QCREATE_BY { get; set; }
        public DateTime? QLAST_UPDATE_DATE { get; set; }
        public string QLAST_UPDATE_BY { get; set; }
        public int? RSTATUS { get; set; }
        public DateTime? RCREATE_DATE { get; set; }
        public string RCREATE_BY { get; set; }
        public DateTime? RLAST_UPDATE_DATE { get; set; }
        public string RLAST_UPDATE_BY { get; set; }
        public string MATNR { get; set; }
        public string VENDOR { get; set; }
    }
    public class QueryQotList
    {
        public string rfqno { get; set; }
        public int status { get; set; }
        public string matnr { get; set; }
        public int vendor { get; set; }
    }

    public class ViewQotListH : SrmRfqH
    {
        public int VRfqId { get; set; }
        public string VRfqNum { get; set; }
        public int? VStatus { get; set; }
        public string VSourcer { get; set; }
        public DateTime? VDeadline { get; set; }
        public DateTime? VCreateDate { get; set; }
        public string VCreateBy { get; set; }
        public DateTime? VLastUpdateDate { get; set; }
        public string VLastUpdateBy { get; set; }
        public DateTime? VEndDate { get; set; }
        public string VEndBy { get; set; }
        public virtual ICollection<ViewQotListL> SrmQotHs { get; set; }
    }
    public class ViewQotListL : SrmQotH
    {
        public string QCreateBy { get; set; }

        public DateTime? QCreateDate { get; set; }

        public string QCurrency { get; set; }

        public string QLastUpdateBy { get; set; }

        public DateTime? QLastUpdateDate { get; set; }

        public double? QLeadTime { get; set; }

        public int? QMatnrId { get; set; }

        public float? QMinQty { get; set; }

        public int QQotId { get; set; }
        public string QQotNum { get; set; }
        public int? QRfqId { get; set; }
        public int? QStatus { get; set; }
        public decimal? QTotalAmount { get; set; }
        public int? QVendorId { get; set; }
        public string QVendor { get; set; }
        public string QMatnr { get; set; }
    }
	public class ViewSummary :ViewSrmRfqH{
        public string vendor { get; set; }
        public string vendorName { get; set; }
        public string matnr { get; set; }
        public string material { get; set; }
        public string volume { get; set; }
        public string weight { get; set; }
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
        public string price { get; set; }
        public string unit { get; set; }
        public string ekgry { get; set; }
        public string leadTime { get; set; }
        public string standQty { get; set; }
        public string minQty { get; set; }
        public string taxcode { get; set; }
        public string effectiveDate { get; set; }
        public string expirationDate { get; set; }
    }
}
