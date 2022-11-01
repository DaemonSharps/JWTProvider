using System;
using JWTProvider.Common.Exceptions;
using JWTProvider.Session.Commands;
using JWTProvider.User.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq.EntityFrameworkCore;
using DB = Infrastructure.DataBase.Entities;

namespace Handlers.Session;

public class SessionCreateHandlerTests
{
    [Fact]
    public async Task CreateNewSession()
    {
        //Arrange
        var dbContext = TestDBContext.CreateInMemoryContext();
        var dateBefore = DateTimeOffset.UtcNow;
        var handler = new SessionCreateHandler(dbContext, null);
        var command = new SessionCreateCommand
        {
            UserId = new Guid("f2408735-baf9-4b7a-b133-33050bc2e86f")
        };
        //Act
        var result = await handler.Handle(command, default);

        //Assert
        var dateAfter = DateTimeOffset.UtcNow;
        Assert.Equal(command.UserId, result.UserId);
        Assert.NotEmpty(result.RefreshToken.ToString());
        Assert.NotEmpty(result.OperatingSystemTypeId.ToString());
        Assert.NotEmpty(result.AppId.ToString());
        Assert.True(dateBefore < result.CreationDate);
        Assert.True(dateBefore < result.LastUpdate);
        Assert.True(dateAfter > result.CreationDate);
        Assert.True(dateAfter > result.LastUpdate);
        Assert.Null(result.FinishDate);
    }

    [Fact]
    public async Task SessionLimit_ThrowError()
    {
        //Arrange
        const System.Net.HttpStatusCode ExpectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        const string ExpectedErrorCode = "CREATE_SESSION_FAILED";
        const string ExpectedErrorMessage = "Max session count is limited: Max = 5, App = Yandex";
        var dbContext = TestDBContext.CreateInMemoryContext();
        var userId = new Guid("f2408735-baf9-4b7a-b133-33050bc2e86f");
        var appId = new Guid("6544598e-f174-41dd-a938-a0ecc5244c4d");
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
        var handler = new SessionCreateHandler(dbContext, null);
        var command = new SessionCreateCommand
        {
            UserId = userId
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

        var loggerMock = new Mock<ILogger<SessionCreateHandler>>();
        var handler = new SessionCreateHandler(dbContext.Object, loggerMock.Object);
        var command = new SessionCreateCommand
        {
            UserId = new Guid("f2408735-baf9-4b7a-b133-33050bc2e86f")
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

