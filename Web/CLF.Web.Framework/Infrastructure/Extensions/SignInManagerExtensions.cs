using CLF.Model.Account;
using CLF.Service.DTO.Account;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLF.Web.Framework.Infrastructure.Extensions
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

    public static  class SignInManagerExtensions
    {
        public static async Task<KeyValuePair<SignInStatus, AspNetUsers>> PasswordSignInAsync(this SignInManager<AspNetUsers> signInManager,  SignInDTO model)
        {
            var userManager = signInManager.UserManager;
            AspNetUsers user = await userManager.FindByEmailAsync(model.UserName);
            if (user == null)
                user = userManager.Users.FirstOrDefault(o => o.PhoneNumber == model.UserName);
            if(user==null)
            {
                return new KeyValuePair<SignInStatus, AspNetUsers>(SignInStatus.NotFoundUser, user);
            }

            var status = await CheckUserStatusAsync(signInManager.UserManager,user);
            if (status != SignInStatus.Success)
                return new KeyValuePair<SignInStatus, AspNetUsers>(status, user);

            if(await signInManager.UserManager.CheckPasswordAsync(user,model.Password))
            {
                await userManager.ResetAccessFailedCountAsync(user);

                return new KeyValuePair<SignInStatus, AspNetUsers>(
               await SignInOrTwoFactorAsync(user, false, isPersistent, model), user);
            }

        }

        private static  async Task<SignInStatus> CheckUserStatusAsync(UserManager<AspNetUsers> userManager,AspNetUsers user)
        {
            if (user == null)
                return SignInStatus.NotFoundUser;

            if (await userManager.IsLockedOutAsync(user))
                return SignInStatus.LockedOut;

            return SignInStatus.Success;
        }
    }
}
}
