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

        public Task<StateModel> GetBrightness()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetColorMode()
        {
            throw new NotImplementedException();
        }

        public Task<StateModel> GetColorTemperature()
        {
            throw new NotImplementedException();
        }

        public Task<StateModel> GetHue()
        {
            throw new NotImplementedException();
        }

        public Task<StateModel> GetSaturation()
        {
            throw new NotImplementedException();
        }

        public async Task<OnOffModel> GetState()
        {
            return await SendRequest<OnOffModel>("/state/on", Method.GET);
        }

        public Task IncrementBrightness(int increment)
        {
            throw new NotImplementedException();
        }

        public Task IncrementColorTemperature(int increment)
        {
            throw new NotImplementedException();
        }

        public Task IncrementHue(int increment)
        {
            throw new NotImplementedException();
        }

        public Task IncrementSaturation(int increment)
        {
            throw new NotImplementedException();
        }

        public Task SetBrightness(int value, int? duration = null)
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

        public Task SetColorTemperature(int value)
        {
            throw new NotImplementedException();
        }

        public Task SetHue(int value)
        {
            throw new NotImplementedException();
        }

        public Task SetSaturation(int value)
        {
            throw new NotImplementedException();
        }

        public Task SetState(bool state)
        {
            return SendRequest("state", Method.PUT, body: "{\"on\": {\"value\":" + state.ToString().ToLower() + "}}");
        }

        public async Task SetStateWithStateCheck(bool state)
        {
            var currentState = (await GetState()).IsTurnedOn;

            if (currentState != state)
            {
                await SetState(state);
            }
        }
    }
}
