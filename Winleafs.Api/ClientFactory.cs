using System.Collections.Generic;
using Winleafs.Api.DTOs;
using Winleafs.Api.Exceptions;
using Winleafs.Models.Models;

namespace Winleafs.Api
{
    public class ClientFactory
    {
        private readonly Dictionary<string, INanoleafClient> _clients = new Dictionary<string, INanoleafClient>();

        public static ClientFactory Instance { get; } = new ClientFactory();

        private ClientFactory()
        {
        }

        public INanoleafClient Get(Device device)
        {

            if (device == null || string.IsNullOrWhiteSpace(device.IPAddress) || device.Port == default)
            {
                throw new InvalidDeviceException("The provided device does not have a valid IP or port.");
            }

            var key = GetIdentifier(device.IPAddress, device.Port);
            if (_clients.ContainsKey(key))
            {
                return _clients[key];
            }

            var client = new NanoleafClient(new ClientDto(device));
            _clients.Add(key, client);
            return client;
        }

        public INanoleafClient Get(string ip, int port, string authenticationToken = null)
        {
            if (string.IsNullOrWhiteSpace(ip) || port == default)
            {
                throw new InvalidDeviceException("The provided device does not have a valid IP or port.");
            }

            var key = GetIdentifier(ip, port);
            if (_clients.ContainsKey(key))
            {
                return _clients[key];
            }

            var client = new NanoleafClient(new ClientDto(ip, port, authenticationToken));
            _clients.Add(key, client);
            return client;
        }

        public bool Delete(Device device)
        {
            if (device == null)
            {
                return false;
            }

            var key = GetIdentifier(device.IPAddress, device.Port);
            return _clients.Remove(key);
        }

        private static string GetIdentifier(string ip, int port)
        {
            return ip + '-' + port;
        }
    }
}
