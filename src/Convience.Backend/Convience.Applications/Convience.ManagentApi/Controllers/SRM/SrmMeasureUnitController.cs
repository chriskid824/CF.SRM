using Convience.Service.SRM;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Convience.ManagentApi.Controllers.SRM
{
    [Route("api/[controller]")]
    [ApiController]
    public class SrmMeasureUnitController : ControllerBase
    {
        private readonly ISrmMeasureUnitService _srmMeasureUnitService;
        public SrmMeasureUnitController(ISrmMeasureUnitService srmMeasureUnitService)
        {
            _srmMeasureUnitService = srmMeasureUnitService;

        }
        [HttpPost("GetMeasureUnit")]
        public IActionResult GetMeasureUnit()
        {
            return Ok(_srmMeasureUnitService.GetMeasureUnit());
        }
    }
}
