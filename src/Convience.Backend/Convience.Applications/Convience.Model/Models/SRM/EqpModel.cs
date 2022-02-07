using Convience.Entity.Entity.SRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    public class QueryEqp 
    {
        public string woNum { get; set; }
        public string matnr { get; set; }
        public string no { get; set; }
        public int status { get; set; }
        public int vendorid { get; set; }
        public string txtSN { get; set; }
        public string vendor { get; set; }
        public int eqpid { get; set; }
        public int matnrid { get; set; }
    }
    public class ViewEqpList : SrmEqpH
    {
        public string buyer { get; set; }
        public int po_id { get; set; }
        public string po_num { get; set; }
        public int polid { get; set; }
        public string description { get; set; }
        public float? qty { get; set; }
        public DateTime? deliverydate { get; set; }
        public string srm_matnr { get; set; }
        public int matnr_id { get; set; }
        public string ekgry { get; set; }
        public string ekgry_id { get; set; }
        public string ekgry_desc { get; set; }
        public int vendorid { get; set; }
        public string txtSN { get; set; }
        public string disposition { get; set; } //工程問題
        public string qdisposition { get; set; }//品質研判措施
        public string ddlresdept { get; set; }
        public string qdrno { get; set; }//QDR No
        public string note { get; set; }//補充說明
        public string reworkcosts { get; set; } //預估整修成本
        public string solve { get; set; } //工程處置措施
        public string no { get; set; } //序號
        public string woNum { get; set; }
        public string matnr { get; set; }
        public string createdBy { get; set; }
        public string vendor { get; set; }

    }
}
