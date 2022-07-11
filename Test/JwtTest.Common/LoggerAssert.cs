using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace JwtTest.Common;

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
