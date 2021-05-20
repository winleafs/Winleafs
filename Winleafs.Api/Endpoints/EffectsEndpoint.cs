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
        private const string BaseUrl = "effects";

        /// <inheritdoc />
        public EffectsEndpoint(NanoleafClient client)
        {
            Client = client;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetEffectsListAsync()
        {
            return await SendRequestAsync<List<string>>($"{BaseUrl}/effectsList", Method.GET).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetEffectsList()
        {
            return SendRequest<List<string>>($"{BaseUrl}/effectsList", Method.GET);
        }

        /// <inheritdoc />
        public Task<string> GetSelectedEffectAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public string GetSelectedEffect()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task SetSelectedEffectAsync(string effectName)
        {
            return SendRequestAsync(BaseUrl, Method.PUT, body: new { select = effectName });
        }

        /// <inheritdoc />
        public void SetSelectedEffect(string effectName)
        {
            SendRequest(BaseUrl, Method.PUT, body: new { select = effectName});
        }


        /// <inheritdoc />
        public Task<Effect> GetEffectDetailsAsync(string effectName)
        {
            if (string.IsNullOrEmpty(effectName))
            {
                return Task.FromResult((Effect)null);
            }

            return SendRequestAsync<Effect>(BaseUrl, Method.PUT, CreateWriteEffectCommand(effectName));
        }

        /// <inheritdoc />
        public Effect GetEffectDetails(string effectName)
        {
            if (string.IsNullOrEmpty(effectName))
            {
                return null;
            }

            return SendRequest<Effect>(BaseUrl, Method.PUT, CreateWriteEffectCommand(effectName));
        }

        private static object CreateWriteEffectCommand(string effectName)
        {
            return new
            {
                write = new
                {
                    command = "request",
                    animName = effectName
                }
            };
        }
    }
}
