using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DataBase
{
    public class Login
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string ChangeableLogin { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public User User { get; set; }

        public string GetFullLogin()
            => string.Join('.', Id.ToString(), ChangeableLogin);
    }
}
