using AutoMapper;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using xs0910.IdentityServer4;
using xs0910.IdentityServer4.Models;

namespace IdentityServerHost.Quickstart.UI
{
    /// <summary>
    /// 用户信息控制器
    /// </summary>
    [Authorize]
    [SecurityHeaders]
    public class UserInfoController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserInfoController(UserManager<ApplicationUser> userManager,
                                  RoleManager<ApplicationRole> roleManager,
                                  ApplicationDbContext context,
                                  IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Show User Lists Page
        /// </summary>
        /// <param name="returnUrl">Url</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var users = _userManager.Users
                .Where(r => !r.IsDeleted)
                .OrderByDescending(r => r.CreateTime)
                .ToList();

            return View(users);
        }

        #region Register
        /// <summary>
        /// Register User Page
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Register User Post Action
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null, string roleName = "Normal")
        {
            ViewData["ReturnUrl"] = returnUrl;
            IdentityResult result = new IdentityResult();

            if (ModelState.IsValid)
            {
                var userItem = await _userManager.FindByNameAsync(model.UserName);
                if (userItem == null)
                {
                    var user = _mapper.Map<ApplicationUser>(model);

                    result = await _userManager.CreateAsync(user);

                    if (result.Succeeded)
                    {

                        var role = await _roleManager.FindByNameAsync(roleName);

                        // 添加用户成功后，需要添加claims
                        var claims = new Claim[]
                        {
                            new Claim(JwtClaimTypes.Name,model.UserName),
                            new Claim(JwtClaimTypes.Email,model.Email),
                            new Claim(JwtClaimTypes.EmailVerified,"false",ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.Role,role?.Id)
                        };

                        result = await _userManager.AddClaimsAsync(user, claims);
                        if (result.Succeeded)
                        {
                            return RedirectToLocal(returnUrl);
                        }
                    }
                }
                else
                {
                    // 当前用户已经存在
                    AddErrors($"{userItem?.UserName} already exists");
                }

                AddErrors(result);
            }

            return View(model);
        }


        #endregion

        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(string id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            EditViewModel model = _mapper.Map<EditViewModel>(user);

            model.Claims = await _userManager.GetClaimsAsync(user);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("userinfo/edit/{id}")]
        public async Task<IActionResult> Edit(EditViewModel model, string id, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            IdentityResult result = new IdentityResult();

            if (ModelState.IsValid)
            {
                var userItem = _userManager.FindByIdAsync(id).Result;

                if (userItem != null)
                {
                    var name = userItem.UserName;
                    var email = userItem.Email;

                    userItem.NickName = model.NickName;
                    userItem.RealName = model.RealName;
                    userItem.Email = model.Email;
                    userItem.PhoneNumber = model.PhoneNumber;
                    userItem.Sex = model.Sex;
                    userItem.Birth = model.Birth;
                    userItem.Address = model.Address;
                    userItem.ModifyTime = DateTime.Now;

                    result = await _userManager.UpdateAsync(userItem);

                    if (result.Succeeded)
                    {
                        var removeClaims = await _userManager.RemoveClaimsAsync(
                            userItem,
                            new Claim[] {
                                new Claim(JwtClaimTypes.Name,name),
                                new Claim(JwtClaimTypes.Email,email)
                            });

                        if (removeClaims.Succeeded)
                        {
                            var addClaims = await _userManager.AddClaimsAsync(
                                userItem,
                                new Claim[] {
                                    new Claim(JwtClaimTypes.Name,userItem.UserName),
                                    new Claim(JwtClaimTypes.Email,userItem.Email)
                                }
                            );

                            if (addClaims.Succeeded)
                            {
                                return RedirectToLocal(returnUrl);
                            }
                            else
                            {
                                AddErrors(addClaims);
                            }
                        }
                        else
                        {
                            AddErrors(removeClaims);
                        }
                    }
                    else
                    {
                        AddErrors($"{model.UserName} no exists");
                    }
                }

            }

            AddErrors(result);

            return View(model);
        }


        #endregion

        #region Delete
        public async Task<JsonResult> Delete(string id)
        {
            IdentityResult result = new IdentityResult();

            if (ModelState.IsValid)
            {
                var userItem = _userManager.FindByIdAsync(id).Result;

                if (userItem != null)
                {
                    userItem.IsDeleted = true;

                    result = await _userManager.UpdateAsync(userItem);

                    if (result.Succeeded)
                    {
                        return Json("");
                    }
                }

                AddErrors($"当前用户不存在");
            }
            AddErrors(result);

            return Json(GetModelStateErrors());
        }
        #endregion

        #region Distribute
        [HttpGet]
        public IActionResult Distribute(string id)
        {
            var userItem = _userManager.FindByIdAsync(id).Result;
            ViewData["UserName"] = userItem.UserName;
            ViewData["UserId"] = id;
            var roleIds = _context.UserRoles.Where(r => r.UserId == id).Select(r => r.RoleId).ToList();

            var roleLists = _roleManager.Roles
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.OrderSort)
                .Select(r => new DistributeRoleViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Enabled = r.Enabled,
                    Checked = false
                }).ToList();

            roleLists.ForEach(item =>
            {
                item.Checked = roleIds.Contains(item.Id);
            });

            return View(roleLists);
        }

        [HttpPost]
        public async Task<JsonResult> Distribute(string strLists, string id)
        {
            if (string.IsNullOrEmpty(strLists))
            {
                return Json("数据传输错误");
            }

            var lists = strLists.Split(",").ToList();
            var userItem = await _userManager.FindByIdAsync(id);

            List<Claim> addClaims = new List<Claim>();
            List<Claim> removeClaims = new List<Claim>();

            var entites = _context.UserRoles.Where(r => r.UserId == id).ToList();
            if (entites.Any())
            {
                _context.UserRoles.RemoveRange(entites);
                _context.SaveChanges();
                entites.ForEach(r => removeClaims.Add(new Claim(JwtClaimTypes.Role, r.RoleId)));
                await _userManager.RemoveClaimsAsync(userItem, removeClaims);
            }

            foreach (var item in lists)
            {
                var userRole = new ApplicationUserRole() { UserId = id, RoleId = item };
                _context.UserRoles.Add(userRole);
                addClaims.Add(new Claim(JwtClaimTypes.Role, item));
            }
            _context.SaveChanges();

            await _userManager.AddClaimsAsync(userItem, addClaims);

            return Json("修改成功");
        }

        #endregion
    }
}
