using Infrastructure.Common;
using Infrastructure.DataBase;

namespace Infrastructure.Extentions
{
    public static class UserExtentions
    {
        public static string HashPassword(this User user, string password)
            => new StringHasher(password)
            .Hash(user.Email, user.Id.ToString());
    }
}
