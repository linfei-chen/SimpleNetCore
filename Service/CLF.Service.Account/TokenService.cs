using CLF.Common.Configuration;
using CLF.Common.Extensions;
using CLF.Common.Infrastructure;
using CLF.Common.SecurityHelper;
using CLF.DataAccess.Account;
using CLF.Domain.Core.EFRepository;
using CLF.Model.Account;
using CLF.Service.Core.Extensions;
using CLF.Service.DTO.Account;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace CLF.Service.Account
{
    public class TokenService : ITokenService
    {
        private readonly JwtConfig jwtConfig = EngineContext.Current.Resolve<JwtConfig>();
        private CommonRepository<AspNetUserSecurityToken> _securityTokenRepository;
        public TokenService()
        {
            this._securityTokenRepository = new CommonRepository<AspNetUserSecurityToken>(new AccountUnitOfWorkContext());
        }
        public string GetAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecurityKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Issuer,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(jwtConfig.ExpireTime),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,//过期也可以验证
                ValidateIssuerSigningKey = true,
                ValidAudience = jwtConfig.Issuer,
                ValidIssuer = jwtConfig.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecurityKey))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public string GetRefreshToken()
        {
            return RandomProvider.GenerateRandom();
        }

        public bool AddToken(AspNetUserSecurityTokenDTO model)
        {
            var securityToken = model.Map<AspNetUserSecurityToken>();
            return _securityTokenRepository.Add(securityToken);
        }

        public bool ModifyToken(AspNetUserSecurityTokenDTO model)
        {
            Expression<Func<AspNetUserSecurityToken, bool>> expression = o => !o.IsDeleted;

            if (!string.IsNullOrEmpty(model.ClientId))
                expression = expression.And(o => o.ClientId == model.ClientId);

            if (!string.IsNullOrEmpty(model.UserName))
                expression = expression.And(o => o.UserName == model.UserName);

            var securityToken = _securityTokenRepository.Find(expression);
            securityToken.ToList().ForEach(o =>
            {
                if (!string.IsNullOrEmpty(model.RefreshToken))
                    o.RefreshToken = model.RefreshToken;
                o.IsRevoked = model.IsRevoked;
            });
            return _securityTokenRepository.Modify(securityToken, new string[] { "RefreshToken", "IsRevoked" }) > 0;
        }

        public AspNetUserSecurityTokenDTO GetAspNetUserSecurityToken(string userName, string refreshToken)
        {
            var data = _securityTokenRepository.FindByFilter(o => o.UserName == userName && o.RefreshToken == refreshToken
                    && !o.IsDeleted && !o.IsRevoked);
            return data.Map<AspNetUserSecurityTokenDTO>();
        }
    }
}
