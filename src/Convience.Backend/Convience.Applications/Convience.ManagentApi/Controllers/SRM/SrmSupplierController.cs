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
    //[Authorize]
    public class SrmSupplierController : ControllerBase
    {
        private readonly ISrmSupplierService _srmSupplierService;
        public SrmSupplierController(ISrmSupplierService srmSupplierService)
        {
            _srmSupplierService = srmSupplierService;
        }
        [HttpPost("GetSupplierList")]
        public IActionResult GetSupplierList(QueryVendorModel query)
        {
            return Ok(_srmSupplierService.GetVendor(query));
        }
        [HttpPost("GetSupplierDetail")]
        public IActionResult GetSupplierDetail(QueryVendorModel query)
        {
            var detail = _srmSupplierService.GetSupplierDetail(query);
            return Ok(detail);
        }
        [HttpPost("UpdateSupplier")]
        public IActionResult UpdateDeliveryL(ViewSrmSupplier dls)
        {
            if (_srmSupplierService.UpdateSupplier(dls)) return Ok();
            return BadRequest("供應商資料更新失敗");
        }
        [HttpPost("CheckSupplier")]
        public IActionResult CheckSupplier(ViewSrmSupplier data)
        {
            if (_srmSupplierService.CheckVendor(data)) return Ok();
            return BadRequest("此供應商代碼已重複使用");
        }
        [HttpPost("AddSupplier")]
        public IActionResult AddSupplier(ViewSrmSupplier data)
        {
            if (_srmSupplierService.AddVendor(data)) return Ok();
            return BadRequest("供應商建立失敗");
        }
        [HttpPost("GetVendorID")]
        public IActionResult GetVendorID(ViewSrmSupplier query)
        {
            return Ok(_srmSupplierService.GetVendorID(query));
        }
    }
}
