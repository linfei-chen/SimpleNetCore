using CLF.Model.Account;
using CLF.Service.Account;
using CLF.Service.DTO.Account;
using CLF.Web.Framework.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CLF.WebApi.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController: ControllerBase
    {
        private ApplicationSignInManager _applicationSignInManager;
        private ITokenService _tokenService;
        public AccountController(ApplicationSignInManager applicationSignInManager,ITokenService tokenService)
        {
            this._applicationSignInManager = applicationSignInManager;
            this._tokenService = tokenService;
        }

        /// <summary>
        /// 登录成功后保存jwt的token，refreshToken
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(string userName,string password)
        {
            var result = new KeyValuePair<SignInStatus, AspNetUsers>(SignInStatus.Failure, null);
            SignInDTO model = new SignInDTO
            {
                UserName = userName,
                Password = password
            };
            result = await _applicationSignInManager.PasswordSignInAsync(model);
            if (result.Key != SignInStatus.Success)
                return BadRequest();

            var claims = new[]
             {
                new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                new Claim(ClaimTypes.Name, userName)
            };

            var token = _tokenService.GetAccessToken(claims);
            var refreshToken = _tokenService.GetRefreshToken();

            var securityToken = new AspNetUserSecurityTokenDTO
            {
                UserName = userName,
                RefreshToken = refreshToken
            };

            var isSave=_tokenService.AddToken(securityToken);
            if (isSave)
                return new ObjectResult(new { success = true, token = token, refreshToken = refreshToken });

            return BadRequest();
        }
    }
}