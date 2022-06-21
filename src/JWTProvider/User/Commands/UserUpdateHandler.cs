using Infrastructure.Common.Exceptions;
using Infrastructure.Constants;
using Infrastructure.DataBase;
using Infrastructure.Entities;
using Infrastructure.Extentions;
using JWTProvider.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using UserDB = Infrastructure.DataBase.User;

namespace JWTProvider.User.Commands
{
    public class UserUpdateHandler : IRequestHandler<UserUpdateCommand, UserDB>
    {
        private readonly UsersDBContext _context;
        private readonly ILogger<UserUpdateHandler> _logger;

        public UserUpdateHandler(UsersDBContext context, ILogger<UserUpdateHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserDB> Handle(UserUpdateCommand command, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Password)
                .SingleOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

            if (user is null) throw new UserNotFoundException();

            user.FirstName = command.FirstName ?? user.FirstName;
            user.MiddleName = command.MiddleName ?? user.MiddleName;
            user.LastName = command.LastName ?? user.LastName;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Update user {command.Email} failed", command);
            }

            return user;
        }
    }
}
