using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using xs0910.IdentityServer4.Models;

namespace IdentityServerHost.Quickstart.UI
{
    /// <summary>
    /// 角色信息控制器
    /// </summary>
    public class RoleInfoController : BaseController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleInfoController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        /// <summary>
        /// Role Lists Page
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public IActionResult Index(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var roles = _roleManager.Roles.Where(r => !r.IsDeleted).OrderBy(r => r.OrderSort).ToList();
            return View(roles);
        }
    }
}
