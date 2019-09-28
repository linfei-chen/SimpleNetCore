using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLF.DataAccess.Account;
using CLF.Model.Account;
using CLF.Service.Account;
using CLF.Service.DTO.Account;
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
        private UserManager<AspNetUsers> _userManager;
        private IAccountService _accountService;

        public  HomeController(IAccountService accountService,UserManager<AspNetUsers> userManager)
        {
            this._accountService = accountService;
            this._userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<ActionResult> Register(RegisterDTO model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.CreateUserAsync(model);
                if (result.Succeeded)
                    return Json(true);

                return ThrowJsonMessage(false, result.Errors.First().Description);
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
    }
}
