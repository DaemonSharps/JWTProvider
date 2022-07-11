using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace JWTUnit
{
    public static class LoggerAssert
    {
        public static void HasLogError(IInvocation invocation, string exceptionMessage, Exception innerException = null)
        {
            Assert.Equal(LogLevel.Error, invocation.Arguments[0]);
            Assert.Contains(invocation.Arguments[2].ToString(), exceptionMessage);
            if (innerException != null)
            {
                Assert.Equal(innerException, invocation.Arguments[3]);
            }
        }
    }
}
