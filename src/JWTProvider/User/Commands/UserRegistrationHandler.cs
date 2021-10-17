﻿using Infrastructure.DataBase;
using Infrastructure.Entities;
using Infrastructure.Extentions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserDB =  Infrastructure.DataBase.User;
using Microsoft.EntityFrameworkCore;

namespace JWTProvider.User.Commands
{
    public class UserRegistrationHandler : IRequestHandler<UserRegistrationCommand, (UserModel model, RestApiError error)>
    {
        private readonly UsersDBContext _context;

        public UserRegistrationHandler(UsersDBContext context)
        {
            _context = context;
        }

        public async Task<(UserModel model, RestApiError error)> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (user != null) return (null, new() { Code = "", Message = "User is already exist"});

            var newUser = new UserDB 
            {
                Email = request.Email,
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                Login = new() { DisplayLogin = request.Login ?? "NewUser" },
                RoleId = 2
            };
            var role = await _context.UserRoles.SingleOrDefaultAsync(r => r.Id == newUser.RoleId, cancellationToken);
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
                return (null, new() { Code = nameof(DbUpdateException), Message = ex.Message });
            }

            return (new() 
            {
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                MiddleName = newUser.MiddleName,
                LastName = newUser.LastName,
                Login = newUser.Login?.DisplayLogin,
                Role = role.Name
            }, null);

        }
    }
}
