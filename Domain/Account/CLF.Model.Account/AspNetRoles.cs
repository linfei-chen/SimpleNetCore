using CLF.Model.Account.Constants;
using CLF.Model.Core.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace CLF.Model.Account
{
    [Table(Tables.AspNetRoles)]
    public class AspNetRoles : IdentityRole
    {
        public AspNetRoles()
        {
            this.MenuNodesInRoles = new List<MenuNodesInRoles>();
            this.PermissionsInRoles = new List<PermissionsInRoles>();
        }
        [MaxLength(250)]
        public string DisplayName { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public virtual ICollection<MenuNodesInRoles> MenuNodesInRoles { get; set; }
        public virtual ICollection<PermissionsInRoles> PermissionsInRoles { get; set; }
    }
}
 