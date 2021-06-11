using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace IdentityServerHost.Quickstart.UI
{
    /// <summary>
    /// 编辑用户ViewModel
    /// </summary>
    public class EditViewModel
    {
        public string Id { get; set; }

        [Display(Name ="账号")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "昵称")]
        public string NickName { get; set; }

        [Display(Name = "真实名称")]
        public string RealName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "手机号码")]
        public string PhoneNumber { get; set; }

        [Display(Name = "性别")]
        public int Sex { get; set; } = 0;

        [Display(Name = "生日")]
        public DateTime Birth { get; set; } = DateTime.Now;

        [Display(Name = "地址")]
        public string Address { get; set; } = string.Empty;

        public IList<Claim> Claims { get; set; }
    }
}
