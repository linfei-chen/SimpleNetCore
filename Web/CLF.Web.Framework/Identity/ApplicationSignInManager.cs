using CLF.Model.Account;
using CLF.Service.DTO.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CLF.Web.Framework.Identity
{
    public enum SignInStatus
    {
        Success,
        LockedOut,
        RequiresTwoFactorAuthentication,
        InvalidEmail,
        Failure,
        NotFoundUser,
        IncorrectPassword,
        NotApplicationUser,
    }

    public class ApplicationSignInManager : SignInManager<AspNetUsers>
    {
        public ApplicationSignInManager(UserManager<AspNetUsers> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<AspNetUsers> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<AspNetUsers>> logger, IAuthenticationSchemeProvider schemes)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isPersistent"></param>
        /// <param name="shouldLockout"></param>
        /// <returns></returns>
        public async Task<KeyValuePair<SignInStatus, AspNetUsers>> PasswordSignInAsync(SignInDTO model, bool isPersistent = false, bool shouldLockout = false)
        {
            AspNetUsers user = await UserManager.FindByEmailAsync(model.UserName);
            if (user == null)
                user = UserManager.Users.FirstOrDefault(o => o.PhoneNumber == model.UserName);

            var status = await CheckUserStatusAsync(UserManager, user);
            if (status != SignInStatus.Success)
                return new KeyValuePair<SignInStatus, AspNetUsers>(status, user);

            if (await UserManager.CheckPasswordAsync(user, model.Password))
            {
                await UserManager.ResetAccessFailedCountAsync(user);

                if (model.RememberMe)
                    await base.RememberTwoFactorClientAsync(user);
                else
                    await base.SignInAsync(user, isPersistent);

                return new KeyValuePair<SignInStatus, AspNetUsers>(SignInStatus.Success, user);
            }

            if (shouldLockout)
            {
                await UserManager.AccessFailedAsync(user);
                if (await UserManager.IsLockedOutAsync(user))
                {
                    return new KeyValuePair<SignInStatus, AspNetUsers>(SignInStatus.LockedOut, user);
                }
            }
            return new KeyValuePair<SignInStatus, AspNetUsers>(SignInStatus.IncorrectPassword, user);
        }

        private async Task<SignInStatus> CheckUserStatusAsync(UserManager<AspNetUsers> userManager, AspNetUsers user)
        {
            if (user == null)
                return SignInStatus.NotFoundUser;

            if (await userManager.IsEmailConfirmedAsync(user))
                return SignInStatus.InvalidEmail;

            if (await userManager.IsLockedOutAsync(user))
                return SignInStatus.LockedOut;

            return SignInStatus.Success;
        }
    }
}
