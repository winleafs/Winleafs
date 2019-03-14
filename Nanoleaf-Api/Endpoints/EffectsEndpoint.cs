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
	    /// <inheritdoc />
	    public EffectsEndpoint(NanoleafClient client)
        {
            Client = client;
        }

	    /// <inheritdoc />
	    public async Task<IEnumerable<string>> GetEffectsListAsync()
        {
            return await SendRequestAsync<List<string>>("/effects/effectsList", Method.GET).ConfigureAwait(false);
        }

	    /// <inheritdoc />
	    public IEnumerable<string> GetEffectsList()
		{
			return GetEffectsListAsync().ConfigureAwait(false).GetAwaiter().GetResult();
		}

	    /// <inheritdoc />
	    public Task<string> GetSelectedEffectAsync()
        {
            throw new NotImplementedException();
        }

	    /// <inheritdoc />
	    public string GetSelectedEffect()
		{
			return GetSelectedEffectAsync().ConfigureAwait(false).GetAwaiter().GetResult();
		}

	    /// <inheritdoc />
	    public Task SetSelectedEffectAsync(string effectName)
        {
            return SendRequestAsync("effects", Method.PUT, body: "{\"select\": \"" + effectName + "\"}");
        }

	    /// <inheritdoc />
	    public void SetSelectedEffect(string effectName)
		{
			SetSelectedEffectAsync(effectName).ConfigureAwait(false).GetAwaiter().GetResult();
		}


	    /// <inheritdoc />
	    public Task<Effect> GetEffectDetailsAsync(string effectName)
        {
            throw new NotImplementedException();
        }

	    /// <inheritdoc />
	    public Effect GetEffectDetails(string effectName)
		{
			throw new NotImplementedException();
		}
	}
}
