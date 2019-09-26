using CLF.Model.Account.Constants;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CLF.Model.Account
{
    [Table(Tables.AspNetUsers)]
    public class AspNetUsers : IdentityUser
    {
        [MaxLength(250)]
        public string DisplayName { get; set; }

        [MaxLength(250)]
        public string OpenId { get; set; }
    }
}
