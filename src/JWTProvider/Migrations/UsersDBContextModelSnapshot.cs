﻿// <auto-generated />
using System;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JWTProvider.Migrations
{
    [DbContext(typeof(UsersDBContext))]
    partial class UsersDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Infrastructure.DataBase.Login", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DisplayLogin")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Logins");

                    b.HasData(
                        new
                        {
                            UserId = new Guid("851dd5e1-28fb-4e1f-8079-07e62c785b09"),
                            DisplayLogin = "Test"
                        });
                });

            modelBuilder.Entity("Infrastructure.DataBase.Password", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Passwords");

                    b.HasData(
                        new
                        {
                            UserId = new Guid("851dd5e1-28fb-4e1f-8079-07e62c785b09"),
                            Hash = "I3UX9g/lL94qcF4CNNtRiGnhP0E="
                        });
                });

            modelBuilder.Entity("Infrastructure.DataBase.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MiddleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("851dd5e1-28fb-4e1f-8079-07e62c785b09"),
                            Email = "test@mail.ru",
                            FirstName = "Денис",
                            LastName = "Алексеевич",
                            MiddleName = "Смирнов",
                            RoleId = 1L
                        });
                });

            modelBuilder.Entity("Infrastructure.DataBase.UserRole", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserRoles");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 2L,
                            Name = "User"
                        },
                        new
                        {
                            Id = 3L,
                            Name = "App"
                        });
                });

            modelBuilder.Entity("Infrastructure.DataBase.Login", b =>
                {
                    b.HasOne("Infrastructure.DataBase.User", "User")
                        .WithOne("Login")
                        .HasForeignKey("Infrastructure.DataBase.Login", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Infrastructure.DataBase.Password", b =>
                {
                    b.HasOne("Infrastructure.DataBase.User", "User")
                        .WithOne("Password")
                        .HasForeignKey("Infrastructure.DataBase.Password", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Infrastructure.DataBase.User", b =>
                {
                    b.HasOne("Infrastructure.DataBase.UserRole", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Infrastructure.DataBase.User", b =>
                {
                    b.Navigation("Login")
                        .IsRequired();

                    b.Navigation("Password")
                        .IsRequired();
                });

            modelBuilder.Entity("Infrastructure.DataBase.UserRole", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}