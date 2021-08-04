using Convience.Entity.Entity.SRM;
using Convience.Model.Models.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    public record QueryMatnrModel:PageQueryModel
    {
        public string Matnr { get; set; }
    }
    public record QueryVendorModel : PageQueryModel
    {
        public string Vendor { get; set; }
    }
    public class ViewSrmRfqM : SrmRfqM
    {
        public string matnr { get; set; }
        public string volume { get; set; }
        public int status { get; set; }
        public string srmMatnr { get; set; }
    }

    public class ViewSrmRfqV : SrmVendor
    {
        public int rfqVId { get; set; }
        public int rfqId { get; set; }
    }
    public class ViewSrmRfqH : SrmRfqH { 
    
        public string sourcerName { get; set; }
        public string viewstatus {
            get {return ((Status)Status).ToString(); }
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
        public string costNo { get; set; }
    }
}
