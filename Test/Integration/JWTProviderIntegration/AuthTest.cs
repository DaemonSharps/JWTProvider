using JWTProvider;
using System.Net.Http.Json;
using JWTProvider.User.Commands;
using JWTProvider.Token.Commands;
using JWTProviderIntegration.Common;
using JWTProvider.Models;

namespace Token;

public class AuthTest : ApiTestBase
{
    public AuthTest(JWTProviderTestFixture<Startup> fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Test()
    {
        var regCommand = new UserRegistrationCommand
        {
            Email = "tes@mail.ru",
            Password = "test",
            FirstName = "1"
        };
        var result = await Client.PostAsJsonAsync("/user", regCommand);
        result.EnsureSuccessStatusCode();
        var token = await result.Content.ReadFromJsonAsync<TokenModel>();
        Assert.NotNull(token);

        var getToken = new GetTokenCommand
        {
            Email = regCommand.Email,
            Password = regCommand.Password
        };

        var res = await Client.PostAsJsonAsync("/token", getToken);
        res.EnsureSuccessStatusCode();
        var token2 = await res.Content.ReadFromJsonAsync<TokenModel>();
        Assert.NotNull(token2);
    }
}

