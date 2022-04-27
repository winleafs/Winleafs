using System;
using System.Collections.Generic;
using System.Linq;
using Winleafs.Models;
using Winleafs.Models.Enums;

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
            foreach (var orchestrator in _orchestratorForDevices.Values)
            {
                //Stop all event triggers such that all timers are disposed for correct disposal of all classes
                orchestrator.EventTriggersCollection.StopAllEvents();
            }

            _orchestratorForDevices.Clear();
            GC.Collect(); //Make sure all previous orchestrators are removed from memory

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
        /// for all devices.
        /// </summary>
        public static void ResetOrchestrators()
        {
            //Reset operation mode back to schedule if an effect or event was active
            foreach (var orchestrator in _orchestratorForDevices.Values)
            {
                orchestrator.TrySetOperationMode(OperationMode.Schedule).GetAwaiter().GetResult();
            }

            //Reset all orchestrators
            Initialize();
        }

        /// <summary>
        /// Gets the <see cref="Orchestrator"/> object for the given <paramref name="device"/>.
        /// </summary>
        /// <param name="device">The device to be looked for.</param>
        /// <returns>An <see cref="Orchestrator"/> instance</returns>
        public static Orchestrator GetOrchestratorForDevice(Device device)
        {
            if (device == null)
            {
                return null;
            }

            return _orchestratorForDevices[device.IPAddress];
        }

        /// <summary>
        /// Retrieves the number of orchestrators that have an
        /// screen mirror effect active.
        /// </summary>
        public static int CountOrchestratorsWithActiveScreenMirrorEffect()
        {
            return _orchestratorForDevices.Values.Count(orchestrator => orchestrator.HasActiveScreenMirrorEffect());
        }

        /// <summary>
        /// Deletes the <see cref="Orchestrator"/> for the given <paramref name="device"/>.
        /// </summary>
        public static void DeleteOrchestrator(Device device)
        {
            _orchestratorForDevices.Remove(device.IPAddress);
        }
    }
}
