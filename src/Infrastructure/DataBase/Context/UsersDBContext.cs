using System;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataBase
{
    public class UsersDBContext : DbContext
    {
        public UsersDBContext(DbContextOptions options) : base(options)
        {
#if DEBUG
           Database.EnsureDeleted();
           Database.EnsureCreated();
#endif
        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Password> Passwords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
#if DEBUG
            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            #region SeedData
            var user = new User
            {
                Id = new Guid("f2408735-baf9-4b7a-b133-33050bc2e86f"),
                Email = "test@mail.ru",
                FirstName = "Денис",
                MiddleName = "Смирнов",
                LastName = "Алексеевич"
            };
            var pwd = new Password
            {
                UserId = user.Id,
                Hash = "mRytDVsoZEPR+eMiMbl/xMAckvL5s+k70iboHYpSIlw=" //test
            };

            builder.Entity<User>().HasData(user);
            builder.Entity<Password>().HasData(pwd);
            #endregion
#endif
        }
    }
}
