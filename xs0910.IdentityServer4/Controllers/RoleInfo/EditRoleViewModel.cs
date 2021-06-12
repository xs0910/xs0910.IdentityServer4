using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerHost.Quickstart.UI
{
    /// <summary>
    /// 编辑角色视图
    /// </summary>
    public class EditRoleViewModel
    {

        public string Id { get; set; }

        [Required]
        [Display(Name = "角色名称")]
        public string Name { get; set; }

        [Display(Name = "描述")]
        public string Description { get; set; }

        [Display(Name = "排序")]
        public int OrderSort { get; set; }

        [Display(Name = "是否启用")]
        public bool Enabled { get; set; }

        public bool System { get; set; }
    }
}
