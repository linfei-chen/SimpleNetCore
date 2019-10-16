using CLF.Model.Core.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLF.Model.Account
{
   public  class AspNetUserTokens: IdentityUserToken<string>
    {
        public string Issuer { get; set; }
        public DateTime? ExpiredDateTime { get; set; }
    }
}
