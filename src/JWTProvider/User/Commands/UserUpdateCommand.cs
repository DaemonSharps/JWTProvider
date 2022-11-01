using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Extentions;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace JWTProvider.User.Commands;

public class UserUpdateCommand : IRequest<Infrastructure.DataBase.Entities.User>, IValidatableObject
{
    [EmailAddress]
    [SwaggerSchema(ReadOnly = true)]
    public virtual string Email { get; set; }

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var fieldList = new[] { FirstName, MiddleName, LastName };
        if (fieldList.IsAllNullOrEmpty())
        {
            yield return new ValidationResult("Оne of the fields should not be empty.",
                new[] { nameof(FirstName), nameof(MiddleName), nameof(LastName) });
        }
    }
}
