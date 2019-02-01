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

		public IEnumerable<string> GetEffectsList()
		{
			return GetEffectsListAsync().ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public Task<string> GetSelectedEffectAsync()
        {
            throw new NotImplementedException();
        }

		public string GetSelectedEffect()
		{
			return GetSelectedEffectAsync().ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public Task SetSelectedEffectAsync(string effectName)
        {
            return SendRequest("effects", Method.PUT, body: "{\"select\": \"" + effectName + "\"}");
        }

		public void SetSelectedEffect(string effectName)
		{
			SetSelectedEffectAsync(effectName).ConfigureAwait(false).GetAwaiter().GetResult();
		}


		public Task<Effect> GetEffectDetailsAsync(string effectName)
        {
            throw new NotImplementedException();
        }

		public Effect GetEffectDetails(string effectName)
		{
			throw new NotImplementedException();
		}
	}
}
