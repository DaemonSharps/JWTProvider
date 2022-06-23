using System;

namespace Infrastructure.Constants
{
    public static class RefreshToken
    {
        public static TimeSpan ExpiresDefault => TimeSpan.FromDays(7);
    }
}
