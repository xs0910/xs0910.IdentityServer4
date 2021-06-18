using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xs0910.IdentityServer4.Authorization
{
    /// <summary>
    /// 自定义授权处理Handler
    /// </summary>
    public class ClaimRequirementHandler : AuthorizationHandler<ClaimRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimRequirement requirement)
        {
            var claim = context.User.Claims.FirstOrDefault(r => r.Type == requirement.ClaimName && requirement.ClaimValue.Split(',').Contains(r.Value));
            if (claim != null)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
