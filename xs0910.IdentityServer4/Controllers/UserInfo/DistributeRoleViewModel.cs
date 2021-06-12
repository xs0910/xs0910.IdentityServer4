using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerHost.Quickstart.UI
{
    public class DistributeRoleViewModel
    {
        public string Id { get; set; }

        [Display(Name = "角色名称")]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool Enabled { get; set; }

        /// <summary>
        /// 当前角色是否被选中
        /// </summary>
        public bool Checked { get; set; }
    }
}
