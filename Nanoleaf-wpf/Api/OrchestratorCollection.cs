using System.Collections.Generic;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Api
{
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

        public static void FireScheduleTimerForActiveDevice()
        {
            GetOrchestratorForDevice(UserSettings.Settings.ActiveDevice).ScheduleTimer.FireTimer();
        }

        public static Orchestrator GetOrchestratorForDevice(Device device)
        {
            return _orchestratorForDevices[device.IPAddress];
        }
    }
}
