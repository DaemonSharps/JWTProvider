using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace JWTUnit;

public static class JWTAssert
{
    public static JwtSecurityToken IsJWT(string token, string accessKey, string issuer)
    {
        var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessKey));
        var validationParameters = new TokenValidationParameters
        {
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha512 },
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = issuerSigningKey
        };

        new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var securityToken);

        return Assert.IsType<JwtSecurityToken>(securityToken);
    }

    public static void IsValidHeader(JwtSecurityToken token, string alg = "HS512", string typ = "JWT")
    {
        var header = token.Header;
        Assert.NotNull(header);
        Assert.Equal(alg, header.Alg);
        Assert.Equal(typ, header.Typ);
    }

    public static void IsValidPayload(JwtSecurityToken token, User user, string issuer)
    {
        var payload = token.Payload;
        Assert.NotNull(payload);
        EqualIfNotNull(payload, issuer, "iss");
        EqualIfNotNull(payload, user.Email, JwtRegisteredClaimNames.Email);
        EqualIfNotNull(payload, user.FirstName, JwtRegisteredClaimNames.GivenName);
        EqualIfNotNull(payload, user.LastName, JwtRegisteredClaimNames.FamilyName);
        EqualIfNotNull(payload, user.MiddleName, JWTClaimKeys.MiddleName);
    }

    private static void EqualIfNotNull(JwtPayload payload, string value, string key)
    {
        if (value != null)
            Assert.Equal(value, payload[key]);
    }
}
