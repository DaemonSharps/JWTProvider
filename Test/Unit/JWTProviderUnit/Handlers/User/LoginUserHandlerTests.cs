using Infrastructure.Common.Exceptions;
using JWTProvider.Common.Exceptions;
using JWTProvider.Token.Commands;
using JWTProvider.User.Queries;
using DBUser = Infrastructure.DataBase.Entities.User;

namespace Handlers.User;

public class LoginUserHandlerTests
{
    [Fact]
    public async Task SuccessLogin()
    {
        //Arrange
        var memoryCache = new MemoryCacheMock();
        var handler = new LoginUserHandler(TestDBContext.CreateInMemoryContext());
        var command = new LoginUserCommand
        {
            Email = "test@mail.ru",
            Password = "test"
        };

        //Act
        var result = await handler.Handle(command, default);
        //Assert
        Assert.NotNull(result);
        Assert.Equal(command.Email, result.Email);
        Assert.Equal("Алексеевич", result.LastName);
        Assert.Equal("Смирнов", result.MiddleName);
        Assert.Equal("Денис", result.FirstName);
        Assert.Equal(new Guid("f2408735-baf9-4b7a-b133-33050bc2e86f"), result.Id);
    }

    [Theory]
    [InlineData("tes")]
    [InlineData(null)]
    public async Task ExistingUser_WrongPassword_Throw(string password)
    {
        //Arrange
        const string ExpectedMessage = "Invalid email or password";
        const string ExpectedCode = "LOGIN_FAILED";
        const System.Net.HttpStatusCode ExpectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        var memoryCache = new MemoryCacheMock();
        var handler = new LoginUserHandler(TestDBContext.CreateInMemoryContext());
        var command = new LoginUserCommand
        {
            Email = "test@mail.ru",
            Password = password
        };

        //Act
        var result = handler.Handle(command, default);
        //Assert
        var exception = await Assert.ThrowsAsync<LoginFailedException>(() => result);
        HttpExceptionAssert.IsValidHttpException(exception, ExpectedStatusCode, ExpectedCode, ExpectedMessage);
        if (password == null)
        {
            Assert.IsType<ArgumentNullException>(exception.InnerException);
        }
    }

    [Theory]
    [InlineData("tes@mail.ru")]
    [InlineData(null)]
    public async Task UnknownUser_Throw(string email)
    {
        //Arrange
        const string ExpectedMessage = "User not found";
        const string ExpectedCode = "LOGIN_FAILED";
        const System.Net.HttpStatusCode ExpectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        var memoryCache = new MemoryCacheMock();
        var handler = new LoginUserHandler(TestDBContext.CreateInMemoryContext());
        var command = new LoginUserCommand
        {
            Email = email
        };

        //Act
        var result = handler.Handle(command, default);
        //Assert
        var exception = await Assert.ThrowsAsync<LoginFailedException>(() => result);
        HttpExceptionAssert.IsValidHttpException(exception, ExpectedStatusCode, ExpectedCode, ExpectedMessage);
    }
}

