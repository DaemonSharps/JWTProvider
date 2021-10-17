using Infrastructure.Entities;
using Infrastructure.Common;

namespace Infrastructure.Extentions
{
    public static class UserExtentions
    {
        public static string HashPassword(this User user, string password)
        {
            var hasher = new StringHasher(password);

            return hasher.Hash(user.Email, user.FirstName);
        }
    }
}
