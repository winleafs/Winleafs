﻿using System;
using System.Collections.Generic;
using Winleafs.Models;
using Winleafs.Nanoleaf.Endpoints;
using Winleafs.Nanoleaf.Endpoints.Interfaces;

namespace Winleafs.Nanoleaf
{
    public interface INanoleafClient
    {
        IEffectsEndpoint EffectsEndpoint { get; }

        IAuthorizationEndpoint AuthorizationEndpoint { get; }

        IStateEndpoint StateEndpoint { get; }

        IIdentifyEndpoint IdentifyEndpoint { get; }

        ILayoutEndpoint LayoutEndpoint { get; }

        IExternalControlEndpoint ExternalControlEndpoint { get; }

    }

    public class NanoleafClient : INanoleafClient
    {
        private static readonly Dictionary<string, INanoleafClient> _clients = new Dictionary<string, INanoleafClient>();

        internal Uri BaseUri;

        internal string Token;

        /// <summary>
        /// Initializes a new instance of the <see cref="NanoleafClient"/> class.
        /// </summary>
        /// <param name="ip">The IP address at which the Nanoleaf device lives.</param>
        /// <param name="port">The port that is used to access the device.</param>
        /// <param name="token">The access token to access the device's API.</param>
        public NanoleafClient(string ip, int port, string token = null)
        {
            BaseUri = new Uri($"http://{ip}:{port}");
            Token = token;
        }

        /// <summary>
        /// Gets a <see cref="INanoleafClient"/> for the given <paramref name="device"/>.
        /// </summary>
        /// <param name="device">The device wanting the <see cref="INanoleafClient"/> for.</param>
        /// <returns>An instance of a class inheriting <see cref="INanoleafClient"/>.</returns>
        public static INanoleafClient GetClientForDevice(Device device)
        {
            if (!_clients.ContainsKey(device.IPAddress))
            {
                _clients.Add(device.IPAddress, new NanoleafClient(device.IPAddress, device.Port, device.AuthToken));
            }

            return _clients[device.IPAddress];
        }

        private IEffectsEndpoint _effectsEndpoint;

        public IEffectsEndpoint EffectsEndpoint => _effectsEndpoint ??= new EffectsEndpoint(this);

        private IAuthorizationEndpoint _authorizationEndpoint;

        public IAuthorizationEndpoint AuthorizationEndpoint => _authorizationEndpoint ??= new AuthorizationEndpoint(this);

        private IStateEndpoint _stateEndpoint;

        public IStateEndpoint StateEndpoint => _stateEndpoint ??= new StateEndpoint(this);

        private ILayoutEndpoint _layoutEndpoint;

        public ILayoutEndpoint LayoutEndpoint => _layoutEndpoint ??= new LayoutEndpoint(this);

        private IIdentifyEndpoint _identifyEndpoint;

        public IIdentifyEndpoint IdentifyEndpoint => _identifyEndpoint ??= new IdentifyEndpoint(this);

        private IExternalControlEndpoint _externalControlEndpoint;

        public IExternalControlEndpoint ExternalControlEndpoint => _externalControlEndpoint ??= new ExternalControlEndpoint(this);
    }
}
