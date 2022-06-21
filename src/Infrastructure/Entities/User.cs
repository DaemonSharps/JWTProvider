using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities
{
    public class User
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        [NotMapped]
        public string FullName
            => string.Join(' ', FirstName, MiddleName, LastName);
    }
}
