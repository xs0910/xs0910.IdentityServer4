using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xs0910.IdentityServer4.Authorization
{
    public class ClaimRequirement : IAuthorizationRequirement
    {
        public ClaimRequirement(string claimName, string claimValue)
        {
            ClaimName = claimName;
            ClaimValue = claimValue;
        }

        public string ClaimName { get; }
        public string ClaimValue { get; }
    }
}
