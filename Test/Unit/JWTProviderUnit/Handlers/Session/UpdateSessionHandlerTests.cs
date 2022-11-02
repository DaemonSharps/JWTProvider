using System;
using Infrastructure.DataBase.Context;
using JWTProvider.Common.Exceptions;
using JWTProvider.Session.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq.EntityFrameworkCore;
using DB = Infrastructure.DataBase.Entities;

namespace Handlers.Session;

public class UpdateSessionHandlerTests
{
    [Fact]
    public async Task UpdateExistingSession()
    {
        //Arrange
        var dbContext = TestDBContext.CreateInMemoryContext();
        var expectedRefreshToken = Guid.NewGuid();
        var existingSessionFinishDate = DateTimeOffset.UtcNow.AddMinutes(5);
        var existingSession = new DB.Session
        {
            RefreshToken = expectedRefreshToken,
            UserId = UsersDBContext.UserId,
            FinishDate = existingSessionFinishDate,
            CreationDate = DateTimeOffset.UtcNow,
            LastUpdate = DateTimeOffset.UtcNow
        };
        dbContext.Sessions.Add(existingSession);
        dbContext.SaveChanges();
        var handler = new UpdateSessionHandler(dbContext, null);
        var command = new UpdateSessionCommand
        {
            RefreshToken = expectedRefreshToken
        };
        var dateBefore = DateTimeOffset.UtcNow;

        //Act
        var result = await handler.Handle(command, default);

        //Assert
        var dateAfter = DateTimeOffset.UtcNow;
        Assert.Equal(existingSession.UserId, result.UserId);
        Assert.NotEqual(expectedRefreshToken, result.RefreshToken);
        Assert.True(existingSessionFinishDate < result.FinishDate);
        Assert.True(result.LastUpdate < result.FinishDate);
        Assert.True(result.LastUpdate > dateBefore);
        Assert.True(result.LastUpdate < dateAfter);
    }

    [Fact]
    public async Task SessionExpired_ThrowError()
    {
        //Arrange
        const System.Net.HttpStatusCode ExpectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        const string ExpectedErrorCode = "UPDATE_SESSION_FAILED";
        const string ExpectedErrorMessage = "Session not found";
        var dbContext = TestDBContext.CreateInMemoryContext();
        var expectedRefreshToken = Guid.NewGuid();
        var existingSessionFinishDate = DateTimeOffset.UtcNow.AddMinutes(-5);
        var existingSession = new DB.Session
        {
            RefreshToken = expectedRefreshToken,
            UserId = UsersDBContext.UserId,
            FinishDate = existingSessionFinishDate,
            CreationDate = DateTimeOffset.UtcNow,
            LastUpdate = DateTimeOffset.UtcNow
        };
        dbContext.Sessions.Add(existingSession);
        dbContext.SaveChanges();
        var handler = new UpdateSessionHandler(dbContext, null);
        var command = new UpdateSessionCommand
        {
            RefreshToken = expectedRefreshToken
        };

        //Act
        var resultTask = handler.Handle(command, default);

        //Assert
        var exception = await Assert.ThrowsAsync<UpdateSessionException>(() => resultTask);
        HttpExceptionAssert.IsValidHttpException(exception, ExpectedStatusCode, ExpectedErrorCode, ExpectedErrorMessage);
    }

    [Fact]
    public async Task SessionNotFound_ThrowError()
    {
        //Arrange
        const System.Net.HttpStatusCode ExpectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        const string ExpectedErrorCode = "UPDATE_SESSION_FAILED";
        const string ExpectedErrorMessage = "Session not found";

        var dbContext = TestDBContext.CreateInMemoryContext();
        var expectedRefreshToken = Guid.NewGuid();

        var handler = new UpdateSessionHandler(dbContext, null);
        var command = new UpdateSessionCommand
        {
            RefreshToken = expectedRefreshToken
        };

        //Act
        var resultTask = handler.Handle(command, default);

        //Assert
        var exception = await Assert.ThrowsAsync<UpdateSessionException>(() => resultTask);
        HttpExceptionAssert.IsValidHttpException(exception, ExpectedStatusCode, ExpectedErrorCode, ExpectedErrorMessage);
    }

    [Fact]
    public async Task DbUpdateError_Throw()
    {
        //Arrange
        const System.Net.HttpStatusCode ExpectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        const string ExpectedErrorCode = "UPDATE_SESSION_FAILED";
        const string ExpectedErrorMessage = "DB error";
        const string DBUpdateExceptionMessage = "db exception";
        var expectedRefreshToken = Guid.NewGuid();

        var innerException = new DbUpdateException(DBUpdateExceptionMessage);

        var dbContext = TestDBContext.CreateContextMock();
        dbContext.Setup(c => c.SaveChangesAsync(default))
            .Throws(innerException);
        dbContext.Setup(d => d.Users).ReturnsDbSet(new[] { new DB.User { } });
        dbContext.Setup(d => d.Sessions).ReturnsDbSet(new[] { new DB.Session { RefreshToken = expectedRefreshToken } });

        var loggerMock = new Mock<ILogger<UpdateSessionHandler>>();
        var handler = new UpdateSessionHandler(dbContext.Object, loggerMock.Object);
        var command = new UpdateSessionCommand
        {
            RefreshToken = expectedRefreshToken
        };
        //Act
        var result = handler.Handle(command, default);

        //Assert
        var exception = await Assert.ThrowsAsync<UpdateSessionException>(() => result);
        HttpExceptionAssert.IsValidHttpException(exception, ExpectedStatusCode, ExpectedErrorCode, ExpectedErrorMessage);

        var invocation = loggerMock.Invocations.Single();
        LoggerAssert.HasLogError(invocation, "Update session failed. Handler request:", innerException);
    }
}

