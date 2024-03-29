﻿using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.DataBase.Entities;
using JWTProvider.Models;
using JWTProvider.User.Commands;
using JWTProviderIntegration.Common;
using JwtTest.Common;

namespace JWTProviderIntegration.Extentions;

internal static partial class HttpClientExtentions
{
    public static async Task<TokenModel> UserRegistration(this HttpClient client, User user, string password)
    {
        var command = new UserRegistrationCommand
        {
            Email = user.Email,
            Password = password,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Patronymic = user.Patronymic
        };
        var result = await client.PostAsJsonAsync("/users", command);
        result.EnsureSuccessStatusCode();
        var tokenModel = await result.Content.ReadFromJsonAsync<TokenModel>();

        Assert.NotNull(tokenModel);
        var jsonWebToken = JWTAssert.IsJWT(tokenModel.AccessToken, TestTokenOptions.AccessKey, TestTokenOptions.Issuer);
        JWTAssert.IsValidHeader(jsonWebToken);
        JWTAssert.IsValidPayload(jsonWebToken, user, TestTokenOptions.Issuer);
        Assert.NotNull(tokenModel.RefreshToken.ToString());

        return tokenModel;
    }

    public static async Task UpdateUser(this HttpClient client, User user, string accessToken)
    {
        var command = new UserUpdateCommand
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Patronymic = user.Patronymic
        };

        using var message = new HttpRequestMessage(HttpMethod.Put, new Uri("http://localhost/users"));
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        message.Content = JsonContent.Create(command);

        var result = await client.SendAsync(message);
        result.EnsureSuccessStatusCode();
    }

    public static async Task DeleteUser(this HttpClient client, string email, string accessToken)
    {
        var command = new UserDeleteCommand
        {
            Email = email
        };

        using var message = new HttpRequestMessage(HttpMethod.Delete, new Uri("http://localhost/users"));
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        message.Content = JsonContent.Create(command);

        var result = await client.SendAsync(message);
        result.EnsureSuccessStatusCode();
    }
}

