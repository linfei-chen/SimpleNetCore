using CLF.Service.DTO.Account;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CLF.Service.Account
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userName);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        bool AddToken(AspNetUserSecurityTokenDTO model);
        bool ModifyToken(AspNetUserSecurityTokenDTO model);

        AspNetUserSecurityTokenDTO GetAspNetUserSecurityToken(string userName, string refreshToken);
    }
}
