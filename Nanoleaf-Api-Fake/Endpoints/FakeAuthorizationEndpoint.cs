using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Nanoleaf_Api.Endpoints.Interfaces;

namespace Nanoleaf_Api_Fake.Endpoints
{
    public class FakeAuthorizationEndpoint : IAuthorizationEndpoint
    {
        private readonly Faker _faker = new Faker();

        public Task<string> GetAuthToken()
        {
            return Task.FromResult(_faker.Random.String(10, 20));
        }
    }
}
