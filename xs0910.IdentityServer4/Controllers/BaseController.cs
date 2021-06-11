using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerHost.Quickstart.UI
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="description"></param>
        public void AddErrors(string description)
        {
            AddErrors(string.Empty, description);
        }

        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="result"></param>
        public void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                AddErrors(error.Description);
            }
        }

        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="description"></param>
        public void AddErrors(string key, string description)
        {
            ModelState.AddModelError(key, description);
        }


        /// <summary>
        /// 获取ModelState 错误信息
        /// </summary>
        /// <returns></returns>
        public string GetModelStateErrors()
        {
            string result = string.Empty;

            if (!ModelState.IsValid)
            {
                List<string> sb = new List<string>();
                var Keys = ModelState.Keys.ToList();
                foreach (var key in Keys)
                {
                    var errors = ModelState[key].Errors.ToList();
                    foreach (var error in errors)
                    {
                        sb.Add(error.ErrorMessage);
                    }
                }

                result = string.Join(",", sb);
            }

            return result;
        }


        /// <summary>
        /// 跳转链接
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

    }
}
