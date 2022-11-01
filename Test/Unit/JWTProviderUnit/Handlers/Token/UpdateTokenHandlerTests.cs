using JWTProvider.Common.Exceptions;
using JWTProvider.Token.Commands;

namespace Handlers.Token;

public class UpdateTokenHandlerTests
{
    [Fact]
    public async Task TokenInCache_UserExist_Return()
    {
        //Arrange
        var expectedCacheKey = Guid.NewGuid();
        object expectedCacheValue = "test@mail.ru";
        var memoryCache = new MemoryCacheMock();
        memoryCache
            .Setup(m => m.TryGetValue(expectedCacheKey, out expectedCacheValue))
            .Returns(true);
        var handler = new UpdateTokenHandler(TestDBContext.CreateInMemoryContext(), memoryCache.Object, new TestTokenOptions());
        var command = new UpdateTokenCommand
        {
            RefreshToken = expectedCacheKey
        };
        //Act
        var result = await handler.Handle(command, default);
        //Assert
        Assert.NotNull(result);
        var key = memoryCache.Verify<Guid, object>(expectedCacheValue, TimeSpan.FromDays(7));

        Assert.NotEqual(expectedCacheKey, key);
        Assert.Equal(result.RefreshToken, key);
        var token = JWTAssert.IsJWT(result.AccessToken, TestTokenOptions.TestAccesKey, TestTokenOptions.TestIssuer);
        JWTAssert.IsValidHeader(token);
        var user = new Infrastructure.DataBase.Entities.User { Email = (string)expectedCacheValue };
        JWTAssert.IsValidPayload(token, user, TestTokenOptions.TestIssuer);
    }

    [Fact]
    public async Task UnknownToken()
    {
        //Arrange
        const System.Net.HttpStatusCode ExpectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        const string ExpectedErrorCode = "INVALID_REFRESH_TOKEN";
        const string ExpectedErrorMessage = "Invalid token";
        var expectedCacheKey = Guid.NewGuid();
        object expectedCacheValue = "test@mail.ru";
        var memoryCache = new MemoryCacheMock();
        memoryCache
            .Setup(m => m.TryGetValue(expectedCacheKey, out expectedCacheValue))
            .Returns(false);
        var handler = new UpdateTokenHandler(TestDBContext.CreateInMemoryContext(), memoryCache.Object, new TestTokenOptions());
        var command = new UpdateTokenCommand
        {
            RefreshToken = expectedCacheKey
        };
        //Act
        var result = handler.Handle(command, default);
        //Assert
        var exception = await Assert.ThrowsAsync<InvalidRefreshTokenException>(() => result);
        HttpExceptionAssert.IsValidHttpException(exception, ExpectedStatusCode, ExpectedErrorCode, ExpectedErrorMessage);
    }
}
