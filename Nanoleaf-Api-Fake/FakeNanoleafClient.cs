using System;
using System.Collections.Generic;
using System.Text;
using Nanoleaf_Api;
using Nanoleaf_Api.Endpoints.Interfaces;
using Nanoleaf_Api_Fake.Endpoints;

namespace Nanoleaf_Api_Fake
{
    public class FakeNanoleafClient : INanoleafClient
    {
        public IEffectsEndpoint EffectsEndpoint => new FakeEffectsEndpoint();

        public IAuthorizationEndpoint AuthorizationEndpoint => new FakeAuthorizationEndpoint();
    }
}
