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
    }
    public class ViewSrmPriceDetail {
        public ViewSrmRfqM matnr { get; set; }
        public SrmQotH[] qot { get; set; }
        public viewSrmQotMaterial[] material { get; set; }
        public viewSrmQotProcess[] process { get; set; }
        public viewSrmQotSurface[] surface { get; set; }
        public viewSrmQotOther[] other { get; set; }
    }
    public class viewSrmQotMaterial: SrmQotMaterial
    {
        public viewSrmQotMaterial() { }
        public viewSrmQotMaterial(SrmQotMaterial parent)
        {
            foreach (PropertyInfo prop in parent.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
        }
        public string VendorName { get; set; }
    }
    public class viewSrmQotProcess : SrmQotProcess {
        public viewSrmQotProcess() { }
        public viewSrmQotProcess(SrmQotProcess parent) {
            foreach (PropertyInfo prop in parent.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
        }
        public string VendorName { get; set; }
    }
    public class viewSrmQotSurface : SrmQotSurface
    {
        public viewSrmQotSurface() { }
        public viewSrmQotSurface(SrmQotSurface parent)
        {
            foreach (PropertyInfo prop in parent.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
        }
        public string VendorName { get; set; }
    }
    public class viewSrmQotOther : SrmQotOther
    {
        public viewSrmQotOther() { }
        public viewSrmQotOther(SrmQotOther parent)
        {
            foreach (PropertyInfo prop in parent.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
        }
        public string VendorName { get; set; }
    }
    public class viewSrmQotInfoRecord : SrmInforecord {
        public viewSrmQotInfoRecord() { }
    }
}
