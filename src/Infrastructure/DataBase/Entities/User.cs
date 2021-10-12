using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DataBase
{
    public class User : Entities.User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Password Password { get; set; }

        [Required]
        public Login Login { get; set; }

        [Required]
        public long RoleId { get; set; }

        public UserRole Role { get; set; }
    }
}
