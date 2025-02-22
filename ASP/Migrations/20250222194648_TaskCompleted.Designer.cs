﻿// <auto-generated />
using System;
using ASP.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ASP.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20250222194648_TaskCompleted")]
    partial class TaskCompleted
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("ASP")
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ASP.Data.Entities.UserAccess", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Dk")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.ToTable("UserAccesses", "ASP");
                });

            modelBuilder.Entity("ASP.Data.Entities.UserData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float?>("FootSize")
                        .HasColumnType("real");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RegDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Social")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TorsoSize")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("UsersData", "ASP");
                });

            modelBuilder.Entity("ASP.Data.Entities.UserRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("CanCreate")
                        .HasColumnType("int");

                    b.Property<int>("CanDelete")
                        .HasColumnType("int");

                    b.Property<int>("CanRead")
                        .HasColumnType("int");

                    b.Property<int>("CanUpdate")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserRoles", "ASP");

                    b.HasData(
                        new
                        {
                            Id = "guest",
                            CanCreate = 0,
                            CanDelete = 0,
                            CanRead = 0,
                            CanUpdate = 0,
                            Description = "solely registered user"
                        },
                        new
                        {
                            Id = "editor",
                            CanCreate = 0,
                            CanDelete = 0,
                            CanRead = 1,
                            CanUpdate = 1,
                            Description = "has authority to edit content"
                        },
                        new
                        {
                            Id = "admin",
                            CanCreate = 1,
                            CanDelete = 1,
                            CanRead = 1,
                            CanUpdate = 1,
                            Description = "admin of DB"
                        },
                        new
                        {
                            Id = "moderator",
                            CanCreate = 0,
                            CanDelete = 1,
                            CanRead = 1,
                            CanUpdate = 0,
                            Description = "has authority to block"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
