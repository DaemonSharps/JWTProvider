using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DataBase
{
    public class Login
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid UserId { get; set; }

        [Required]
        public string DisplayLogin { get; set; }

        public User User { get; set; }

        public string GetFullLogin()
            => string.Join('.', UserId.ToString(), DisplayLogin);
    }
}
