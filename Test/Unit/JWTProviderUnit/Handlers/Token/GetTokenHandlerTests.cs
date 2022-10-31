using Infrastructure.Common.Exceptions;
using JWTProvider.Common.Exceptions;
using JWTProvider.Token.Commands;
using DBUser = Infrastructure.DataBase.Entities.User;

namespace Handlers.Token;

public class GetTokenHandlerTests
{
    [Fact]
    public async Task ExistingUser_ReturnToken()
    {
        //Arrange
        var memoryCache = new MemoryCacheMock();
        var handler = new GetTokenHandler(TestDBContext.CreateInMemoryContext(), memoryCache.Object, new TestTokenOptions());
        var command = new GetTokenCommand
        {
            Email = "test@mail.ru",
            Password = "test"
        };

        //Act
        var result = await handler.Handle(command, default);
        //Assert
        Assert.NotNull(result);
        var key = memoryCache.Verify<Guid, string>(command.Email, TimeSpan.FromDays(7));

        Assert.Equal(result.RefreshToken, key);
        var token = JWTAssert.IsJWT(result.AccessToken, TestTokenOptions.TestAccesKey, TestTokenOptions.TestIssuer);
        JWTAssert.IsValidHeader(token);
        var user = new DBUser { Email = command.Email };
        JWTAssert.IsValidPayload(token, user, TestTokenOptions.TestIssuer);
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
        var handler = new GetTokenHandler(TestDBContext.CreateInMemoryContext(), memoryCache.Object, new TestTokenOptions());
        var command = new GetTokenCommand
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
        var handler = new GetTokenHandler(TestDBContext.CreateInMemoryContext(), memoryCache.Object, new TestTokenOptions());
        var command = new GetTokenCommand
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

