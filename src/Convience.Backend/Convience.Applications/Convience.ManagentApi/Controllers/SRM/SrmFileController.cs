using Convience.Entity.Entity.SRM;
using Convience.JwtAuthentication;
using Convience.ManagentApi.Infrastructure.Authorization;
using Convience.ManagentApi.Infrastructure.Logs;
using Convience.Model.Models.ContentManage;
using Convience.Model.Models.SRM;
using Convience.Service.ContentManage;
using Convience.Service.SRM;
using Convience.Service.SystemManage;
using Convience.Util.Extension;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Convience.ManagentApi.Controllers.SRM
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SrmFileController : ControllerBase
    {
        private readonly ISrmFileService _srmFileService;
        private readonly IUserService _userService;
        private readonly IFileManageService _fileManageService;

        public SrmFileController(ISrmFileService srmFileService, IUserService userService, IFileManageService fileManageService)
        {
            _srmFileService = srmFileService;
            _userService = userService;
            _fileManageService = fileManageService;
        }

        [HttpPost("AddTemplate")]
        public IActionResult AddTemplate(SrmFileUploadTemplate template)
        {
            if (_srmFileService.AddTemplate(template)) return Ok();
            return BadRequest("新增樣板失敗");
        }
        [HttpPost("UpdateTemplate")]
        public IActionResult UpdateTemplate(SrmFileUploadTemplate template)
        {
            if (_srmFileService.UpdateTemplate(template)) return Ok();
            return BadRequest("樣板修改失敗");
        }

        [HttpPost("GetTemplateList")]
        public IActionResult GetTemplateList(JObject query)
        {
            //SapDeliveryService sc = new SapDeliveryService();
            //sc.test();
            if (query == null)
            {
                return null;
            }
            QueryFile q = new QueryFile();
            //var aaa = query.Property("poNum");
            q.id = query["templateId"].ToString()==""?0: (int)query["templateId"];
            q.werk = (int)query["werks"];
            q.user = User;
            var aaa = _srmFileService.GetTemplateList(q);

            return Ok(aaa);

            //return JsonConvert.SerializeObject(aaa, Formatting.None,
            //            new JsonSerializerSettings()
            //            {
            //                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //            });
        }
        [HttpPost("GetFileList")]
        public IActionResult GetFileList(JObject query)
        {
            if (query == null)
            {
                return null;
            }
            QueryFile q = new QueryFile();
            //var aaa = query.Property("poNum");
            q.number = query["number"].ToString();
            q.functionId = (int)query["functionId"];
            q.werk=(int)query["werks"];
            q.type= (int)query["type"];
            q.user = User;
            var aaa = _srmFileService.GetFileListByNumer(q);

            return Ok(aaa);
        }

        //[Permission("fileAdd")]
        //public async Task<IActionResult> UploadFile(List<ViewSrmFileRecordResult> files)
        //{
        //var result = await _fileManageService.UploadAsync(fileUploadModel);
        //if (!string.IsNullOrEmpty(result))
        //{
        //    return this.BadRequestResult(result);
        //}
        //return Ok();
        //}
        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile([FromForm] Model.Models.SRM.FileUploadViewModel fileUploadModel)
        {
            fileUploadModel.file = JsonConvert.DeserializeObject<ViewSrmFileRecordResult>(fileUploadModel.json);
            fileUploadModel.user = User.GetUserClaims();
            try
            {
                var result = await _srmFileService.UploadAsync(fileUploadModel);
                if (!string.IsNullOrEmpty(result))
                {
                    return this.BadRequestResult(result);
                }

            }
            catch (Exception e)
            { return this.BadRequestResult("上傳失敗"); }

            return Ok();
        }

        [HttpGet]
        [Permission("fileGet")]
        [LogFilter("内容管理", "文件管理", "下载文件")]
        public async Task<IActionResult> DownloadFile([FromQuery] Model.Models.SRM.NzFileViewModel viewModel)
        {
            var stream = await _srmFileService.DownloadAsync(viewModel);
            return File(stream, "application/octet-stream", viewModel.Name);
        }

        [HttpDelete]
        [Permission("fileDelete")]
        [LogFilter("内容管理", "文件管理", "删除文件")]
        public async Task<IActionResult> DeleteFile([FromQuery] NzFileViewModel viewModel)
        {
            var isSuccess = await _srmFileService.DeleteFileAsync(viewModel);
            if (!isSuccess)
            {
                return this.BadRequestResult("删除失败！");
            }
            return Ok();
        }

        [HttpGet("GetAnnList")]
        public IActionResult GetAnnList()
        {
            QueryFile q = new QueryFile();
            q.user = User;
            var aaa = _srmFileService.GetAnnList(q);

            return Ok(aaa);
        }
    }
}