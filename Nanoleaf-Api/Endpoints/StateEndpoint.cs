using System;
using System.Threading.Tasks;

using RestSharp;

using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Models.Models.State;

namespace Winleafs.Api.Endpoints
{
    public class StateEndpoint : NanoleafEndpoint, IStateEndpoint
    {
        public StateEndpoint(NanoleafClient client)
        {
            Client = client;
        }

		public StateModel GetBrightness()
		{
			throw new NotImplementedException();
		}

		public Task<StateModel> GetBrightnessAsync()
        {
            throw new NotImplementedException();
        }

		public string GetColorMode()
		{
			throw new NotImplementedException();
		}

		public Task<string> GetColorModeAsync()
        {
            throw new NotImplementedException();
        }

		public StateModel GetcolorTemperature()
		{
			throw new NotImplementedException();
		}

		public Task<StateModel> GetColorTemperatureAsync()
        {
            throw new NotImplementedException();
        }

		public StateModel GetHue()
		{
			throw new NotImplementedException();
		}

		public Task<StateModel> GetHueAsync()
        {
            throw new NotImplementedException();
        }

		public StateModel GetSaturation()
		{
			throw new NotImplementedException();
		}

		public Task<StateModel> GetSaturationAsync()
        {
            throw new NotImplementedException();
        }

		public OnOffModel GetState()
		{
			return GetStateAsync().ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public async Task<OnOffModel> GetStateAsync()
        {
            return await SendRequest<OnOffModel>("/state/on", Method.GET);
        }

		public void IncrementBrightness(int increment)
		{
			throw new NotImplementedException();
		}

		public Task IncrementBrightnessAsync(int increment)
        {
            throw new NotImplementedException();
        }

        public Task IncrementColorTemperatureAsync(int increment)
        {
            throw new NotImplementedException();
        }

		public void IncrementColorTemprature(int increment)
		{
			throw new NotImplementedException();
		}

		public void IncrementHue(int increment)
		{
			throw new NotImplementedException();
		}

		public Task IncrementHueAsync(int increment)
        {
            throw new NotImplementedException();
        }

		public void IncrementSaturation(int increment)
		{
			throw new NotImplementedException();
		}

		public Task IncrementSaturationAsync(int increment)
        {
            throw new NotImplementedException();
        }

		public void SetBrightness(int value, int? duration = null)
		{
			SetBrightnessAsync(value, duration).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public Task SetBrightnessAsync(int value, int? duration = null)
        {
            if (duration.HasValue)
            {
                return SendRequest("state", Method.PUT, body: "{\"brightness\": {\"value\":" + value.ToString() + ", \"duration\":" + duration.Value.ToString() + "}}");
            }
            else
            {
                return SendRequest("state", Method.PUT, body: "{\"brightness\": {\"value\":" + value.ToString() + "}}");
            }
        }

		public void SetColorTemperature(int value)
		{
			throw new NotImplementedException();
		}

		public Task SetColorTemperatureAsync(int value)
        {
            return SendRequest("state", Method.PUT, body: "{\"ct\": {\"value\":" + value.ToString() + "}}");
        }

		public void SetHue(int value)
		{
			throw new NotImplementedException();
		}

		public Task SetHueAsync(int value)
        {
            return SendRequest("state", Method.PUT, body: "{\"hue\": {\"value\":" + value.ToString() + "}}");
        }

		public void SetSaturation(int value)
		{
			throw new NotImplementedException();
		}

		public Task SetSaturationAsync(int value)
        {
            return SendRequest("state", Method.PUT, body: "{\"sat\": {\"value\":" + value.ToString() + "}}");
        }

		public void SetState(bool state)
		{
			SetStateAsync(state).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public Task SetStateAsync(bool state)
        {
            return SendRequest("state", Method.PUT, body: "{\"on\": {\"value\":" + state.ToString().ToLower() + "}}");
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
    }
}
