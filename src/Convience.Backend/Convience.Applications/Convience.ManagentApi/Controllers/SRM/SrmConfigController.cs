using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Convience.Entity.Entity.SRM;
using Convience.JwtAuthentication;
using Convience.ManagentApi.Infrastructure.Authorization;
using Convience.Model.Models.SRM;
using Convience.Service.SRM;
using Convience.Util.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Convience.ManagentApi.Controllers.SRM
{
    [Route("api/[controller]")]
    [ApiController]
    public class SrmConfigController : ControllerBase
    {
        private readonly ISrmProcessService _srmProcessService;
        private readonly ISrmSurfaceService _srmSurfaceService;

        public SrmConfigController(ISrmProcessService srmProcessService,ISrmSurfaceService srmSurfaceService)
        {
            _srmProcessService = srmProcessService;
            _srmSurfaceService = srmSurfaceService;
        }
        #region process
        [HttpPost("GetProcessList")]
        [Permission("price")]
        public IActionResult GetProcessList(QuerySrmProcess query)
        {
            return Ok(_srmProcessService.GetProcessList(query));
        }
        [HttpPost("AddProcess")]
        [Permission("price")]
        public IActionResult AddProcess(SrmProcess process)
        {
            var result = _srmProcessService.AddProcess(process);
            if (!string.IsNullOrWhiteSpace(result))
            {
                return this.BadRequestResult(result);
            }
            return Ok();
        }
        #endregion process
        #region surface
        [HttpPost("GetSurfaceList")]
        [Permission("price")]
        public IActionResult GetSurfaceList(QuerySrmSurface query)
        {
            return Ok(_srmSurfaceService.GetSurfaceList(query));
        }
        [HttpPost("AddSurface")]
        [Permission("price")]
        public IActionResult AddSurface(SrmSurface surface)
        {
            var result = _srmSurfaceService.AddSurface(surface);
            if (!string.IsNullOrWhiteSpace(result))
            {
                return this.BadRequestResult(result);
            }
            return Ok();
        }
        #endregion surface
    }
}
