using Infrastructure.Common;
using Infrastructure.DataBase.Entities;
using Infrastructure.Middleware;
using Infrastructure.Middleware.Options;

namespace Common;

public class JWTGeneratorTests
{
    [Theory]
    [InlineData("test@mail.ru", "firstName", "lastName", "middleName")]
    [InlineData("test@mail.ru", null, "lastName", "middleName")]
    [InlineData("test@mail.ru", "firstName", null, "middleName")]
    [InlineData("test@mail.ru", "firstName", "lastName", null)]
    [InlineData("test@mail.ru", null, null, "middleName")]
    [InlineData("test@mail.ru", null, "lastName", null)]
    [InlineData("test@mail.ru", null, null, null)]
    public void GenerateFromUser(string email, string fName, string lName, string mName)
    {
        //Arrange
        const string AccessKey = "test_access_key_numbers_0001119990019910199110110";
        const string Issuer = "tokenIssuer";
        var generator = JWTGenerator.GetGenerator(AccessKey, Issuer);
        var user = new User
        {
            Email = email,
            FirstName = fName,
            LastName = lName,
            MiddleName = mName
        };
        //Act
        generator.CreateAcessToken(user);
        var result = generator.AcessToken;

        //Assert

        Assert.NotNull(result);
        var jwtToken = JWTAssert.IsJWT(result, AccessKey, Issuer);

        JWTAssert.IsValidHeader(jwtToken);
        JWTAssert.IsValidPayload(jwtToken, user, Issuer);
    }

    [Theory]
    [InlineData(null, "test_access_key_numbers_0001119990019910199110110")]
    [InlineData("tokenIssuer", null)]
    [InlineData(null, null)]
    public void NullRequiredParameter(string issuer, string accessKey)
    {
        //Arrange
        var options = new TokenOptions
        {
            AccessKey = accessKey,
            Issuer = issuer
        };
        var callConstructor = () => JWTGenerator.GetGenerator(options);
        //Act
        //Assert
        var exception = Assert.Throws<ArgumentNullException>(callConstructor);
        var name = accessKey == null
            ? nameof(accessKey)
            : nameof(issuer);
        Assert.Contains(name, exception.Message);
    }
}
