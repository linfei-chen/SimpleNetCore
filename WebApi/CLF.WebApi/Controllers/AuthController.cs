using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CLF.Common.Configuration;
using CLF.Common.Exceptions;
using CLF.Common.Infrastructure;
using CLF.Model.Account;
using CLF.Service.Account;
using CLF.Service.DTO.Account;
using CLF.Web.Framework.Mvc;
using CLF.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CLF.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private ITokenService _tokenService;
        public AuthController(ITokenService tokenService)
        {
            this._tokenService = tokenService;
        }

        /// <summary>
        /// jwt刷新token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RefreshToken(string token, string refreshToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(token);
            var username = principal.Identity.Name;

            var aspNetUserSecurityToken = _tokenService.GetAspNetUserSecurityToken(username, refreshToken);
            if (aspNetUserSecurityToken == null) return BadRequest();

            var newToken = _tokenService.GenerateAccessToken(username);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            aspNetUserSecurityToken.RefreshToken = newRefreshToken;
            var result = _tokenService.ModifyToken(aspNetUserSecurityToken);
            if (result)
            {
                return new ObjectResult(new { success = true, token = newToken, refreshToken = newRefreshToken });
            }
            return BadRequest();
        }

        /// <summary>
        /// 废除jwt refreshToken
        /// </summary>
        /// <returns></returns>
        [ThrowIfException, Authorize]
        [HttpPost]
        public IActionResult RevokeToken()
        {
            AspNetUserSecurityTokenDTO model = new AspNetUserSecurityTokenDTO
            {
                UserName = User.Identity.Name,
                IsRevoked = true
            };
            var result = _tokenService.ModifyToken(model);
            return ThrowJsonMessage(result);
        }
    }
}