using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using xs0910.IdentityServer4.ViewModels;
using Secret = IdentityServer4.Models.Secret;

namespace IdentityServerHost.Quickstart.UI
{
    /// <summary>
    /// 客户端
    /// </summary>
    [Authorize]
    public class ClientsController : BaseController
    {
        private readonly ConfigurationDbContext _context;

        public ClientsController(ConfigurationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var list = await _context.Clients
                .Include(r => r.AllowedGrantTypes)
                .Include(r => r.AllowedScopes)
                .Include(r => r.AllowedCorsOrigins)
                .Include(r => r.RedirectUris)
                .Include(r => r.PostLogoutRedirectUris)
                .ToListAsync();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrEdit(int id = 0, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (id > 0)
            {
                var model = (await _context.Clients
                    .Include(r => r.AllowedGrantTypes)
                    .Include(r => r.AllowedScopes)
                    .Include(r => r.AllowedCorsOrigins)
                    .Include(r => r.ClientSecrets)
                    .Include(r => r.RedirectUris)
                    .Include(r => r.PostLogoutRedirectUris)
                    .ToListAsync())
                    .FirstOrDefault(r => r.Id == id)
                    .ToModel();

                var vm = new CreateOrEditViewModel();
                if (model != null)
                {
                    vm.Id = id;
                    vm.ClientId = model?.ClientId;
                    vm.ClientName = model?.ClientName;
                    vm.Description = model?.Description;
                    vm.AllowedGrantTypes = string.Join(",", model?.AllowedGrantTypes);
                    vm.AllowedScopes = string.Join(",", model?.AllowedScopes);
                    vm.AllowedCorsOrigins = string.Join(",", model?.AllowedCorsOrigins);
                    vm.ClientSecrets = string.Join(",", model?.ClientSecrets);
                    vm.RedirectUris = string.Join(",", model?.RedirectUris);
                    vm.PostLogoutRedirectUris = string.Join(",", model?.PostLogoutRedirectUris);
                }

                return View(vm);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// 新增或者修改Post事件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrEdit(CreateOrEditViewModel model, string returnUrl = null)
        {
            // 新增
            if (model != null && model.Id == 0)
            {
                IdentityServer4.Models.Client client = new IdentityServer4.Models.Client()
                {
                    ClientId = model.ClientId,
                    ClientName = model.ClientName,
                    Description = model.Description,
                    AllowedCorsOrigins = model?.AllowedCorsOrigins?.Split(","),
                    AllowedGrantTypes = model?.AllowedGrantTypes?.Split(","),
                    AllowedScopes = model?.AllowedScopes?.Split(","),
                    PostLogoutRedirectUris = model?.PostLogoutRedirectUris?.Split(","),
                    RedirectUris = model?.RedirectUris?.Split(",")
                };

                if (!string.IsNullOrEmpty(model.ClientSecrets))
                {
                    client.ClientSecrets = new List<Secret>() { new Secret(model.ClientSecrets.Sha256()) };
                }

                await _context.Clients.AddAsync(client.ToEntity());
                await _context.SaveChangesAsync();
            }

            if (model != null && model.Id > 0)
            {
                var edit = (await _context.Clients
                    .Include(r => r.AllowedGrantTypes)
                    .Include(r => r.AllowedScopes)
                    .Include(r => r.AllowedCorsOrigins)
                    .Include(r => r.ClientSecrets)
                    .Include(r => r.RedirectUris)
                    .Include(r => r.PostLogoutRedirectUris)
                    .ToListAsync()
                    ).FirstOrDefault(r => r.Id == model.Id);

                edit.ClientId = model.ClientId;
                edit.ClientName = model.ClientName;
                edit.Description = model.Description;

                var allowedCorsOrigins = new List<ClientCorsOrigin>();
                if (!string.IsNullOrEmpty(model.AllowedCorsOrigins))
                {
                    model.AllowedCorsOrigins.Split(",").Where(r => r != "" && r != null).ToList()
                        .ForEach(r =>
                        {
                            allowedCorsOrigins.Add(new ClientCorsOrigin()
                            {
                                Client = edit,
                                ClientId = edit.Id,
                                Origin = r
                            });
                        });
                    edit.AllowedCorsOrigins = allowedCorsOrigins;
                }

                var allowedGrantTypes = new List<ClientGrantType>();
                if (!string.IsNullOrEmpty(model.AllowedGrantTypes))
                {
                    model.AllowedGrantTypes.Split(",").Where(r => r != "" && r != null).ToList()
                        .ForEach(r =>
                        {
                            allowedGrantTypes.Add(new ClientGrantType
                            {
                                Client = edit,
                                ClientId = edit.Id,
                                GrantType = r
                            });
                        });
                    edit.AllowedGrantTypes = allowedGrantTypes;
                }

                var allowedScopes = new List<ClientScope>();
                if (!string.IsNullOrEmpty(model.AllowedScopes))
                {
                    model.AllowedScopes.Split(",").Where(r => r != "" && r != null).ToList()
                        .ForEach(r =>
                        {
                            allowedScopes.Add(new ClientScope
                            {
                                Client = edit,
                                ClientId = edit.Id,
                                Scope = r
                            });
                        });
                    edit.AllowedScopes = allowedScopes;
                }

                var redirectUris = new List<ClientRedirectUri>();
                if (!string.IsNullOrEmpty(model.RedirectUris))
                {
                    model.RedirectUris.Split(",").Where(r => r != "" && r != null).ToList()
                        .ForEach(r =>
                        {
                            redirectUris.Add(new ClientRedirectUri
                            {
                                Client = edit,
                                ClientId = edit.Id,
                                RedirectUri = r
                            });
                        });
                    edit.RedirectUris = redirectUris;
                }

                var postLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>();
                if (!string.IsNullOrEmpty(model.PostLogoutRedirectUris))
                {
                    model.PostLogoutRedirectUris.Split(",").Where(r => r != "" && r != null).ToList()
                        .ForEach(r =>
                        {
                            postLogoutRedirectUris.Add(new ClientPostLogoutRedirectUri
                            {
                                Client = edit,
                                ClientId = edit.Id,
                                PostLogoutRedirectUri = r
                            });
                        });
                    edit.PostLogoutRedirectUris = postLogoutRedirectUris;
                }

                _context.Clients.Update(edit);
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
                var model = (await _context.Clients
                    .Include(r => r.AllowedGrantTypes)
                    .Include(r => r.AllowedScopes)
                    .Include(r => r.AllowedCorsOrigins)
                    .Include(r => r.ClientSecrets)
                    .Include(r => r.RedirectUris)
                    .Include(r => r.PostLogoutRedirectUris)
                    .ToListAsync())
                    .FirstOrDefault(r => r.Id == id);
                if (model != null)
                {
                    _context.Clients.Remove(model);
                    await _context.SaveChangesAsync();

                    return new MessageResult();
                }
            }
            return new MessageResult(201, false, "数据不存在，无法删除");
        }
    }
}
