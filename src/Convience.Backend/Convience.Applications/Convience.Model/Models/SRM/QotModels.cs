using Convience.Entity.Entity.SRM;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    public class FileUploadViewModel_QOT :SrmQotH
    {
        public string CurrentDirectory { get; set; }
        // 上传文件
        public IEnumerable<IFormFile> Files { get; set; }
    }
    public class QueryQot
    {
        public int? rfqId { get; set; }
        public int? matnrId { get; set; }
        public Status? status { get; set; }
        public int? vendorId { get; set; }
        public int? qotId { get; set; }
        public int? caseId { get; set; }
        public string  matnr { get; set; }
    }
    public class ViewSrmPriceDetail {
        public ViewSrmRfqM matnr { get; set; }
        public SrmQotH[] qot { get; set; }
        public viewSrmQotMaterial[] material { get; set; }
        public viewSrmQotProcess[] process { get; set; }
        public viewSrmQotSurface[] surface { get; set; }
        public viewSrmQotOther[] other { get; set; }
        public ViewSrmInfoRecord[] infoRecord { get; set; }
    }
    public class viewSrmQotMaterial : SrmQotMaterial
    {
        public viewSrmQotMaterial() { }
        public viewSrmQotMaterial(SrmQotMaterial parent)
        {
            foreach (PropertyInfo prop in parent.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
        }
        
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public int RfqId { get; set; }
    }

    public class viewSrmQotMaterialS : SrmQotMaterial
    {
        public viewSrmQotMaterialS() { }

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
        public string ProcessName { get; set; }
        public int RfqId { get; set; }
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
        public string ProcessName { get; set; }
        public int RfqId { get; set; }
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
        public int RfqId { get; set; }
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
        public int? rfqId { get; set; }
        public string rfqno { get; set; }
        public int status { get; set; }
        public string matnr { get; set; }
        public int vendorid { get; set; }
        public string vendor { get; set; }
    }
    public class ViewQot : SrmQotH
    {
        public string Matnr { get; set; }
        public string Size { get; set; }
        public string RfqNum { get; set; }
        #region SrmRfqM
        public int RfqMId { get; set; }
        public int? RfqId { get; set; }
        public int? MatnrId { get; set; }
        public string Version { get; set; }
        public string Material { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
        public string Density { get; set; }
        public double? Weight { get; set; }
        public string MachineName { get; set; }
        public double? Qty { get; set; }
        public string Note { get; set; }
        #endregion
    }
    public class ViewQotResult {
        public ViewSrmRfqM matnr { get; set; }
        public SrmQotH qot { get; set; }
        public viewSrmQotMaterial[] material { get; set; }
        public viewSrmQotProcess[] process { get; set; }
        public viewSrmQotSurface[] surface { get; set; }
        public viewSrmQotOther[] other { get; set; }  
        
    }

    public class ViewQotListH : SrmRfqH
    {
        public int VRfqId { get; set; }
        public string VRfqNum { get; set; }
        public string VStatusDesc { get; set; }
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
        public string VVendor { get; set; }
        public int VVendorId { get; set; }
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
        public string QStatusDesc { get; set; }
        public decimal? QTotalAmount { get; set; }
        public int? QVendorId { get; set; }
        public string QVendor { get; set; }
        public string QMatnr { get; set; }
        public string User { get; set; }
    }

    public record ResultQotModel
    {
        //public ViewSrmRfqH h { get; set; }
        //public ViewSrmRfqM[] m { get; set; }
        //public ViewSrmRfqV[] v { get; set; }
        public System.Linq.IQueryable q { get; set; }
        public System.Linq.IQueryable m { get; set; }
        //public ViewSrmRfqV[] v { get; set; }
    }
    public class SrmQotUpdateMaterial:SrmQotH
    {
        public string reason { get; set; }
    }
}
