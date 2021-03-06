using AutoMapper;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using xs0910.IdentityServer4.Models;
using xs0910.IdentityServer4.ViewModels;

namespace IdentityServerHost.Quickstart.UI
{
    /// <summary>
    /// 角色信息控制器
    /// </summary>
    [Authorize]
    [SecurityHeaders]
    public class RoleInfoController : BaseController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;

        public RoleInfoController(RoleManager<ApplicationRole> roleManager,
                                  IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
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

        #region Register
        [HttpGet]
        [Authorize("Admin")]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        public async Task<IActionResult> Register(RegisterRoleViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            IdentityResult result = new IdentityResult();

            if (ModelState.IsValid)
            {
                var roleItem = _roleManager.FindByNameAsync(model.Name).Result;
                if (roleItem == null)
                {
                    roleItem = _mapper.Map<ApplicationRole>(model);
                    roleItem.Enabled = true;
                    roleItem.System = false;
                    roleItem.CreateTime = DateTime.Now;
                    roleItem.CreateId = HttpContext.User.GetSubjectId();
                    roleItem.CreateBy = HttpContext.User.GetDisplayName();

                    result = await _roleManager.CreateAsync(roleItem);

                    if (result.Succeeded)
                    {
                        return RedirectToLocal(returnUrl);
                    }
                }
                else
                {
                    // 当前角色已经存在
                    AddErrors($"{roleItem?.Name} already exists");
                }
                AddErrors(result);
            }

            return View(model);
        }
        #endregion

        #region Delete
        [HttpPost]
        [Authorize("SuperAdmin")]
        public async Task<MessageResult> Delete(string id)
        {
            IdentityResult result = new IdentityResult();
            if (ModelState.IsValid)
            {
                var roleItem = _roleManager.FindByIdAsync(id).Result;

                if (roleItem != null)
                {
                    if (roleItem.IsDeleted)
                    {
                        AddErrors($"{roleItem.Name} 已经被删除了");
                    }
                    else if (roleItem.System)
                    {
                        AddErrors($"{roleItem.Name} 是系统预置角色，不允许删除");
                    }
                    else
                    {
                        roleItem.IsDeleted = true;
                        result = await _roleManager.UpdateAsync(roleItem);
                        if (result.Succeeded)
                        {
                            return new MessageResult();
                        }
                    }
                }
                else
                {
                    AddErrors("当前角色不存在");
                }

                AddErrors(result);
            }
            return MessageResult.Failure(GetModelStateErrors());
        }
        #endregion

        #region Edit
        [HttpGet]
        [Authorize("SuperAdmin")]
        public async Task<IActionResult> Edit(string id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<EditRoleViewModel>(role);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("SuperAdmin")]
        public async Task<IActionResult> Edit(EditRoleViewModel model, string id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            IdentityResult result = new IdentityResult();

            if (ModelState.IsValid)
            {
                var roleItem = _roleManager.FindByIdAsync(model.Id).Result;
                if (roleItem! != null)
                {
                    roleItem.Name = roleItem.System ? roleItem.Name : model.Name;
                    roleItem.Description = model.Description;
                    roleItem.OrderSort = model.OrderSort;
                    roleItem.Enabled = model.Enabled;
                    roleItem.ModifyId = HttpContext.User?.GetSubjectId();
                    roleItem.ModifyBy = HttpContext.User?.GetDisplayName();
                    roleItem.ModifyTime = DateTime.Now;

                    result = await _roleManager.UpdateAsync(roleItem);
                    if (result.Succeeded)
                    {
                        return RedirectToLocal(returnUrl);
                    }
                }
                else
                {
                    AddErrors($"{model?.Name} 不存在");
                }
                AddErrors(result);
            }

            return View(model);
        }

        #endregion

    }
}
