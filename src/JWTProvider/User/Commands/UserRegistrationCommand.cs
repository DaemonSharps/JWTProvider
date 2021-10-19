using Infrastructure.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace JWTProvider.User.Commands
{
    public class UserRegistrationCommand : UserUpdateCommand
    {
        [Required]
        public string Password { get; set; }
    }
}
