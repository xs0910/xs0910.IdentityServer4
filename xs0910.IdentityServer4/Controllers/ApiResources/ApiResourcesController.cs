using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using xs0910.IdentityServer4.ViewModels;

namespace IdentityServerHost.Quickstart.UI
{
    public class ApiResourcesController : BaseController
    {
        private readonly ConfigurationDbContext _context;
        public ApiResourcesController(ConfigurationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var list = await _context.ApiResources
                .Include(r => r.UserClaims)
                .ToListAsync();

            return View(list);
        }

        /// <summary>
        /// 新增/修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CreateOrEdit(int id = 0, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (id > 0)
            {
                var model = (await _context.ApiResources
                    .Include(r => r.UserClaims)
                    .ToListAsync())
                    .FirstOrDefault(r => r.Id == id)
                    .ToModel();

                if (model != null)
                {
                    var vm = new CreateOrEditApiViewModel()
                    {
                        Id = id,
                        Name = model.Name,
                        DisplayName = model.DisplayName,
                        Description = model.Description,
                        UserClaims = string.Join(",", model?.UserClaims)
                    };
                    return View(vm);
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrEdit(CreateOrEditApiViewModel model, string returnUrl = null)
        {
            // 新增
            if (model != null && model.Id == 0)
            {
                ApiResource apiResource = new ApiResource()
                {
                    Name = model.Name,
                    DisplayName = model.DisplayName,
                    Description = model.Description,
                    UserClaims = model?.UserClaims?.Split(","),
                    Enabled = true
                };

                await _context.ApiResources.AddAsync(apiResource.ToEntity());
                await _context.SaveChangesAsync();
            }
            // 修改
            if (model != null && model.Id > 0)
            {
                var entity = (await _context.ApiResources
                    .Include(r => r.UserClaims)
                    .ToListAsync())
                    .FirstOrDefault(r => r.Id == model.Id);
                entity.Name = model.Name;
                entity.DisplayName = model.DisplayName;
                entity.Description = model.Description;

                var claims = new List<IdentityServer4.EntityFramework.Entities.ApiResourceClaim>();
                if (!string.IsNullOrEmpty(model?.UserClaims))
                {
                    model?.UserClaims?.Split(",").Where(r => r != "" && r != null).ToList()
                        .ForEach(r =>
                        {
                            claims.Add(new IdentityServer4.EntityFramework.Entities.ApiResourceClaim()
                            {
                                ApiResource = entity,
                                ApiResourceId = entity.Id,
                                Type = r
                            });
                        });

                    entity.UserClaims = claims;
                }

                _context.ApiResources.Update(entity);
                await _context.SaveChangesAsync();
            }

            return RedirectToLocal(returnUrl);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageResult> Delete(int id)
        {
            if (id > 0)
            {
                var model = (await _context.ApiResources
                    .Include(r => r.UserClaims)
                    .ToListAsync())
                    .FirstOrDefault(r => r.Id == id);
                if (model != null)
                {
                    _context.ApiResources.Remove(model);
                    await _context.SaveChangesAsync();

                    return new MessageResult();
                }
            }
            return new MessageResult(201, false, "数据不存在，无法删除");
        }
    }
}
