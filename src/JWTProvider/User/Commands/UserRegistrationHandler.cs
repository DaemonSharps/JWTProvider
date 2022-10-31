using System.Text.Json;
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

        if (user == null)
        {
            user = new DB.User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName
            };

            _context.Add(user);
        }
        else if (user.FinishDate != null)
        {
            user.FinishDate = null;
            user.FirstName = request.FirstName;
            user.MiddleName = request.MiddleName;
            user.LastName = request.LastName;

            _context.Update(user);
        }
        else if (user.FinishDate == null) throw new UserExistsException();


        var password = new DB.Password
        {
            Hash = user.HashPassword(request.Password),
            UserId = user.Id
        };
        _context.Add(password);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            var jsonRequest = JsonSerializer.Serialize(request);
            _logger.LogError(ex, "Register user {Email} failed. DB request: {JsonRequest}", request.Email, jsonRequest);
            throw new UserRegistrationException("DB error", ex);
        }

        return user;
    }
}
