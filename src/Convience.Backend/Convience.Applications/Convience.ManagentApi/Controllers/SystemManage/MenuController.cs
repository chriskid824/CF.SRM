using Convience.ManagentApi.Infrastructure.Authorization;
using Convience.ManagentApi.Infrastructure.Logs;
using Convience.Model.Models.SystemManage;
using Convience.Service.SystemManage;
using Convience.Util.Extension;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace Convience.ManagentApi.Controllers.SystemManage
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        [Permission("menuList")]
        public IActionResult Get()
        {
            return Ok(_menuService.GetAllMenu());
        }

        [HttpDelete]
        [Permission("menuDelete")]
        [LogFilter("系统管理", "菜單管理", "删除菜單")]
        public async Task<IActionResult> Delete(int id)
        {
            var isSuccess = await _menuService.DeleteMenuAsync(id);
            if (!isSuccess)
            {
                return this.BadRequestResult("删除失敗!");
            }
            return Ok();
        }

        [HttpPost]
        [Permission("menuAdd")]
        [LogFilter("系统管理", "菜單管理", "創建菜單")]
        public async Task<IActionResult> Add(MenuViewModel menuViewModel)
        {
            var isSuccess = await _menuService.AddMenuAsync(menuViewModel);
            if (!isSuccess)
            {
                return this.BadRequestResult("添加失敗!");
            }
            return Ok();
        }

        [HttpPatch]
        [Permission("menuUpdate")]
        [LogFilter("系统管理", "菜單管理", "更新菜單")]
        public async Task<IActionResult> Update(MenuViewModel menuViewModel)
        {
            var isSuccess = await _menuService.UpdateMenuAsync(menuViewModel);
            if (!isSuccess)
            {
                return this.BadRequestResult("更新失敗!");
            }
            return Ok();
        }

    }
}