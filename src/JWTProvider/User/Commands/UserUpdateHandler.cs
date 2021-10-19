using Infrastructure.Constants;
using Infrastructure.DataBase;
using Infrastructure.Entities;
using Infrastructure.Extentions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserDB = Infrastructure.DataBase.User;

namespace JWTProvider.User.Commands
{
    public class UserUpdateHandler : IRequestHandler<UserUpdateCommand, (UserDB model, RestApiError error)>
    {
        private readonly UsersDBContext _context;

        public UserUpdateHandler(UsersDBContext context)
        {
            _context = context;
        }

        public async Task<(UserDB model, RestApiError error)> Handle(UserUpdateCommand command, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Login)
                .Include(u => u.Password)
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

            if (user is null) return (null, new() { Code = RestErrorCodes.UserNF, Message = "User not found" });
            if (new[] { command.FirstName, command.LastName, command.MiddleName, command.Login }.IsAllNullOrEmpty()) 
                return (null, new() { Code = RestErrorCodes.NoContent });

            user.FirstName = command.FirstName ?? user.FirstName;
            user.MiddleName = command.MiddleName ?? user.MiddleName;
            user.LastName = command.LastName ?? user.LastName;
            user.Login.DisplayLogin = command.Login ?? user.Login.DisplayLogin;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                return (null, new() { Code = nameof(DbUpdateException), Message = ex.Message });
            }

            return (user, null);
        }
    }
}
