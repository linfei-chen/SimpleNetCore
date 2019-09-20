using CLF.Model.Account;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CLF.DataAccess.Account.Repository
{
   public class RoleRepository:RoleStore<AspNetRoles>
    {
        public RoleRepository(DbContext context) : base(context) { }

        public AspNetRoles GetById(string roleId)
        {
            return base.Context.Set<AspNetRoles>().Find(roleId);
        }

        public async Task<bool> AddPermission(string roleid, int permissionId)
        {
            var permission = await base.Context.Set<Permission>().FirstOrDefaultAsync(m => m.Id == permissionId);
            var role = await base.Context.Set<AspNetRoles>().FirstOrDefaultAsync(m => m.Id == roleid);
            if (permission != null && role != null && !role.Permissions.Contains(permission))
            {
                role.Permissions.Add(permission);
                return base.Context.SaveChanges() > 0;
            }
            return true;
        }
    }
}
