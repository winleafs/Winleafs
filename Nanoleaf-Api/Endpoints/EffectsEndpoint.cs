using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Nanoleaf_Api.Endpoints.Interfaces;
using Nanoleaf_Models.Models.Effects;
using RestSharp;

namespace Nanoleaf_Api.Endpoints
{
    public class EffectsEndpoint : NanoleafEndpoint, IEffectsEndpoint
    {
        public EffectsEndpoint(NanoleafClient client)
        {
            Client = client;
        }

        public async Task<IEnumerable<string>> GetEffectsListAsync()
        {
            return await SendRequest<List<string>>("/effects/effectsList", Method.GET);
        }

        public Task<string> GetSelectedEffectAsync()
        {
            throw new NotImplementedException();
        }

        public Task SetSelectedEffectAsync(string effectName)
        {
            return SendRequest("effects", Method.PUT, body: "{\"select\": \"" + effectName + "\"}");
        }

        public Task<Effect> GetEffectDetailsAsync(string effectName)
        {
            throw new NotImplementedException();
        }
    }
}
