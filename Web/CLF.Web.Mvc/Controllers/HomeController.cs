﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLF.DataAccess.Account;
using CLF.Model.Account;
using CLF.Service.Account;
using CLF.Service.DTO.Account;
using CLF.Web.Framework.Identity;
using CLF.Web.Framework.Infrastructure;
using CLF.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CLF.Service.Core.Messages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace CLF.Web.Mvc.Controllers
{
    public class HomeController : BaseController
    {
        private ApplicationSignInManager _applicationSignInManager;
        private UserManager<AspNetUsers> _userManager;
        private IAccountService _accountService;
        private IEmailSender _emailSender;
        public HomeController (IEmailSender emailSender, IAccountService accountService,UserManager<AspNetUsers> userManager,ApplicationSignInManager applicationSignInManager)
        {
            this._emailSender = emailSender;
            this._userManager = userManager;
            this._accountService = accountService;
            this._applicationSignInManager = applicationSignInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            SignInDTO model = new SignInDTO();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ThrowIfException]
        public async Task<ActionResult> Login([FromBody]SignInDTO model, string returnUrl)
        {
            if(string.IsNullOrEmpty(returnUrl))
                return ThrowJsonMessage(false);

            if (!ModelState.IsValid)
            {
                return ThrowJsonMessage(false, GetModelStateErrorMessage());
            }

            var result = new KeyValuePair<SignInStatus, AspNetUsers>(SignInStatus.Failure, null);
            result = await _applicationSignInManager.PasswordSignInAsync(model);
            switch (result.Key)
            {
                case SignInStatus.InvalidEmail:
                    ModelState.AddModelError("Email", "注册邮件尚未激活！");
                    break;
                case SignInStatus.NotFoundUser:
                    ModelState.AddModelError("Email", "账户或密码错误，请重新输入！");
                    break;
                case SignInStatus.LockedOut:
                    ModelState.AddModelError("Email", "账户已被锁定，请稍后登陆！");
                    break;
                case SignInStatus.Success:
                    return Redirect(returnUrl);
            }
            return ThrowJsonMessage(false, GetModelStateErrorMessage());
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            RegisterDTO model = new RegisterDTO();
            return View(model);
        }

        [AllowAnonymous]
        //[AutoValidateAntiforgeryToken]
        [HttpPost]
        [ThrowIfException]
        public async Task<ActionResult> Register([FromBody]RegisterDTO model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.CreateUserAsync(model);
                if (result.Key.Succeeded)
                {
                    //发送验证邮件
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var transformCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action("ConfirmEmail", "Home", new { email = result.Value.Email, code = transformCode });
                    EmailMessage emailMessage = new EmailMessage
                    {
                        Subject = "注册激活",
                        Body = callbackUrl,
                        To = new List<string> { model.Email }
                    };
                    await  _emailSender.SendAsync(emailMessage);
                    return Json(true);
                }
                return ThrowJsonMessage(false, result.Key.Errors.First().Description);
            }
            return ThrowJsonMessage(false, GetModelStateErrorMessage());
        }

        [AllowAnonymous]
        [ThrowIfException]
        public async Task<JsonResult> ExistEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return Json(false);

            var result = await _userManager.FindByEmailAsync(email);
            return Json(result == null);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string email, string code)
        {
            if (!string.IsNullOrEmpty(email) &&!string.IsNullOrEmpty(code))
            {
                var user = await _userManager.FindByEmailAsync(email);
                var transformCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var result = await _userManager.ConfirmEmailAsync(user,transformCode );
                if (result.Succeeded)
                {
                    return View();
                }
                return View("Error");
            }
            return View("Error");
        }
    }
}
