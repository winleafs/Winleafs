using System.Collections.Generic;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Api
{
    /// <summary>
    /// Collection class for all orchestrator of all devices
    /// </summary>
    public static class OrchestratorCollection
    {
        private static Dictionary<string, Orchestrator> _orchestratorForDevices = new Dictionary<string, Orchestrator>();

        /// <summary>
        /// Initializes the <see cref="OrchestratorCollection"/> instance so it
        /// contains the <see cref="Orchestrator"/>s for the <see cref="Device"/>s
        /// the user has.
        /// </summary>
        public static void Initialize()
        {
            _orchestratorForDevices = new Dictionary<string, Orchestrator>();

            foreach (var device in UserSettings.Settings.Devices)
            {
                AddOrchestratorForDevice(device);
            }
        }

        /// <summary>
        /// Adds an <see cref="Orchestrator"/> instance to the collection for
        /// the provided <paramref name="device"/>.
        /// </summary>
        /// <param name="device">The device to be used.</param>
        public static void AddOrchestratorForDevice(Device device)
        {
            _orchestratorForDevices.Add(device.IPAddress, new Orchestrator(device));
        }

        /// <summary>
        /// Resets the <see cref="Orchestrator"/> to the <see cref="OperationMode.Schedule"/>
        /// for the currently active device.
        /// </summary>
        public static void ResetOrchestratorForActiveDevice()
        {
            //Reset operation mode back to schedule if an effect or event was active
            _orchestratorForDevices[UserSettings.Settings.ActiveDevice.IPAddress].TrySetOperationMode(OperationMode.Schedule).GetAwaiter().GetResult(); 
            _orchestratorForDevices[UserSettings.Settings.ActiveDevice.IPAddress] = new Orchestrator(UserSettings.Settings.ActiveDevice);
        }

        /// <summary>
        /// Gets the <see cref="Orchestrator"/> object for the given <paramref name="device"/>.
        /// </summary>
        /// <param name="device">The device to be looked for.</param>
        /// <returns>An <see cref="Orchestrator"/> instance</returns>
        public static Orchestrator GetOrchestratorForDevice(Device device)
        {
            return _orchestratorForDevices[device.IPAddress];
        }
    }
}
