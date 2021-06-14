using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerHost.Quickstart.UI
{
    public class CreateOrEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "客户端Id")]
        public string ClientId { get; set; }

        [Display(Name = "客户端名称")]
        public string ClientName { get; set; }

        [Display(Name = "客户端密钥")]
        public string ClientSecrets { get; set; }

        [Display(Name = "描述")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "授权类型")]
        public string AllowedGrantTypes { get; set; }

        [Display(Name = "作用域")]
        public string AllowedScopes { get; set; }

        [Display(Name = "允许的跨域域名")]
        public string AllowedCorsOrigins { get; set; }

        [Display(Name = "回调地址")]
        public string RedirectUris { get; set; }

        [Display(Name = "退出回调")]
        public string PostLogoutRedirectUris { get; set; }

    }
}
