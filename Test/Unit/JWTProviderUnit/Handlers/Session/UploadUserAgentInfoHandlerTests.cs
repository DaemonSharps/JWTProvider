using System;
using System.Collections;
using JWTProvider.Common.Exceptions;
using JWTProvider.Session.Commands;
using JWTProvider.User.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq.EntityFrameworkCore;
using MyCSharp.HttpUserAgentParser.AspNetCore;
using MyCSharp.HttpUserAgentParser.Providers;
using static System.Collections.Specialized.BitVector32;
using DB = Infrastructure.DataBase.Entities;

namespace Handlers.Session;

public class UploadUserAgentInfoHandlerTests
{
    public class UserAgentTestParams : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0",
                "Firefox",
                "Browser",
                "Windows",
                "123.0.0.132"
            };

            yield return new object[]
            {
                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36",
                "Chrome",
                "Browser",
                "Linux",
                "0.0.0.0"
            };

            yield return new object[]
            {
                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36 OPR/38.0.2220.41",
                "Opera",
                "Browser",
                "Linux",
                "1.1.1.1"
            };

            yield return new object[]
            {
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36 Edg/91.0.864.59",
                "Edge",
                "Browser",
                "Windows",
                "111.111.111.111"
            };

            yield return new object[]
            {
                "Mozilla/5.0 (iPhone; CPU iPhone OS 13_5_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.1.1 Mobile/15E148 Safari/604.1",
                "Safari",
                "Browser",
                "IOS",
                "111.255.23.111"
            };

            yield return new object[]
            {
                "Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0)",
                "Internet Explorer",
                "Browser",
                "Windows",
                "222.22.2.222"
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(UserAgentTestParams))]
    public async Task Test(string userAgent, string expectedAppCode, string expectedAppTypeCode, string expectedOsTypeCode, string expectedIpAddress)
    {
        //Arrange
        var accessor = new HttpUserAgentParserAccessor(new HttpUserAgentParserDefaultProvider());
        var dBContext = TestDBContext.CreateInMemoryContext();
        var handler = new UploadUserAgentInfoHandler(accessor, dBContext, null);
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(expectedIpAddress);
        httpContext.Request.Headers.Add("user-agent", userAgent);
        var command = new UploadUserAgentInfoCommand
        {
            HttpContext = httpContext
        };
        //Act
        var result = await handler.Handle(command, default);

        //Assert
        Assert.Equal(expectedIpAddress, result.IpAddress);

        var app = dBContext.Apps.Single(a => a.Id == result.AppId);
        Assert.Equal(expectedAppCode, app.Code);

        var appType = dBContext.AppTypes.Single(at => at.Id == result.AppTypeId);
        Assert.Equal(expectedAppTypeCode, appType.Code);

        var osType = dBContext.OperatingSystemTypes.Single(os => os.Id == result.OperatingSystemTypeId);
        Assert.Equal(expectedOsTypeCode, osType.Code);
    }

    [Fact]
    public async void DBUpdateError_Throw()
    {
        //Arrange
        const string ExpectedErrorCode = "UPDATE_SESSION_FAILED";
        const string ExpectedErrorMessage = "DB error";
        const string DBUpdateExceptionMessage = "db error test";
        var expectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        var innerException = new DbUpdateException(DBUpdateExceptionMessage);

        var accessor = new HttpUserAgentParserAccessor(new HttpUserAgentParserDefaultProvider());
        var dbContext = TestDBContext.CreateContextMock();
        dbContext.Setup(c => c.SaveChangesAsync(default))
            .Throws(innerException);
        dbContext.Setup(c => c.OperatingSystemTypes).ReturnsDbSet(new[] { new DB.OperatingSystemType() });
        dbContext.Setup(c => c.Apps).ReturnsDbSet(new[] { new DB.App() });
        dbContext.Setup(c => c.AppTypes).ReturnsDbSet(new[] { new DB.AppType() });

        var loggerMock = new Mock<ILogger<UploadUserAgentInfoHandler>>();

        var handler = new UploadUserAgentInfoHandler(accessor, dbContext.Object, loggerMock.Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0)");

        var command = new UploadUserAgentInfoCommand
        {
            HttpContext = httpContext
        };
        //Act
        var resultTask = handler.Handle(command, default);

        //Assert
        var exception = await Assert.ThrowsAsync<UpdateSessionException>(() => resultTask);
        HttpExceptionAssert.IsValidHttpException(exception, expectedStatusCode, ExpectedErrorCode, ExpectedErrorMessage);
        var invocation = loggerMock.Invocations.Single();
        LoggerAssert.HasLogError(invocation, "Upload agent info failed. Handler request:", innerException);
    }
}

