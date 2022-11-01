using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.DataBase.Entities;
using JWTProvider.Models;
using JWTProvider.Session.Commands;
using JWTProvider.User.Commands;
using JWTProvider.User.Queries;
using JWTProviderIntegration.Common;
using JwtTest.Common;

namespace JWTProviderIntegration.Extentions;

internal static partial class HttpClientExtentions
{
    public static async Task<TokenModel> GetToken(this HttpClient client, User user, string password)
    {
        var command = new LoginUserCommand
        {
            Email = user.Email,
            Password = password
        };

        var result = await client.PostAsJsonAsync("/sessions", command);
        result.EnsureSuccessStatusCode();

        var tokenModel = await result.Content.ReadFromJsonAsync<TokenModel>();

        Assert.NotNull(tokenModel);
        var jsonWebToken = JWTAssert.IsJWT(tokenModel.AccessToken, TestTokenOptions.AccessKey, TestTokenOptions.Issuer);
        JWTAssert.IsValidHeader(jsonWebToken);
        JWTAssert.IsValidPayload(jsonWebToken, user, TestTokenOptions.Issuer);
        Assert.NotNull(tokenModel.RefreshToken.ToString());

        return tokenModel;
    }

    public static async Task<TokenModel> CheckRefreshToken(this HttpClient client, User user, Guid refreshToken)
    {
        var command = new UpdateSessionCommand
        {
            RefreshToken = refreshToken
        };

        var result = await client.PutAsJsonAsync("/sessions", command);
        result.EnsureSuccessStatusCode();

        var tokenModel = await result.Content.ReadFromJsonAsync<TokenModel>();

        Assert.NotNull(tokenModel);
        Assert.NotEqual(refreshToken, tokenModel.RefreshToken);
        var jsonWebToken = JWTAssert.IsJWT(tokenModel.AccessToken, TestTokenOptions.AccessKey, TestTokenOptions.Issuer);
        JWTAssert.IsValidHeader(jsonWebToken);
        JWTAssert.IsValidPayload(jsonWebToken, user, TestTokenOptions.Issuer);
        Assert.NotNull(tokenModel.RefreshToken.ToString());

        return tokenModel;
    }
}

