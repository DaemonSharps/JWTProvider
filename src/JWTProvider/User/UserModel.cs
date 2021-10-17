using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTProvider.User
{
    public class UserModel : Infrastructure.Entities.User
    {
        public string Login { get; set; }

        public string Role { get; set; }

    }
}
