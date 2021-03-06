using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xs0910.IdentityServer4.Models
{
    /// <summary>
    /// 角色表
    /// </summary>
    public class ApplicationRole : IdentityRole
    {
        /// <summary>
        /// 角色描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///排序
        /// </summary>
        public int OrderSort { get; set; }

        /// <summary>
        /// 是否系统内置
        /// </summary>
        public bool System { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 创建ID
        /// </summary>
        public string CreateId { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改ID
        /// </summary>
        public string ModifyId { get; set; }
        /// <summary>
        /// 修改者
        /// </summary>
        public string ModifyBy { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
