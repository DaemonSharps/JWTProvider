using System;
using Infrastructure.DataBase.Context;
using JWTProvider.Common.Exceptions;
using JWTProvider.User.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Logging;
using Moq.EntityFrameworkCore;
using DB = Infrastructure.DataBase.Entities;

namespace Handlers.User;

public class UserDeleteHandlerTests
{
    [Fact]
    public async Task CloseExistingUser()
    {
        //Arrange
        var dbContext = TestDBContext.CreateInMemoryContext();
        var existingSession = new DB.Session
        {
            UserId = UsersDBContext.UserId,
            RefreshToken = Guid.NewGuid(),
            CreationDate = DateTimeOffset.UtcNow,
            LastUpdate = DateTimeOffset.UtcNow,
            FinishDate = DateTimeOffset.UtcNow.AddDays(10)
        };
        dbContext.Sessions.Add(existingSession);
        dbContext.SaveChanges();
        var handler = new UserDeleteHandler(dbContext, null);
        var command = new UserDeleteCommand
        {
            Email = "test@mail.ru"
        };

        var dateBefore = DateTimeOffset.UtcNow;
        //Act
        var result = await handler.Handle(command, default);

        //Assert
        var dateAfter = DateTimeOffset.UtcNow;
        Assert.Null(result.Password);
        Assert.True(result.FinishDate > dateBefore);
        Assert.True(result.FinishDate < dateAfter);
        Assert.Single(result.Sessions);
        foreach (var session in result.Sessions)
        {
            Assert.True(session.FinishDate > dateBefore);
            Assert.True(session.FinishDate < dateAfter);
        }
    }

    [Fact]
    public async Task UserNotFound_ThrowError()
    {
        //Arrange
        const string ExpectedMessage = "User not found";
        const string ExpectedCode = "USER_NOT_FOUND";
        const System.Net.HttpStatusCode ExpectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        var dbContext = TestDBContext.CreateInMemoryContext();

        var handler = new UserDeleteHandler(dbContext, null);
        var command = new UserDeleteCommand
        {
            Email = "1@1.ru"
        };

        //Act
        var result = handler.Handle(command, default);

        //Assert
        var exception = await Assert.ThrowsAsync<UserNotFoundException>(() => result);
        HttpExceptionAssert.IsValidHttpException(exception, ExpectedStatusCode, ExpectedCode, ExpectedMessage);
    }

    [Fact]
    public async Task UserClosed_ThrowError()
    {
        //Arrange
        const string ExpectedMessage = "User not found";
        const string ExpectedCode = "USER_NOT_FOUND";
        const System.Net.HttpStatusCode ExpectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        const string Email = "test@mail.ru";

        var dbContext = TestDBContext.CreateInMemoryContext();
        var user = dbContext.Users.First(u => u.Email == Email);
        user.FinishDate = DateTimeOffset.UtcNow.AddDays(-10);
        dbContext.SaveChanges();

        var handler = new UserDeleteHandler(dbContext, null);
        var command = new UserDeleteCommand
        {
            Email = Email
        };

        //Act
        var result = handler.Handle(command, default);

        //Assert
        var exception = await Assert.ThrowsAsync<UserNotFoundException>(() => result);
        HttpExceptionAssert.IsValidHttpException(exception, ExpectedStatusCode, ExpectedCode, ExpectedMessage);
    }

    [Fact]
    public async Task DbUpdateError_Throw()
    {
        //Arrange
        const string ExpectedErrorCode = "USER_UPDATE_FAILED";
        const string ExpectedErrorMessage = "DB error";
        const string DBUpdateExceptionMessage = "db exception";
        const System.Net.HttpStatusCode ExpectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        const string Email = "test@mail.ru";

        var dbContext = TestDBContext.CreateContextMock();
        var innerException = new DbUpdateException(DBUpdateExceptionMessage);
        dbContext.Setup(c => c.SaveChangesAsync(default))
            .Throws(innerException);
        var userId = Guid.NewGuid();
        var sessions = new List<DB.Session> { new DB.Session { Id = userId } };
        dbContext.Setup(c => c.Users).ReturnsDbSet(new[] { new DB.User { Id = userId, Email = Email, Sessions = sessions } });
        dbContext.Setup(c => c.Sessions).ReturnsDbSet(sessions);
        dbContext.Setup(c => c.Passwords).ReturnsDbSet(new[] { new DB.Password() });

        var loggerMock = new Mock<ILogger<UserDeleteHandler>>();
        var handler = new UserDeleteHandler(dbContext.Object, loggerMock.Object);
        var command = new UserDeleteCommand
        {
            Email = Email
        };
        //Act
        var result = handler.Handle(command, default);
        //Assert
        var exception = await Assert.ThrowsAsync<UserUpdateException>(() => result);
        HttpExceptionAssert.IsValidHttpException(exception, ExpectedStatusCode, ExpectedErrorCode, ExpectedErrorMessage);

        var invocation = loggerMock.Invocations.Single();
        LoggerAssert.HasLogError(invocation, $"Close user failed. Handler request:", innerException);
    }
}