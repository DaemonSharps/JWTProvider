using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Constants
{
    public struct RestErrorCodes
    {
        public const string InvalidRT = "INVALID_REFRESH_TOKEN";

        public const string UserNF = "USER_NOT_FOUND";

        public const string LoginFalied = "LOGIN_FAILED";
    }
}
