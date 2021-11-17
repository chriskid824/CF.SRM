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
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Convience.ManagentApi.Controllers.SRM
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SrmDisscussionController : ControllerBase
    {
        private readonly ISrmDisscussionService _srmDisscussionService;
        private readonly ISrmPoService _srmPoService;
        private readonly IUserService _userService;

        public SrmDisscussionController(ISrmDisscussionService srmDisscussionService, ISrmPoService srmPoService, IUserService userService)
        {
            _srmDisscussionService = srmDisscussionService;
            _srmPoService = srmPoService;
            _userService = userService;
        }

        [HttpPost("AddTitle")]
        public IActionResult AddTitle(ViewSrmDisscussionH title)
        {
            title.CreateBy = User.GetUserName();
            title.CreateDate = DateTime.Now;
            if (_srmDisscussionService.AddTitle(title)) return Ok();
            return BadRequest("新增文章失敗");
        }
        [HttpPost("AddContent")]
        public IActionResult AddContent(SrmDisscussionC content)
        {
            content.CreateBy = User.GetUserName();
            content.CreateDate = DateTime.Now;
            if (_srmDisscussionService.AddContent(content)) return Ok();
            return BadRequest("新增內容失敗");
        }
        [HttpPost("UpdateContent")]
        public IActionResult UpdateContent(SrmDisscussionC content)
        {
            content.LastUpdateBy = User.GetUserName();
            content.LastUpdateDate = DateTime.Now;
            if (_srmDisscussionService.UpdateContent(content)) return Ok();
            return BadRequest("新增內容失敗");
        }
        [HttpPost("DeleteContent")]
        public IActionResult DeleteContent(SrmDisscussionC content)
        {
            content.LastUpdateBy = User.GetUserName();
            content.LastUpdateDate = DateTime.Now;
            if (_srmDisscussionService.DeleteContent(content)) return Ok();
            return BadRequest("新增內容失敗");
        }
        [HttpPost("GetDissList")]
        public IActionResult GetDissList(JObject query)
        {
            QueryFile q = new QueryFile();
            q.user = User;
            int number;
            bool success=Int32.TryParse(query["id"].ToString(), out number);
            if (success)
            {
                q.id = number;
            }
            //q.id =  query["id"].TRY.Type!= JTokenType.Null ? Convert.ToInt16(query["id"]):null;
            int page = (int)query["page"];
            int size = (int)query["size"];
            var aaa = _srmDisscussionService.GetDissList(q,page,size);

            return Ok(aaa);
        }
        [HttpPost("GetDisscussion")]
        public IActionResult GetDisscussion(JObject query)
        {
            QueryFile q = new QueryFile();
            
            if (query["id"].Type== JTokenType.Null)
            {
                return BadRequest("沒有選擇項目");
            }
            q.id = Convert.ToInt16(query["id"]);
            q.user = User;
            int page = (int)query["page"];
            int size = (int)query["size"];
            var h = _srmDisscussionService.GetDisscussion(q, page, size);
            return Ok(h);
        }
        [HttpGet("GetNumberList")]
        public IActionResult GetNumberList(int id)
        {
            List<ViewSelects> selects = new List<ViewSelects>();

            switch (id)
            {
                //case 7:
                //    break;
                //case 8:
                //    break;
                default:
                    QueryPoList queryPo = new QueryPoList();
                    queryPo.user = User;
                    selects = _srmPoService.GetAll(queryPo).Select(p => new ViewSelects() { id = p.PoNum, name = p.PoNum }).ToList();
                    break;
            }

            return Ok(selects);
        }
    }
}