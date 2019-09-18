﻿// <auto-generated />
using System;
using CLF.DataAccess.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CLF.DataAccess.Account.Migrations
{
    [DbContext(typeof(AccountContext))]
    partial class AccountContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CLF.Model.Account.MenuNode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActionName")
                        .HasMaxLength(128);

                    b.Property<string>("BigIcon")
                        .HasMaxLength(256);

                    b.Property<string>("ControllerName")
                        .HasMaxLength(128);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description")
                        .HasMaxLength(512);

                    b.Property<int>("Index");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("Leaf");

                    b.Property<int>("Level");

                    b.Property<string>("ModifiedBy");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<int?>("ParentId");

                    b.Property<string>("SmallIcon")
                        .HasMaxLength(256);

                    b.Property<string>("TreeCode")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("MenuNode");
                });

            modelBuilder.Entity("CLF.Model.Account.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActionName")
                        .HasMaxLength(128);

                    b.Property<string>("AreaName");

                    b.Property<string>("ControllerName")
                        .HasMaxLength(128);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description")
                        .HasMaxLength(512);

                    b.Property<int>("Index");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("Leaf");

                    b.Property<int>("Level");

                    b.Property<string>("ModifiedBy");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<int?>("ParentId");

                    b.Property<string>("Remark");

                    b.Property<string>("TreeCode")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Permission");
                });

            modelBuilder.Entity("CLF.Model.Account.MenuNode", b =>
                {
                    b.HasOne("CLF.Model.Account.MenuNode", "ParentNode")
                        .WithMany("ChildNodes")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("CLF.Model.Account.Permission", b =>
                {
                    b.HasOne("CLF.Model.Account.Permission", "ParentNode")
                        .WithMany("ChildNodes")
                        .HasForeignKey("ParentId");
                });
#pragma warning restore 612, 618
        }
    }
}
