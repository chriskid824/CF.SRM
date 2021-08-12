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
}
