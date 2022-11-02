using System;
using System.Net.Http.Headers;
using JWTProvider;

namespace JWTProviderIntegration.Common
{
    public class ApiTestBase : IClassFixture<JWTProviderTestFixture<Startup>>
    {
        public HttpClient Client { get; }

        public ApiTestBase(JWTProviderTestFixture<Startup> fixture)
        {
            Client = fixture.CreateClient();

            Client.DefaultRequestHeaders.Add(
                "user-agent",
                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36 OPR/38.0.2220.41");
        }
    }
}

