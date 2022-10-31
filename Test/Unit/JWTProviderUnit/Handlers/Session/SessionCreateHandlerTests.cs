using System;
using JWTProvider.Session.Commands;

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
}

