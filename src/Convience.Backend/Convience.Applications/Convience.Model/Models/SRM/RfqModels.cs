using Convience.Entity.Entity.SRM;
using Convience.Model.Models.SystemManage;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    public class FileUploadViewModel_RFQ:SrmRfqH
    {
        public string CurrentDirectory { get; set; }
        // 上传文件
        public IEnumerable<IFormFile> Files { get; set; }
    }
    public record QueryMatnrModel : PageQueryModel
    {
        public string Matnr { get; set; }
        public int[] Werks { get; set; }
    }
    public record QueryVendorModel : PageQueryModel
    {
        public string Vendor { get; set; }
        public string Code { get; set; }
        public int? Org { get; set; }
        public int? Ekorg { get; set; }
        public int[] Werks { get; set; }
    }
    public class ViewSrmRfqM : SrmRfqM
    {
        public string matnr { get; set; }
        public string volume { get; set; }
        public int status { get; set; }
        public string srmMatnr { get; set; }
        public string description { get; set; }
    }

    public class ViewSrmMatnr : SrmMatnr
    {
        public ViewSrmMatnr() { }
        public ViewSrmMatnr(SrmMatnr parent)
        {
            foreach (PropertyInfo prop in parent.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
        }
        public string viewstatus {
            get { return ((Status)Status.Value).ToString(); }
        }
    }

    public class ViewSrmVendor : SrmVendor{
        public ViewSrmVendor() { }
        public ViewSrmVendor(SrmVendor parent)
        {
            foreach (PropertyInfo prop in parent.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
        }
        public string viewstatus {
            get { return ((Status)Status.Value).ToString(); }
        }
    }

    public class ViewSrmRfqV : SrmVendor
    {
        public int rfqVId { get; set; }
        public int rfqId { get; set; }
        public string viewstatus {
            get { return ((Status)Status.Value).ToString(); }
        }
    }
    public class ViewSrmRfqH : SrmRfqH { 
    
        public string sourcerName { get; set; }
        public string viewstatus {
            get {return Status.HasValue?((Status)Status).ToString():""; }
        }
        public string C_date {
            get { return CreateDate.HasValue? CreateDate.Value.ToString("yyyy/MM/dd"):""; }
        }
        public string deadline_str {
            get { return Deadline.HasValue ? Deadline.Value.ToString("yyyy/MM/dd") : ""; }
        }
        public string C_by {
            get; set;
        }
        public string ekgry {
            get;set;
        }
    }
    public record ResultRfqModel {
        public ViewSrmRfqH h { get; set; }
        //public ViewSrmRfqM[] m { get; set; }
        //public ViewSrmRfqV[] v { get; set; }
        public System.Linq.IQueryable m { get; set; }
        public ViewSrmRfqV[] v { get; set; }
    }
    public class QueryRfqList {
        public string rfqNum { get; set; }
        public int status { get; set; }
        public string name { get; set; }
        public int[] statuses { get; set; }
        public int[] werks { get; set; }
        public bool end { get; set; }
    }
    public class QueryRfq
    {
        public string rfqNum { get; set; }
        public int? status { get; set; }
        public string name { get; set; }
        public string costNo { get; set; }
        public int[] statuses { get; set; }
        public int[] werks { get; set; }
    }
}
