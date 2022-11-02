using System;
using Infrastructure.DataBase.Context;
using Infrastructure.DataBase.Entities;
using JWTProvider.Common.Exceptions;
using JWTProvider.Session.Commands;
using JWTProvider.User.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq.EntityFrameworkCore;
using DB = Infrastructure.DataBase.Entities;

namespace Handlers.Session;

public class CreateSessionHandlerTests
{
    [Fact]
    public async Task CreateNewSession()
    {
        //Arrange
        var dbContext = TestDBContext.CreateInMemoryContext();
        var dateBefore = DateTimeOffset.UtcNow;
        var handler = new CreateSessionHandler(dbContext, null, new TestSessionOptions());
        var command = new CreateSessionCommand
        {
            UserId = UsersDBContext.UserId,
            UserAgentInfo = new UserAgentDBEntries
            {
                AppId = UsersDBContext.AppId,
                IpAddress = "0.0.0.0",
                OperatingSystemTypeId = UsersDBContext.OperationSystemTypeId
            }
        };
        //Act
        var result = await handler.Handle(command, default);

        //Assert
        var dateAfter = DateTimeOffset.UtcNow;
        Assert.Equal(command.UserId, result.UserId);
        Assert.NotEmpty(result.RefreshToken.ToString());
        Assert.Equal(command.UserAgentInfo.OperatingSystemTypeId, result.OperatingSystemTypeId);
        Assert.Equal(command.UserAgentInfo.AppId, result.AppId);
        Assert.True(dateBefore < result.CreationDate);
        Assert.True(dateBefore < result.LastUpdate);
        Assert.True(dateAfter > result.CreationDate);
        Assert.True(dateAfter > result.LastUpdate);
        Assert.NotNull(result.FinishDate);
    }

    [Fact]
    public async Task SessionLimit_ThrowError()
    {
        //Arrange
        const System.Net.HttpStatusCode ExpectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        const string ExpectedErrorCode = "CREATE_SESSION_FAILED";
        const string ExpectedErrorMessage = "Max session count is limited: Max = 5, App = Yandex";
        var dbContext = TestDBContext.CreateInMemoryContext();
        var userId = UsersDBContext.UserId;
        var appId = UsersDBContext.AppId;
        for (int i = 0; i < 5; i++)
        {
            var oldSession = new DB.Session
            {
                UserId = userId,
                AppId = appId
            };
            dbContext.Add(oldSession);
        }

        dbContext.SaveChanges();
        var dateBefore = DateTimeOffset.UtcNow;
        var handler = new CreateSessionHandler(dbContext, null, new TestSessionOptions());
        var command = new CreateSessionCommand
        {
            UserId = userId,
            UserAgentInfo = new UserAgentDBEntries
            {
                AppId = appId,
                AppTypeId = Guid.NewGuid(),
                IpAddress = "0.0.0.0",
                OperatingSystemTypeId = Guid.NewGuid()
            }
        };
        //Act
        var resultTask = handler.Handle(command, default);

        //Assert
        var exception = await Assert.ThrowsAsync<CreateSessionException>(() => resultTask);
        HttpExceptionAssert.IsValidHttpException(exception, ExpectedStatusCode, ExpectedErrorCode, ExpectedErrorMessage);
    }

    [Fact]
    public async Task DbUpdateError_Throw()
    {
        //Arrange
        const System.Net.HttpStatusCode ExpectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        const string ExpectedErrorCode = "CREATE_SESSION_FAILED";
        const string ExpectedErrorMessage = "DB error";
        const string DBUpdateExceptionMessage = "db exception";

        var innerException = new DbUpdateException(DBUpdateExceptionMessage);

        var dbContext = TestDBContext.CreateContextMock();
        dbContext.Setup(c => c.SaveChangesAsync(default))
            .Throws(innerException);
        dbContext.Setup(d => d.Apps).ReturnsDbSet(new[] { new DB.App { } });
        dbContext.Setup(d => d.OperatingSystemTypes).ReturnsDbSet(new[] { new DB.OperatingSystemType { } });
        dbContext.Setup(d => d.Sessions).ReturnsDbSet(new[] { new DB.Session { } });

        var loggerMock = new Mock<ILogger<CreateSessionHandler>>();
        var handler = new CreateSessionHandler(dbContext.Object, loggerMock.Object, new TestSessionOptions());
        var command = new CreateSessionCommand
        {
            UserId = UsersDBContext.UserId,
            UserAgentInfo = new UserAgentDBEntries
            {
                AppId = Guid.NewGuid(),
                AppTypeId = Guid.NewGuid(),
                IpAddress = "0.0.0.0",
                OperatingSystemTypeId = Guid.NewGuid()
            }
        };
        //Act
        var result = handler.Handle(command, default);
        //Assert
        var exception = await Assert.ThrowsAsync<CreateSessionException>(() => result);
        HttpExceptionAssert.IsValidHttpException(exception, ExpectedStatusCode, ExpectedErrorCode, ExpectedErrorMessage);

        var invocation = loggerMock.Invocations.Single();
        LoggerAssert.HasLogError(invocation, "Create session failed. Handler request:", innerException);
    }
}

