using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;

namespace Decoupling.Decouplers
{
    public partial class Decoupler
    {
        void SetSystemUser(DecouplerIdentitySettings identitySettings)
        {
            var claims = new List<Claim>();
            // fake system user
            claims.Add(new Claim(nameof(identitySettings.Uid).ToLower(), identitySettings.Uid));
            claims.Add(new Claim(nameof(identitySettings.Uname).ToLower(), identitySettings.Uname));
            var identity = new ClaimsIdentity(claims);
            var httpContext = GetService<IHttpContextAccessor>();
            httpContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            };
            Thread.CurrentPrincipal = httpContext.HttpContext.User;
        }
    }
}
