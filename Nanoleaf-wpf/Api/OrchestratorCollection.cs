using System.Collections.Generic;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Api
{
    /// <summary>
    /// Collectino class for all orchestrator of all devices
    /// </summary>
    public static class OrchestratorCollection
    {
        private static Dictionary<string, Orchestrator> _orchestratorForDevices = new Dictionary<string, Orchestrator>();

        public static void Initialize()
        {
            _orchestratorForDevices = new Dictionary<string, Orchestrator>();

            foreach (var device in UserSettings.Settings.Devices)
            {
                AddOrchestratorForDevice(device);
            }
        }

        public static void AddOrchestratorForDevice(Device device)
        {
            _orchestratorForDevices.Add(device.IPAddress, new Orchestrator(device));
        }

        public static void ResetOrchestratorForActiveDevice()
        {
            _orchestratorForDevices[UserSettings.Settings.ActiveDevice.IPAddress].TrySetOperationMode(OperationMode.Schedule).GetAwaiter().GetResult(); //Reset operation mode back to schedule if an effect or event was active
            _orchestratorForDevices[UserSettings.Settings.ActiveDevice.IPAddress] = new Orchestrator(UserSettings.Settings.ActiveDevice);
        }

        public static Orchestrator GetOrchestratorForDevice(Device device)
        {
            return _orchestratorForDevices[device.IPAddress];
        }
    }
}
