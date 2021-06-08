using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xs0910.IdentityServer4.Models
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 真实名称
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 性别:0-保密，1-男，2-女
        /// </summary>
        public int Sex { get; set; } = 0;

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birth { get; set; } = DateTime.Now;

        /// <summary>
        /// 住址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; } = DateTime.Now;


        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 关联角色信息
        /// </summary>
        public ICollection<ApplicationUserRole> UserRoles; 
    }
}
