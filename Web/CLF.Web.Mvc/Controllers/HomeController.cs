using System;
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

namespace CLF.Web.Mvc.Controllers
{
    public class HomeController : BaseController
    {
        private ApplicationSignInManager _applicationSignInManager;
        private UserManager<AspNetUsers> _userManager;
        private IAccountService _accountService;

        public HomeController(IAccountService accountService, ApplicationSignInManager applicationSignInManager, UserManager<AspNetUsers> userManager)
        {
            this._applicationSignInManager = applicationSignInManager;
            this._userManager = userManager;
            this._accountService = accountService;
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
        public async Task<ActionResult> Login(SignInDTO model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return ThrowJsonMessage(false, GetModelStateErrorMessage());
            }

            var result = new KeyValuePair<SignInStatus, AspNetUsers>(SignInStatus.Failure, null);
            result = await _applicationSignInManager.PasswordSignInAsync(model);
            switch (result.Key)
            {
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
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        [ThrowIfException]
        public async Task<ActionResult> Register(RegisterDTO model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.CreateUserAsync(model);
                if (result.Key.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(result.Value);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { user = result.Value, code = code });

                    //发送邮件
                    //await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);
                    //保持登陆
                    SignInDTO signInModel = new SignInDTO { UserName = model.Email, Password = model.Password };
                    await _applicationSignInManager.PasswordSignInAsync(signInModel, false, false);

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
        public async Task<ActionResult> ConfirmEmail(AspNetUsers user,string code)
        {
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return View();
            }
            else
            {
                return View("Error");
            }
        }
    }
}
