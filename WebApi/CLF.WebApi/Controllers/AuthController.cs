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
using CLF.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CLF.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [ThrowIfException]
        public ActionResult GetToken(string userName, string password)
        {
            var config = EngineContext.Current.Resolve<JwtConfig>();
            if (config == null)
                throw new BusinessPromptException($"{nameof(config)}不能为空");

            if (string.Equals(userName, config.AppId) && string.Equals(password, config.AppSecret))
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMinutes(30)).ToUnixTimeSeconds()}"),
                    new Claim(ClaimTypes.Name, userName)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.SecurityKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: config.Domain,
                    audience: config.Domain,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            else
            {
                return BadRequest(new { success = false, message = "用户名或密码错误" });
            }
        }
    }
}