using Convience.Entity.Entity.SRM;
using Convience.Model.Models.SystemManage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    public class ViewSrmMaterial : SrmMatnr
    {
        public string StatusDesc { get; set; }
        public string User { get; set; }
    }
    public record QueryMaterial : PageQueryModel
    {
        public string material { get; set; }
        public string name { get; set; }
        public int[] Werks { get; set; }
    }

    public class FileUploadViewModel : SrmMaterialTrend
    {
        public string CurrentDirectory { get; set; }
        // 上传文件
        public IEnumerable<IFormFile> Files { get; set; }
    }

    public record QuerySrmMaterialTrend : PageQueryModel {
        public SrmMaterialTrend materialTrend { get; set; }
        public DateTime? searchDate { get; set; }
    }

    public class ViewSrmMaterialTrend:SrmMaterialTrend {
        public ViewSrmMaterialTrend() { }
        public ViewSrmMaterialTrend(SrmMaterialTrend parent)
        {
            if (parent != null)
            {
                foreach (PropertyInfo prop in parent.GetType().GetProperties())
                    GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
            }
        }
        public string EffectiveDate_str { get { return EffectiveDate?.ToString("yyyy/MM/dd"); } }
        public string Deadline_str { get { return Deadline?.ToString("yyyy/MM/dd"); } }
        public string CreateDate_str { get { return CreateDate?.ToString("yyyy/MM/dd HH:mm:ss"); } }
        public string CreateName { get; set; }
    }

}
