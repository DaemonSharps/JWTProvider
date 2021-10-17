using System.ComponentModel.DataAnnotations;
namespace Infrastructure.Entities
{
    public class User
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string GetFullName()
            => string.Join(' ', FirstName, MiddleName, LastName);
    }
}
