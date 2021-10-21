﻿using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.DataBase
{
    public class UsersDBContext : DbContext
    {
        public UsersDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Password> Passwords { get; set; }

        public DbSet<Login> Logins { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            #region SeedData
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@mail.ru",
                FirstName = "Денис",
                MiddleName = "Смирнов",
                LastName = "Алексеевич",
                RoleId = 1
            };
            var pwd = new Password
            {
                UserId = user.Id,
                Hash = "I3UX9g/lL94qcF4CNNtRiGnhP0E="
            };
            var login = new Login
            {
                UserId = user.Id,
                DisplayLogin = "Test"
            };

            builder.Entity<UserRole>()
                .HasData(
                    new()
                    {
                        Id = 1,
                        Name = "Admin"
                    },
                    new()
                    {
                        Id = 2,
                        Name = "User"
                    },
                    new()
                    {
                        Id = 3,
                        Name = "App"
                    });
            builder.Entity<User>().HasData(user);
            builder.Entity<Login>().HasData(login);
            builder.Entity<Password>().HasData(pwd);
            #endregion
        }
    }
}
