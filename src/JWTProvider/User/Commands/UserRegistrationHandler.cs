using System.Threading;
using System.Threading.Tasks;
using Infrastructure.DataBase;
using Infrastructure.Extentions;
using JWTProvider.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserDB = Infrastructure.DataBase.User;

namespace JWTProvider.User.Commands
{
    public class UserRegistrationHandler : IRequestHandler<UserRegistrationCommand, UserDB>
    {
        private readonly UsersDBContext _context;
        private readonly ILogger<UserRegistrationHandler> _logger;

        public UserRegistrationHandler(UsersDBContext context, ILogger<UserRegistrationHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserDB> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user != null) throw new UserExistsException();

            var newUser = new UserDB
            {
                Email = request.Email,
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName
            };

            try
            {
                _context.Add(newUser);
                var password = new Password
                {
                    Hash = newUser.HashPassword(request.Password),
                    UserId = newUser.Id
                };
                _context.Add(password);

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Register user {request.Email} failed", request);
                throw new UserRegistrationException("DB error" ,ex);
            }

            return newUser;

        }
    }
}
