using Convience.Entity.Entity.SRM;
using Convience.JwtAuthentication;
using Convience.Model.Models.SystemManage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    public class QueryFile
    {
        public int? id { get; set; }
        public int? werk { get; set; }
        public string? number { get; set; }
        public int? functionId { get; set; }
        public int? type { get; set; }
        public System.Security.Claims.ClaimsPrincipal user { get; set; }
    }
    public class ViewSrmFileUploadTemplate : SrmFileUploadTemplate
    {
        public string FileTypesDesc { get; set; }
        public string FunctionName { get; set; }
    }
    public class ViewSrmFileRecordResult : ViewSrmFileRecord
    {
        public List<NzFileViewModel> fileList { get; set; }
    }
    public class FileUploadViewModel
    {
        public UserClaims user { get; set; }
        public ViewSrmFileRecordResult file { get; set; }
        public string json { get; set; }
        public List<int> fileTypeList  { get; set; }

        // 上传文件
        public IEnumerable<IFormFile> Files { get; set; }
    }
    public class NzFileViewModel
    {
        public int Uid { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
    //public class FileUploadViewModel: ViewSrmFileRecord
    //{
    //    public IEnumerable<IFormFile> Files { get; set; }
    //}
}