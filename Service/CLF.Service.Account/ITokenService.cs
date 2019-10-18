using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CLF.Service.Account
{
    public interface ITokenService
    {
        string GetAccessToken(IEnumerable<Claim> claims);
        string GetRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
