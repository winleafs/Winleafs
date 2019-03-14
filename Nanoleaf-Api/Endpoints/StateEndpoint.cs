using System;
using System.Threading.Tasks;

using RestSharp;

using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Models.Models.State;

namespace Winleafs.Api.Endpoints
{
    public class StateEndpoint : NanoleafEndpoint, IStateEndpoint
    {
	    /// <inheritdoc />
	    public StateEndpoint(NanoleafClient client)
        {
            Client = client;
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
            return await SendRequestAsync<OnOffModel>("/state/on", Method.GET);
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
                return SendRequestAsync("state", Method.PUT, body: "{\"brightness\": {\"value\":" + value.ToString() + ", \"duration\":" + duration.Value.ToString() + "}}");
            }
            else
            {
                return SendRequestAsync("state", Method.PUT, body: "{\"brightness\": {\"value\":" + value.ToString() + "}}");
            }
        }

	    /// <inheritdoc />
	    public void SetColorTemperature(int value)
		{
			throw new NotImplementedException();
		}

	    /// <inheritdoc />
	    public Task SetColorTemperatureAsync(int value)
        {
            return SendRequestAsync("state", Method.PUT, body: "{\"ct\": {\"value\":" + value.ToString() + "}}");
        }

		public void SetHue(int value)
		{
			throw new NotImplementedException();
		}

		public Task SetHueAsync(int value)
        {
            return SendRequestAsync("state", Method.PUT, body: "{\"hue\": {\"value\":" + value.ToString() + "}}");
        }

		public void SetSaturation(int value)
		{
			throw new NotImplementedException();
		}

		public Task SetSaturationAsync(int value)
        {
            return SendRequestAsync("state", Method.PUT, body: "{\"sat\": {\"value\":" + value.ToString() + "}}");
        }

		public void SetState(bool state)
		{
			SetStateAsync(state).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public Task SetStateAsync(bool state)
        {
            return SendRequestAsync("state", Method.PUT, body: "{\"on\": {\"value\":" + state.ToString().ToLower() + "}}");
        }

		public void SetStateWithStateCheck(bool state)
		{
			SetStateWithStateCheckAsync(state).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public async Task SetStateWithStateCheckAsync(bool state)
        {
            var currentState = (await GetStateAsync()).IsTurnedOn;

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
            return SendRequestAsync("state", Method.PUT, body: "{\"sat\": {\"value\":" + saturation.ToString() + "}, \"hue\": {\"value\":" + hue.ToString() + "}}", disableLogging: disableLogging);
        }
    }
}
