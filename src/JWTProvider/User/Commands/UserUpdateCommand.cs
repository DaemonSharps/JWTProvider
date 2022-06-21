using Infrastructure.Extentions;
using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JWTProvider.User.Commands
{
    public class UserUpdateCommand : IRequest<Infrastructure.DataBase.User>, IValidatableObject
    {
        internal string Email { get; set; }

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
}
