using Infrastructure.DataBase.Entities;
using JWTProvider;
using JWTProviderIntegration.Common;
using JWTProviderIntegration.Extentions;

namespace BusinessLogicTests;

public class BusinessLogicTests : ApiTestBase
{
    public BusinessLogicTests(JWTProviderTestFixture<Startup> fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task RegistrationForNewUser()
    {
        var expectedUser = new User
        {
            Email = "t@mail.ru",
            FirstName = "fn",
            LastName = "ln",
            MiddleName = "mn"
        };
        const string ExpectedPassword = "t";
        var registrationResult = await Client.UserRegistration(expectedUser, ExpectedPassword);

        Thread.Sleep(TimeSpan.FromSeconds(3));
        var updateTokenResult = await Client.CheckRefreshToken(expectedUser, registrationResult.RefreshToken);
        Assert.NotEqual(registrationResult.AccessToken, updateTokenResult.AccessToken);
    }

    [Fact]
    public async Task LoginExistingUser()
    {
        var expectedUser = new User
        {
            Email = "test@mail.ru",
            FirstName = "Денис",
            MiddleName = "Смирнов",
            LastName = "Алексеевич"
        };

        const string ExpectedPassword = "test";
        var loginResult = await Client.GetToken(expectedUser, ExpectedPassword);

        Thread.Sleep(TimeSpan.FromSeconds(3));
        var updateTokenResult = await Client.CheckRefreshToken(expectedUser, loginResult.RefreshToken);
        Assert.NotEqual(loginResult.AccessToken, updateTokenResult.AccessToken);
    }


    [Fact]
    public async Task RegistrationForNewUserAndUpdate()
    {
        var expectedUser = new User
        {
            Email = "t2@mail.ru",
            FirstName = "fn",
            LastName = "ln",
            MiddleName = "mn"
        };
        const string ExpectedPassword = "t";
        var registrationResult = await Client.UserRegistration(expectedUser, ExpectedPassword);

        var updateTokenResult = await Client.CheckRefreshToken(expectedUser, registrationResult.RefreshToken);

        expectedUser.LastName = "different_ln";
        expectedUser.MiddleName = "different_mn";
        expectedUser.FirstName = "different_fn";
        await Client.UpdateUser(expectedUser, updateTokenResult.AccessToken);

        Thread.Sleep(TimeSpan.FromSeconds(3));
        var updateTokenResultAfterUserUpdate = await Client.CheckRefreshToken(expectedUser, updateTokenResult.RefreshToken);
        Assert.NotEqual(updateTokenResult.AccessToken, updateTokenResultAfterUserUpdate.AccessToken);
    }

    [Fact]
    public async Task LoginExistingUserAndUpdate()
    {
        var expectedUser = new User
        {
            Email = "test@mail.ru",
            FirstName = "Денис",
            MiddleName = "Смирнов",
            LastName = "Алексеевич"
        };

        const string ExpectedPassword = "test";
        var loginResult = await Client.GetToken(expectedUser, ExpectedPassword);

        var updateTokenResult = await Client.CheckRefreshToken(expectedUser, loginResult.RefreshToken);

        expectedUser.LastName = "different_ln";
        expectedUser.MiddleName = "different_mn";
        expectedUser.FirstName = "different_fn";
        await Client.UpdateUser(expectedUser, updateTokenResult.AccessToken);

        Thread.Sleep(TimeSpan.FromSeconds(3));
        var updateTokenResultAfterUserUpdate = await Client.CheckRefreshToken(expectedUser, updateTokenResult.RefreshToken);
        Assert.NotEqual(updateTokenResult.AccessToken, updateTokenResultAfterUserUpdate.AccessToken);
    }

    [Fact]
    public async Task DeleteExistingUser()
    {
        var expectedUser = new User
        {
            Email = "t1@mail.ru",
            FirstName = "t1",
            MiddleName = "t1",
            LastName = "t1"
        };
        const string ExpectedPassword = "test";

        var loginResult = await Client.UserRegistration(expectedUser, ExpectedPassword);

        await Client.DeleteUser(expectedUser.Email, loginResult.AccessToken);

        Thread.Sleep(TimeSpan.FromSeconds(3));
        var checkRefreshTokenTask = Client.CheckRefreshToken(expectedUser, loginResult.RefreshToken);
        await Assert.ThrowsAsync<HttpRequestException>(() => checkRefreshTokenTask);
    }

    [Fact]
    public async Task DeleteExistingUser_AndRegister()
    {
        var expectedUser = new User
        {
            Email = "test@mail.ru",
            FirstName = "Денис",
            MiddleName = "Смирнов",
            LastName = "Алексеевич"
        };

        const string ExpectedPassword = "test";

        var loginResult = await Client.GetToken(expectedUser, ExpectedPassword);

        await Client.DeleteUser(expectedUser.Email, loginResult.AccessToken);

        var userRegistrationResult = await Client.UserRegistration(expectedUser, ExpectedPassword);

        await Client.CheckRefreshToken(expectedUser, userRegistrationResult.RefreshToken);
    }
}

