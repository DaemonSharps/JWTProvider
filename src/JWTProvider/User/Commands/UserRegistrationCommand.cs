using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace JWTProvider.User.Commands;

public class UserRegistrationCommand : UserUpdateCommand
{
    [Required]
    public string Password { get; set; }

    [EmailAddress]
    [SwaggerSchema(ReadOnly = false)]
    public override string Email { get; set; }
}
