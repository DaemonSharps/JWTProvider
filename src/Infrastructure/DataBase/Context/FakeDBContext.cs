using System;
using System.Collections.Generic;

namespace Infrastructure.DataBase
{
    public static class FakeDBContext
    {
        private static readonly User _user = new()
        {
            Id = Guid.NewGuid(),
            EMail = "test@mail.ru"
        };

        public static List<User> Users = new()
        {
            _user
        };

        public static List<Password> Passwords = new()
        {
            new Password
            {
                UserId = _user.Id,
                Hash = "xSN+wIT6Nj8tiI3kzqBWXf45V90="
            }
        };
    }
}
