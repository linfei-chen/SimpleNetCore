using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CLF.Model.Account
{
   public class AspNetRoles: IdentityRole
    {
        public AspNetRoles()
        {
            this.MenuNodes = new List<MenuNode>();
            this.Permissions = new List<Permission>();
        }
        public string DisplayName { get; set; }

        public string Description { get; set; }

        public virtual ICollection<MenuNode> MenuNodes { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }

    }
}
