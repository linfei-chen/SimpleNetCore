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
    public class MenuNodesInRolesMap: EntityTypeConfiguration<MenuNodesInRoles>
    {
        public override void Configure(EntityTypeBuilder<MenuNodesInRoles> builder)
        {
            builder.HasOne(p => p.MenuNode)
                .WithMany(p => p.MenuNodesInRoles)
                .HasForeignKey(k => k.MenuNodeId);

            builder.HasOne(p => p.AspNetRoles)
                .WithMany(p => p.MenuNodesInRoles)
                .HasForeignKey(k => k.RoleId);

            base.Configure(builder);
        }
    }
}
