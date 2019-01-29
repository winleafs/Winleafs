using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RestSharp;

using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Models.Models.Effects;

namespace Winleafs.Api.Endpoints
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
