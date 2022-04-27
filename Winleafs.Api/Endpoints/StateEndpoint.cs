using System;
using System.Threading.Tasks;
using RestSharp;
using Winleafs.Models.State;
using Winleafs.Nanoleaf.Endpoints.Interfaces;

namespace Winleafs.Nanoleaf.Endpoints
{
    public class StateEndpoint : NanoleafEndpoint, IStateEndpoint
    {
        private const string BaseUrl = "state";

        public StateEndpoint(NanoleafConnection connection) : base(connection)
        {
        }

        /// <inheritdoc />
	    public StateModel GetBrightness()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<StateModel> GetBrightnessAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public string GetColorMode()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<string> GetColorModeAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public StateModel GetColorTemperature()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<StateModel> GetColorTemperatureAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public StateModel GetHue()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<StateModel> GetHueAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public StateModel GetSaturation()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<StateModel> GetSaturationAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public OnOffModel GetState()
        {
            return GetStateAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public async Task<OnOffModel> GetStateAsync()
        {
            return await SendRequestAsync<OnOffModel>($"{BaseUrl}/on", Method.GET);
        }

        /// <inheritdoc />
        public void IncrementBrightness(int increment)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task IncrementBrightnessAsync(int increment)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task IncrementColorTemperatureAsync(int increment)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IncrementColorTemperature(int increment)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IncrementHue(int increment)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task IncrementHueAsync(int increment)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IncrementSaturation(int increment)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task IncrementSaturationAsync(int increment)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void SetBrightness(int value, int? duration = null)
        {
            SetBrightnessAsync(value, duration).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public Task SetBrightnessAsync(int value, int? duration = null)
        {
            if (duration.HasValue)
            {
                return SendRequestAsync(BaseUrl, Method.PUT, body: new { brightness = new { value, duration } });
            }
            else
            {
                return SendRequestAsync(BaseUrl, Method.PUT, body: new { brightness = new { value } });
            }
        }

        /// <inheritdoc />
        public void SetColorTemperature(int value)
        {
            SetColorTemperatureAsync(value).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public Task SetColorTemperatureAsync(int value)
        {
            return SendRequestAsync(BaseUrl, Method.PUT, body: new { ct = new { value } });
        }

        public void SetHue(int value)
        {
            SetHueAsync(value).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task SetHueAsync(int value)
        {
            return SendRequestAsync(BaseUrl, Method.PUT, body: new { hue = new { value } });
        }

        public void SetSaturation(int value)
        {
            SetSaturationAsync(value).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task SetSaturationAsync(int value)
        {
            return SendRequestAsync(BaseUrl, Method.PUT, body: new { sat = new { value } });
        }

        public void SetState(bool state)
        {
            SetStateAsync(state).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task SetStateAsync(bool state)
        {
            return SendRequestAsync(BaseUrl, Method.PUT, body: new { on = new { value = state } });
        }

        public void SetStateWithStateCheck(bool state)
        {
            SetStateWithStateCheckAsync(state).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task SetStateWithStateCheckAsync(bool state)
        {
            var currentState = (await GetStateAsync())?.IsTurnedOn;

            if (currentState != state)
            {
                await SetStateAsync(state);
            }
        }

        public void SetHueAndSaturation(int hue, int saturation)
        {
            SetHueAndSaturationAsync(hue, saturation).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task SetHueAndSaturationAsync(int hue, int saturation, bool disableLogging = false)
        {
            return SendRequestAsync(BaseUrl, Method.PUT, body: new
            {
                sat = new { value = saturation },
                hue = new { value = hue }
            }, disableLogging: disableLogging);
        }

        public void SetHueSaturationAndBrightness(int hue, int saturation, int brightness)
        {
            SetHueSaturationAndBrightnessAsync(hue, saturation, brightness).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task SetHueSaturationAndBrightnessAsync(int hue, int saturation, int brightness, bool disableLogging = false)
        {
            return SendRequestAsync(BaseUrl, Method.PUT, body: new
            {
                sat = new { value = saturation },
                hue = new { value = hue },
                brightness = new { value = brightness }
            }, disableLogging: disableLogging);
        }
    }
}
