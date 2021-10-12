using Convience.Entity.Entity.SRM;
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
        public IActionResult GetMaterialList(QueryMaterial query)
        {
            return Ok(_srmMaterialService.GetMaterialList(query));
        }
        [HttpPost("GetMaterialDetail")]
        public IActionResult GetSupplierDetail(QueryMaterial query)
        {
            var detail = _srmMaterialService.GetMaterialDetail(query);
            return Ok(detail);
        }
        [HttpPost("UpdateMaterial")]
        public IActionResult UpdateDeliveryL(ViewSrmMatnr1 data)
        {
            if (_srmMaterialService.UpdateMaterial(data)) return Ok();
            return BadRequest("料號資料 更新 失敗");
        }
        [HttpPost("Checkdata")]
        public IActionResult Checkdata(ViewSrmMatnr1 data)
        {
            if (string.IsNullOrWhiteSpace(_srmMaterialService.Checkdata(data))) return Ok();
            return BadRequest(_srmMaterialService.Checkdata(data));
        }
        [HttpPost("AddMatnr")]
        public IActionResult AddMatnr(ViewSrmMatnr1 data)
        {
            return Ok(_srmMaterialService.AddMatnr(data));
        }
        [HttpPost("GetEkgrp")]
        public IActionResult GetEkgrp(SrmEkgry data)
        {
            return Ok(_srmMaterialService.GetEkgrp(data));
        }
        [HttpPost("GetGroupList")]
        public IActionResult GetGroupList(QueryMaterial data)
        {
            return Ok(_srmMaterialService.GetGroupList(data));
        }
        [HttpPost("GetUnitList")]
        public IActionResult GetUnitList(QueryMaterial data)
        {
            return Ok(_srmMaterialService.GetUnitList(data));
        }
    }
}
