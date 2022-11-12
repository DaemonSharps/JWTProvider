﻿// <auto-generated />
using System;
using Infrastructure.DataBase.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace JWTProvider.Migrations
{
    [DbContext(typeof(UsersDBContext))]
    partial class UsersDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Infrastructure.DataBase.Entities.App", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AppTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AppTypeId");

                    b.ToTable("Apps");

                    b.HasData(
                        new
                        {
                            Id = new Guid("6544598e-f174-41dd-a938-a0ecc5244c4d"),
                            AppTypeId = new Guid("4c175cf1-67a7-4b35-a978-9b79acf39743"),
                            Code = "Yandex"
                        });
                });

            modelBuilder.Entity("Infrastructure.DataBase.Entities.AppType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AppTypes");

                    b.HasData(
                        new
                        {
                            Id = new Guid("4c175cf1-67a7-4b35-a978-9b79acf39743"),
                            Code = "Browser"
                        });
                });

            modelBuilder.Entity("Infrastructure.DataBase.Entities.OperatingSystemType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("OperatingSystemTypes");

                    b.HasData(
                        new
                        {
                            Id = new Guid("7486becb-b36c-4e79-9b1a-a0e49240ae3c"),
                            Code = "Windows"
                        });
                });

            modelBuilder.Entity("Infrastructure.DataBase.Entities.Password", b =>
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
                            UserId = new Guid("f2408735-baf9-4b7a-b133-33050bc2e86f"),
                            Hash = "mRytDVsoZEPR+eMiMbl/xMAckvL5s+k70iboHYpSIlw="
                        });
                });

            modelBuilder.Entity("Infrastructure.DataBase.Entities.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AppId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreationDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("FinishDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("IP")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("LastUpdate")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid?>("OperatingSystemTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RefreshToken")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AppId");

                    b.HasIndex("OperatingSystemTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("Infrastructure.DataBase.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreationDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTimeOffset?>("FinishDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("LastUpdate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Patronymic")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("f2408735-baf9-4b7a-b133-33050bc2e86f"),
                            CreationDate = new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Email = "test@mail.ru",
                            FirstName = "Денис",
                            LastName = "Смирнов",
                            LastUpdate = new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Patronymic = "Алексеевич"
                        });
                });

            modelBuilder.Entity("Infrastructure.DataBase.Entities.App", b =>
                {
                    b.HasOne("Infrastructure.DataBase.Entities.AppType", "AppType")
                        .WithMany("Apps")
                        .HasForeignKey("AppTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppType");
                });

            modelBuilder.Entity("Infrastructure.DataBase.Entities.Password", b =>
                {
                    b.HasOne("Infrastructure.DataBase.Entities.User", "User")
                        .WithOne("Password")
                        .HasForeignKey("Infrastructure.DataBase.Entities.Password", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Infrastructure.DataBase.Entities.Session", b =>
                {
                    b.HasOne("Infrastructure.DataBase.Entities.App", "App")
                        .WithMany("Sessions")
                        .HasForeignKey("AppId");

                    b.HasOne("Infrastructure.DataBase.Entities.OperatingSystemType", "OperatingSystemType")
                        .WithMany("Sessions")
                        .HasForeignKey("OperatingSystemTypeId");

                    b.HasOne("Infrastructure.DataBase.Entities.User", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("App");

                    b.Navigation("OperatingSystemType");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Infrastructure.DataBase.Entities.App", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("Infrastructure.DataBase.Entities.AppType", b =>
                {
                    b.Navigation("Apps");
                });

            modelBuilder.Entity("Infrastructure.DataBase.Entities.OperatingSystemType", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("Infrastructure.DataBase.Entities.User", b =>
                {
                    b.Navigation("Password")
                        .IsRequired();

                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}
