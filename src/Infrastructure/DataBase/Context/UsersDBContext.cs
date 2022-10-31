using System;
using System.Collections.Generic;
using Infrastructure.DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataBase.Context
{
    public class UsersDBContext : DbContext
    {
        public UsersDBContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Password> Passwords { get; set; }

        public virtual DbSet<AppType> AppTypes { get; set; }

        public virtual DbSet<App> Apps { get; set; }

        public virtual DbSet<Session> Sessions { get; set; }

        public virtual DbSet<OperatingSystemType> OperatingSystemTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            #region SeedData
#if DEBUG
            var user = new User
            {
                Id = new Guid("f2408735-baf9-4b7a-b133-33050bc2e86f"),
                Email = "test@mail.ru",
                FirstName = "Денис",
                MiddleName = "Смирнов",
                LastName = "Алексеевич",
                FinishDate = null
            };
            var pwd = new Password
            {
                UserId = user.Id,
                Hash = "mRytDVsoZEPR+eMiMbl/xMAckvL5s+k70iboHYpSIlw=" //test
            };

            var appType = new AppType
            {
                Id = new Guid("4c175cf1-67a7-4b35-a978-9b79acf39743"),
                Code = "Browser"
            };

            var app = new App
            {
                Id = new Guid("6544598e-f174-41dd-a938-a0ecc5244c4d"),
                AppTypeId = appType.Id,
                Name = "Yandex"
            };

            var operatingSystemType = new OperatingSystemType
            {
                Id = new Guid("7486becb-b36c-4e79-9b1a-a0e49240ae3c"),
                Code = "Windows"
            };

            builder.Entity<User>().HasData(user);
            builder.Entity<Password>().HasData(pwd);
            builder.Entity<AppType>().HasData(appType);
            builder.Entity<App>().HasData(app);
            builder.Entity<OperatingSystemType>().HasData(operatingSystemType);
#endif
            #endregion
        }
    }
}
