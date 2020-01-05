using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using RestSharp;

using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Models.Models.ExternalControl;

namespace Winleafs.Api.Endpoints
{
    public class ExternalControlEndpoint : NanoleafEndpoint, IExternalControlEndpoint
    {
        private const string BaseUrl = "effects";
        private ExternalControlInfo _externalControlInfo;

        private static readonly byte _zeroAsByte = Convert.ToByte(0);
        private static readonly byte _oneAsByte = Convert.ToByte(1);
        private static readonly string _canvasStreamIPProtocol = "udp";
        private static readonly int _canvasStreamPort = 60222;

        /// <inheritdoc />
        public ExternalControlEndpoint(NanoleafClient client)
        {
            Client = client;
        }

        /// <inheritdoc />
        public async Task<ExternalControlInfo> GetExternalControlInfoAsync(DeviceType deviceType)
        {
            //For Aurora, we will use external control version 1 by passing no version
            //For Canvas, we will use external control version 2 by passing that version
            switch (deviceType)
            {
                case DeviceType.Aurora:
                    return await SendRequestAsync<ExternalControlInfo>(BaseUrl, Method.PUT, body: "{\"write\": {\"command\": \"display\", \"animType\": \"extControl\"}}");
                case DeviceType.Canvas:
                    return await SendRequestAsync<ExternalControlInfo>(BaseUrl, Method.PUT, body: "{\"write\": {\"command\": \"display\", \"animType\": \"extControl\", \"extControlVersion\": \"v2\"}}");
                default:
                    throw new NotImplementedException($"No external control info implemented for device type {deviceType.ToString()}");
            }            
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
                    await GetExternalControlInfoAsync(deviceType);

                    _externalControlInfo = new ExternalControlInfo
                    {
                        StreamIPAddress = deviceIPAddress,
                        StreamIProtocol = _canvasStreamIPProtocol,
                        StreamPort = _canvasStreamPort
                    };
                    break;
                default:
                    throw new NotImplementedException($"No external control preparation implemented for device type {deviceType.ToString()}");
            }
            
        }

        /// <inheritdoc />
        public void SetPanelColor(DeviceType deviceType, int panelId, int red, int green, int blue)
        {
            switch (deviceType)
            {
                case DeviceType.Aurora:
                    //Send command according to external control v1 specification
                    SendUDPCommand(
                        _oneAsByte, //Number of panels
                        Convert.ToByte(panelId),
                        _oneAsByte, //Default 1
                        Convert.ToByte(red),
                        Convert.ToByte(green),
                        Convert.ToByte(blue),
                        _zeroAsByte, //default 0
                        _oneAsByte //transitionTime (1 = 100ms)
                    );

                    break;
                case DeviceType.Canvas:
                    //Send command according to external control v2 specification
                    var panelIdBytes = BitConverter.GetBytes(panelId).Take(2); //Panel id is two bytes long

                    if (BitConverter.IsLittleEndian)
                    {
                        //The byte array must be in big-endian notation
                        panelIdBytes = panelIdBytes.Reverse();
                    }                    

                    SendUDPCommand(
                        _zeroAsByte, //Number of panels
                        _oneAsByte, //Number of panels (use 0,1 to use two bytes to set it 1 panel)
                        panelIdBytes.ElementAt(0),
                        panelIdBytes.ElementAt(1), //Panel Id as two bytes
                        Convert.ToByte(red),
                        Convert.ToByte(green),
                        Convert.ToByte(blue),
                        _zeroAsByte, //White value for a color
                        _zeroAsByte, //Transition time 
                        _oneAsByte //Transition time  (use 0,1 to use two bytes to set a transition time of 1 (1 = 100ms))
                    );

                    break;
                default:
                    throw new NotImplementedException($"No {nameof(SetPanelColor)} implemented for device type {deviceType.ToString()}");
            }   
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
    }
}
