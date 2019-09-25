﻿using CLF.Domain.Core.Mapping;
using CLF.Model.Account;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLF.DataAccess.Account.Mapping
{
    /// <summary>
    /// 多对多的Map
    /// </summary>
    public class PermissionsInRolesMap : EntityTypeConfiguration<PermissionsInRoles>
    {
        public override void Configure(EntityTypeBuilder<PermissionsInRoles> builder)
        {
            builder.HasOne(p => p.Permission)
                .WithMany(p => p.PermissionsInRoles)
                .HasForeignKey(k => k.PermissionId);

            builder.HasOne(p => p.AspNetRoles)
                .WithMany(p => p.PermissionsInRoles)
                .HasForeignKey(k => k.RoleId);

            base.Configure(builder);
        }
    }
}
