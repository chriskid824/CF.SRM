using Convience.Entity.Entity.SRM;
using Convience.JwtAuthentication;
using Convience.Mail;
using Convience.ManagentApi.Infrastructure.Authorization;
using Convience.ManagentApi.Infrastructure.Logs;
using Convience.Model.Models.ContentManage;
using Convience.Model.Models.SRM;
using Convience.Model.Models.SystemManage;
using Convience.Service.SRM;
using Convience.Service.SystemManage;
using Convience.Util.Extension;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Convience.ManagentApi.Controllers.SRM
{
    [Route("api/[controller]")]
    [ApiController]
    public class SrmMaterialController : ControllerBase
    {
        private readonly ISrmMaterialService _srmMaterialService;

        public SrmMaterialController(ISrmMaterialService srmMaterialService)
        {
            _srmMaterialService = srmMaterialService;
        }

        [HttpPost("GetMaterialList")]
        [Permission("supplier")]
        public IActionResult GetMaterialList(QueryMaterial query)
        {
            return Ok(_srmMaterialService.GetMaterialList(query));
        }
        [HttpPost("GetMaterialDetail")]
        [Permission("detail")]
        public IActionResult GetSupplierDetail(QueryMaterial query)
        {
            var detail = _srmMaterialService.GetMaterialDetail(query);
            return Ok(detail);
        }
        [HttpPost("UpdateMaterial")]
        public IActionResult UpdateDeliveryL(ViewSrmMaterial data)
        {
            if (_srmMaterialService.UpdateMaterial(data)) return Ok();
            return BadRequest("料號資料 更新 失敗");
        }
        [HttpPost("CheckMatnr")]
        public IActionResult CheckMatnr(ViewSrmMaterial data)
        {
            if (_srmMaterialService.CheckMatnr(data)) return Ok();
            return BadRequest("此料號以重複使用");
        }

        [HttpPost("UploadFile")]
        [Permission("RFQ_ACTION")]
        public IActionResult UploadFile([FromForm] Model.Models.SRM.FileUploadViewModel fileUploadModel)
        {
            try
            {
                UserClaims user = User.GetUserClaims();
                fileUploadModel.CreateBy = user.UserName;
                var result = _srmMaterialService.UploadAsync(fileUploadModel);
                if (!string.IsNullOrEmpty(result))
                {
                    return this.BadRequestResult(result);
                }
            }
            catch (Exception ex) {
                return this.BadRequestResult(ex.Message);
            }
            return Ok();
        }

        [HttpPost("GetMaterialTrendList")]
        [Permission("price-manage")]
        public IActionResult GetMaterialTrendList(QuerySrmMaterialTrend query)
        {
            return Ok(_srmMaterialService.GetMaterialTrendList(query));
        }
    }
}
