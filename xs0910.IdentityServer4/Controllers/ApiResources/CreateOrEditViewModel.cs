using System.ComponentModel.DataAnnotations;

namespace IdentityServerHost.Quickstart.UI
{
    public class CreateOrEditApiViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "资源名称")]
        public string Name { get; set; }

        [Display(Name = "显示名称")]
        public string DisplayName { get; set; }

        [Display(Name = "描述")]
        public string Description { get; set; }

        [Display(Name = "声明")]
        public string UserClaims { get; set; }
    }
}
