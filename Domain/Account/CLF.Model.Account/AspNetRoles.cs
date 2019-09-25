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
            this.MenuNodesInRoles = new List<MenuNodesInRoles>();
            this.PermissionsInRoles = new List<PermissionsInRoles>();
        }
        public string DisplayName { get; set; }

        public string Description { get; set; }

        public virtual ICollection<MenuNodesInRoles> MenuNodesInRoles { get; set; }
        public virtual ICollection<PermissionsInRoles> PermissionsInRoles { get; set; }
    }
}
 