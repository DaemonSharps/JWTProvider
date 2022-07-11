using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.Entities;
using JWTProvider.Models;
using JWTProvider.Token.Commands;
using JWTProvider.User.Commands;
using JWTProviderIntegration.Common;
using JwtTest.Common;

namespace JWTProviderIntegration.Extentions
{
    internal static partial class HttpClientExtentions
    {
        public static async Task<TokenModel> GetToken(this HttpClient client, User user, string password)
        {
            var command = new GetTokenCommand
            {
                Email = user.Email,
                Password = password
            };

            var result = await client.PostAsJsonAsync("/token", command);
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
            var command = new UpdateTokenCommand
            {
                RefreshToken = refreshToken
            };

            var result = await client.PutAsJsonAsync("/token", command);
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
}

