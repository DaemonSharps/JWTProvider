using System;
using JWTProvider;

namespace JWTProviderIntegration.Common
{
    public class ApiTestBase : IClassFixture<JWTProviderTestFixture<Startup>>
    {
        public HttpClient Client { get; }

        public ApiTestBase(JWTProviderTestFixture<Startup> fixture)
        {
            Client = fixture.CreateClient();
        }
    }
}

