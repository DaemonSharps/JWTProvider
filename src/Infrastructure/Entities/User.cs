using System;
using System.ComponentModel.DataAnnotations;
namespace Infrastructure.Entities
{
    public class User
    {
        [Required, EmailAddress]
        public string EMail { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string LastName { get; set; }

        public string GetFullName()
            => string.Join(' ', FirstName, SecondName, LastName);
    }
}
