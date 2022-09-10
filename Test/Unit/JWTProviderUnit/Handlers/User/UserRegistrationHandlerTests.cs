using JWTProvider.Common.Exceptions;
using JWTProvider.User.Commands;
using DeepEqual.Syntax;
using Infrastructure.Common.Exceptions;
using Infrastructure.Extentions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq.EntityFrameworkCore;
using DB = Infrastructure.DataBase.Entities;

namespace Handlers.User;

public class UserRegistrationHandlerTests
{
    [Fact]
    public async Task UserExists_Throw()
    {
        //Arrange
        const string ExpectedErrorCode = "USER_EXISTS";
        const string ExpectedErrorMessage = "User is already exist";
        var expectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        var handler = new UserRegistrationHandler(TestDBContext.CreateInMemoryContext(), null);
        var command = new UserRegistrationCommand
        {
            Email = "test@mail.ru"
        };
        //Act
        var result = handler.Handle(command, default);
        //Assert
        var exception = await Assert.ThrowsAsync<UserExistsException>(() => result);
        HttpExceptionAssert.IsValidHttpException(exception, expectedStatusCode, ExpectedErrorCode, ExpectedErrorMessage);
    }

    [Theory]
    [InlineData("fn", "mn", "ln")]
    [InlineData(null, "mn", "ln")]
    [InlineData("fn", null, "ln")]
    [InlineData("fn", "mn", null)]
    [InlineData(null, null, null)]
    public async Task NewUser(string firstName, string middleName, string lastName)
    {
        //Arrange
        const string Email = "t@mail.ru";
        const string Password = "pw";
        var dbContext = TestDBContext.CreateInMemoryContext();
        var handler = new UserRegistrationHandler(dbContext, null);
        var command = new UserRegistrationCommand
        {
            Email = Email,
            FirstName = firstName,
            MiddleName = middleName,
            LastName = lastName,
            Password = Password
        };
        //Act
        var result = await handler.Handle(command, default);
        //Assert

        command.WithDeepEqual(result)
            .IgnoreDestinationProperty(u => u.Id)
            .IgnoreDestinationProperty(u => u.Password)
            .IgnoreDestinationProperty(u => u.FullName)
            .IgnoreDestinationProperty(u => u.Sessions)
            .IgnoreDestinationProperty(u => u.LastUpdate)
            .IgnoreDestinationProperty(u => u.CreationDate)
            .IgnoreDestinationProperty(u => u.FinishDate)
            .Assert();

        Assert.Equal(result.FullName, string.Join(' ', command.FirstName, command.MiddleName, command.LastName));
        Assert.True(dbContext.Users.Contains(result));

        var passwordDB = result.Password;
        Assert.True(dbContext.Passwords.Contains(passwordDB));
        Assert.Equal(result.Id, passwordDB.UserId);
        var expectedHash = result.HashPassword(command.Password);
        Assert.Equal(expectedHash, passwordDB.Hash);
    }

    [Fact]
    public async Task DbUpdateError_Throw()
    {
        //Arrange
        const string ExpectedErrorCode = "USER_REGISTRATION_FAILED";
        const string ExpectedErrorMessage = "DB error";
        const string DBUpdateExceptionMessage = "db exception";
        var expectedStatusCode = System.Net.HttpStatusCode.BadRequest;

        var dbContext = TestDBContext.CreateContextMock();
        var innerException = new DbUpdateException(DBUpdateExceptionMessage);
        dbContext.Setup(c => c.SaveChangesAsync(default))
            .Throws(innerException);
        dbContext.Setup(c => c.Users).ReturnsDbSet(new[] { new DB.User() });

        var loggerMock = new Mock<ILogger<UserRegistrationHandler>>();
        var handler = new UserRegistrationHandler(dbContext.Object, loggerMock.Object);
        var command = new UserRegistrationCommand
        {
            Email = "tes@mail.ru",
            Password = "t"
        };
        //Act
        var result = handler.Handle(command, default);
        //Assert
        var exception = await Assert.ThrowsAsync<UserRegistrationException>(() => result);
        HttpExceptionAssert.IsValidHttpException(exception, expectedStatusCode, ExpectedErrorCode, ExpectedErrorMessage);

        var invocation = loggerMock.Invocations.Single();
        LoggerAssert.HasLogError(invocation, $"Register user {command.Email} failed", innerException);
    }
}
