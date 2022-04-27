using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using RestSharp;
using Winleafs.Models.Enums;
using Winleafs.Models.ExternalControl;
using Winleafs.Nanoleaf.Endpoints.Interfaces;

namespace Winleafs.Nanoleaf.Endpoints
{
    public class ExternalControlEndpoint : NanoleafEndpoint, IExternalControlEndpoint
    {
        private const string BaseUrl = "effects";
        private const string StreamProtocol = "udp";
        private const int CanvasShapesStreamPort = 60222;

        private ExternalControlInfo _externalControlInfo;
        private static readonly byte _zeroAsByte = Convert.ToByte(0);
        private static readonly byte _oneAsByte = Convert.ToByte(1);

        public ExternalControlEndpoint(NanoleafConnection connection) : base(connection)
        {
        }

        /// <inheritdoc />
        public async Task<ExternalControlInfo> GetExternalControlInfoAsync(DeviceType deviceType)
        {
            //For Aurora, we will use external control version 1 by passing no version
            //For Canvas, we will use external control version 2 by passing that version
            return (deviceType) switch
            {
                DeviceType.Aurora => await SendRequestAsync<ExternalControlInfo>(BaseUrl, Method.PUT,
                    body: new { write = new { command = "display", animType = "extControl" } }).ConfigureAwait(false),
                DeviceType.Canvas => await CreateExternalControlV2Body().ConfigureAwait(false),
                DeviceType.Shapes => await CreateExternalControlV2Body().ConfigureAwait(false),
                _ => throw new NotImplementedException($"No External Control exists for device of type {deviceType}")
            };
        }

        /// <inheritdoc />
        public async Task PrepareForExternalControl(DeviceType deviceType, string deviceIPAddress)
        {
            //Per Nanoleaf documentation:
            //For Aurora, the request will give back information about the ip address and which port to use
            //For Canvas, the request will return null, and the external control info is of a pre defined structure
            switch (deviceType)
            {
                case DeviceType.Aurora:
                    _externalControlInfo = await GetExternalControlInfoAsync(deviceType);
                    break;
                case DeviceType.Canvas:
                case DeviceType.Shapes:
                    await GetExternalControlInfoAsync(deviceType);

                    _externalControlInfo = new ExternalControlInfo
                    {
                        StreamIPAddress = deviceIPAddress,
                        StreamIProtocol = StreamProtocol,
                        StreamPort = CanvasShapesStreamPort
                    };
                    break;
                case DeviceType.Unknown:
                    break;
                default:
                    throw new NotImplementedException($"No external control preparation implemented for device type {deviceType}");
            }

        }

        /// <inheritdoc />
        public void SetPanelsColors(DeviceType deviceType, List<int> panelIds, List<Color> colors)
        {
            switch (deviceType)
            {
                case DeviceType.Aurora:
                    SetExternalV1Colors(panelIds, colors);
                    break;
                case DeviceType.Canvas:
                case DeviceType.Shapes:
                    SetExternalV2Colors(panelIds, colors);
                    break;
                case DeviceType.Unknown:
                    break;
                default:
                    throw new NotImplementedException($"No {nameof(SetPanelsColors)} implemented for device type {deviceType}");
            }
        }

        private void SetExternalV1Colors(IReadOnlyList<int> panelIds, IReadOnlyList<Color> colors)
        {
            const int bytesPerPanel = 7;
            //1 byte for the number of panels, then 7 bytes per panel
            var bytes = new byte[1 + panelIds.Count * bytesPerPanel];
            bytes[0] = Convert.ToByte(panelIds.Count);

            var byteIndex = 1;

            for (var i = 0; i < panelIds.Count; i++)
            {
                bytes[byteIndex++] = Convert.ToByte(panelIds[i]);
                bytes[byteIndex++] = _oneAsByte; //Default 1, nFrames
                bytes[byteIndex++] = colors[i].R;
                bytes[byteIndex++] = colors[i].G;
                bytes[byteIndex++] = colors[i].B;
                bytes[byteIndex++] = _zeroAsByte; //default 0, white value for a color
                bytes[byteIndex++] = _oneAsByte; //transitionTime (1 = 100ms)
            }

            SendUDPCommand(bytes);
        }

        private void SetExternalV2Colors(IReadOnlyList<int> panelIds, IReadOnlyList<Color> colors)
        {
            const int bytesPerPanel = 8;
            //2 bytes for the number of panels, then 8 bytes per panel
            var bytes = new byte[2 + panelIds.Count * bytesPerPanel];

            var numberOfPanelsBytes = GetTwoBytesFromInteger(panelIds.Count);
            bytes[0] = numberOfPanelsBytes.ElementAt(0);
            bytes[1] = numberOfPanelsBytes.ElementAt(1);

            var byteIndex = 2;

            for (var i = 0; i < panelIds.Count; i++)
            {
                var panelIdBytes = GetTwoBytesFromInteger(panelIds[i]); //Panel id is two bytes long

                bytes[byteIndex++] = panelIdBytes.ElementAt(0);
                bytes[byteIndex++] = panelIdBytes.ElementAt(1);
                bytes[byteIndex++] = colors[i].R;
                bytes[byteIndex++] = colors[i].G;
                bytes[byteIndex++] = colors[i].B;
                bytes[byteIndex++] = _zeroAsByte; //default 0, white value for a color
                bytes[byteIndex++] = _zeroAsByte; //Transition time
                bytes[byteIndex++] = _oneAsByte; //transitionTime (use 0,1 to use two bytes to set a transition time of 1 (1 = 100ms))
            }

            SendUDPCommand(bytes);
        }

        /// <summary>
        /// Sends a string via UDP Datagram to the Nanoleaf device
        /// There is no need to keep the connection alive due to datagram <see cref="SocketType.Dgram"/>
        /// </summary>
        private void SendUDPCommand(params byte[] data)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var endpoint = new IPEndPoint(IPAddress.Parse(_externalControlInfo.StreamIPAddress), _externalControlInfo.StreamPort);

            socket.SendTo(data, endpoint);
            socket.Close();
        }

        /// <summary>
        /// Converts the given <paramref name="value"/> to a byte collection of size 2.
        /// </summary>
        private static IEnumerable<byte> GetTwoBytesFromInteger(int value)
        {
            //value is two bytes long
            var bytes = BitConverter.GetBytes(value).Take(2);
            return BitConverter.IsLittleEndian ? bytes.Reverse() : bytes;
        }

        private Task<ExternalControlInfo> CreateExternalControlV2Body()
        {
            return SendRequestAsync<ExternalControlInfo>(BaseUrl, Method.PUT,
                body: new { write = new { command = "display", animType = "extControl", extControlVersion = "v2" } });
        }
    }
}
