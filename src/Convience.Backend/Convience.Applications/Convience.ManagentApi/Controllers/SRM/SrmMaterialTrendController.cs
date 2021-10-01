using Convience.Entity.Entity.SRM;
using Convience.JwtAuthentication;
using Convience.ManagentApi.Infrastructure.Authorization;
using Convience.Model.Models.SRM;
using Convience.Service.SRM;
using Convience.Util.Extension;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Convience.ManagentApi.Controllers.SRM
{
    [Route("api/[controller]")]
    [ApiController]
    public class SrmMaterialTrendController : ControllerBase
    {
        private readonly ISrmMaterialTrendService _srmMaterialTrendService;

        public SrmMaterialTrendController(ISrmMaterialTrendService srmMaterialTrendService)
        {
            _srmMaterialTrendService = srmMaterialTrendService;
        }
        [HttpPost("UploadFile")]
        [Permission("price")]
        public IActionResult UploadFile([FromForm] Model.Models.SRM.FileUploadViewModel fileUploadModel)
        {
            try
            {
                UserClaims user = User.GetUserClaims();
                fileUploadModel.CreateBy = user.UserName;
                var result = _srmMaterialTrendService.UploadAsync(fileUploadModel);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    return this.BadRequestResult(result);
                }
            }
            catch (Exception ex)
            {
                return this.BadRequestResult(ex.Message);
            }
            return Ok();
        }

        [HttpPost("GetMaterialTrendList")]
        [Permission("price")]
        public IActionResult GetMaterialTrendList(QuerySrmMaterialTrend query)
        {
            return Ok(_srmMaterialTrendService.GetMaterialTrendList(query));
        }
        [HttpPost("GetMaterialList")]
        [Permission("price")]
        public IActionResult GetMaterialList(QuerySrmMaterial query)
        {
            return Ok(_srmMaterialTrendService.GetMaterialList(query));
        }
        [HttpPost("AddMaterial")]
        [Permission("price")]
        public IActionResult AddMaterial(SrmMaterial material)
        {
            var result = _srmMaterialTrendService.AddMaterial(material);
            if (!string.IsNullOrWhiteSpace(result))
            {
                return this.BadRequestResult(result);
            }
            return Ok();
        }
    }
}
