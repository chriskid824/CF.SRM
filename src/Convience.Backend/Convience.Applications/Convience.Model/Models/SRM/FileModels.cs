using Convience.Entity.Entity.SRM;
using Convience.JwtAuthentication;
using Convience.Model.Models.SystemManage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public string number { get; set; }
        // 上传文件
        public IEnumerable<IFormFile> Files { get; set; }
    }
    public class NzFileViewModel
    {
        public int Uid { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }

    public class AnnouncementType
    {
        public string Stylecolor { get; set; }
        public string TxtTypeName { get; set; }
        public string Icon { get; set; }
        public string Router { get; set; }
        public List<AnnouncementDetail> NumberList = new List<AnnouncementDetail>();
    }
    public class AnnouncementDetail
    {
        public string id { get; set; }
        public string number { get; set; }
        public int status { get; set; }
        public string viewstatus
        {
            get { return ((Status)status).ToString(); }
        }
        public string color { get { 
            switch(this.status)
                {
                    case 6:
                        return "magenta";
                    case 17:
                        return "magenta";
                    case 19:
                        return "magenta";
                    case 20:
                        return "magenta";
                    case 18:
                        return "green";
                    default:
                        return "blue";
                }
           } 
        }
    }
    public class BaseFileData
    {
        public bool active { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
    //public class FileUploadViewModel: ViewSrmFileRecord
    //{
    //    public IEnumerable<IFormFile> Files { get; set; }
    //}
}