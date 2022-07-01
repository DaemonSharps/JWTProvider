using Infrastructure.Middleware;
using Microsoft.Extensions.Options;

namespace Mocks;

internal class TestTokenOptions : IOptions<TokenOptions>
{
    public const string TestAccesKey = "test_options_access_key_11111111111111111100000000";
    public const string TestIssuer = "test_issuer";

    public TokenOptions Value => new TokenOptions
    {
        AccessKey = TestAccesKey,
        Issuer = TestIssuer
    };
}

