﻿// <auto-generated />
using System;
using Infrastructure.DataBase.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace JWTProvider.Migrations
{
    [DbContext(typeof(UsersDBContext))]
    [Migration("20221112133856_postgres_update")]
    partial class postgres_update
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Infrastructure.DataBase.Entities.App", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AppTypeId")
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AppTypeId");

                    b.ToTable("Apps");
                });

            modelBuilder.Entity("Infrastructure.DataBase.Entities.AppType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("AppTypes");
                });

            modelBuilder.Entity("Infrastructure.DataBase.Entities.OperatingSystemType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("OperatingSystemTypes");
                });

            modelBuilder.Entity("Infrastructure.DataBase.Entities.Password", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("Passwords");
                });

            modelBuilder.Entity("Infrastructure.DataBase.Entities.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AppId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("FinishDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("IP")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("LastUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("OperatingSystemTypeId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RefreshToken")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

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
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("FinishDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("LastUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Patronymic")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
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