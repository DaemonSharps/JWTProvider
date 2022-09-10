using System.Threading;
using System.Threading.Tasks;
using Infrastructure.DataBase;
using Infrastructure.DataBase.Context;
using Infrastructure.Extentions;
using JWTProvider.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DB = Infrastructure.DataBase.Entities;

namespace JWTProvider.User.Commands;

public class UserRegistrationHandler : IRequestHandler<UserRegistrationCommand, DB.User>
{
    private readonly UsersDBContext _context;
    private readonly ILogger<UserRegistrationHandler> _logger;

    public UserRegistrationHandler(UsersDBContext context, ILogger<UserRegistrationHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DB.User> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user != null) throw new UserExistsException();

        var newUser = new DB.User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName
        };

        try
        {
            _context.Add(newUser);
            var password = new DB.Password
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
            throw new UserRegistrationException("DB error", ex);
        }

        return newUser;

    }
}
