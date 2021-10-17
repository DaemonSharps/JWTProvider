using Infrastructure.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace JWTProvider.User.Commands
{
    public class UserRegistrationCommand : IRequest<(Infrastructure.DataBase.User user, RestApiError error)>
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Login { get; set; }
    }
}
