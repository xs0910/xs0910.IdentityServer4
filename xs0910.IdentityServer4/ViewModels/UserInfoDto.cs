using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xs0910.IdentityServer4.ViewModels
{
    /// <summary>
    /// 用户信息表
    /// </summary>
    public class UserInfoDto
    {
        public UserInfoDto() { }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 登录账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 真实名称
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPwd { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 性别
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

        public bool IsDeleted { get; set; }
    }
}
