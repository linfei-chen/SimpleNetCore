using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLF.Model.Account
{
    public class AspNetUsers : IdentityUser
    {
        public string DisplayName { get; set; }

        public string OpenId { get; set; }
    }
}
