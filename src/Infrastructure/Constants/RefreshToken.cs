using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Constants
{
    public static class RefreshToken
    {
        public static TimeSpan ExpiresDefault => TimeSpan.FromDays(7);
    }
}
