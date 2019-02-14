using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using Winleafs.Models.Enums;
using Winleafs.Models.Models.Scheduling;

namespace Winleafs.Models.Models
{
    public class UserSettings
    {
        public static readonly string APPLICATIONNAME = "Winleafs";
        public static readonly string APPLICATIONVERSION = "v0.1.2";

        public static readonly string SettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATIONNAME);
        private static readonly string SettingsFileName = Path.Combine(SettingsFolder, "Settings.txt");

        private static UserSettings _settings { get; set; }

        public static UserSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    LoadSettings();
                }

                return _settings;
            }
        }

        #region Stored properties
        public List<Device> Devices { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public int? SunriseHour { get; set; }
        public int? SunriseMinute { get; set; }
        public int? SunsetHour { get; set; }
        public int? SunsetMinute { get; set; }

        public bool StartAtWindowsStartup { get; set; }

        public int AmbilightRefreshRatePerSecond { get; set; }
        public int AmbilightMonitorIndex { get; set; }
        #endregion

        /// <summary>
        /// Used in the GUI to determine which device is currently being edited
        /// </summary>
        [JsonIgnore]
        public Device ActviceDevice
        {
            get
            {
                return Devices.FirstOrDefault(d => d.ActiveInGUI);
            }
        }

        public static void LoadSettings()
        {
            if (!HasSettings())
            {
                var userSettings = new UserSettings();

                //Set defaults here
                userSettings.Devices = new List<Device>();
                userSettings.AmbilightRefreshRatePerSecond = 1;
                userSettings.AmbilightMonitorIndex = 0;

                _settings = userSettings;
            }
            else
            {
                try
                {
                    var json = File.ReadAllText(SettingsFileName);

                    var userSettings = JsonConvert.DeserializeObject<UserSettings>(json);

                    _settings = userSettings;
                }
                catch (Exception e)
                {
                    throw new SettingsFileJsonException(e);
                }
            }
        }

        public static bool HasSettings()
        {
            return File.Exists(SettingsFileName);
        }

        public static void DeleteSettings()
        {
            if (HasSettings())
            {
                File.Delete(SettingsFileName);
                _settings = null;
            }
        }

        public void SaveSettings()
        {
            var json = JsonConvert.SerializeObject(this);

            if (!Directory.Exists(SettingsFolder))
            {
                Directory.CreateDirectory(SettingsFolder);
            }

            File.WriteAllText(SettingsFileName, json);
        }

        public void AddDevice(Device device)
        {
            Devices.Add(device);
            SaveSettings();
        }

        public void AddSchedule(Schedule schedule, bool makeActive)
        {
            var device = ActviceDevice;

            if (makeActive)
            {
                device.Schedules.ForEach(s => s.Active = false);
                schedule.Active = true;
            }

            device.Schedules.Add(schedule);
            device.Schedules = device.Schedules.OrderBy(s => s.Name).ToList();
            SaveSettings();
        }

        public void ActivateSchedule(Schedule schedule)
        {
            ActviceDevice.Schedules.ForEach(s => s.Active = false);

            schedule.Active = true;
            SaveSettings();
        }

        public void DeleteSchedule(Schedule schedule)
        {
            ActviceDevice.Schedules.Remove(schedule);
            SaveSettings();
        }

        public void UpdateSunriseSunset(int sunriseHour, int sunriseMinute, int sunsetHour, int sunsetMinute)
        {
            SunriseHour = sunriseHour;
            SunriseMinute = sunriseMinute;
            SunsetHour = sunsetHour;
            SunsetMinute = sunsetMinute;

            //Update each sunset and sunrise trigger to the new times
            foreach (var device in Devices)
            {
                foreach (var schedule in device.Schedules)
                {
                    foreach (var program in schedule.Programs)
                    {
                        foreach (var trigger in program.Triggers)
                        {
                            if (trigger.TriggerType == TriggerType.Sunrise)
                            {
                                trigger.Hours = sunriseHour;
                                trigger.Minutes = sunriseMinute;
                            }
                            else if (trigger.TriggerType == TriggerType.Sunset)
                            {
                                trigger.Hours = sunsetHour;
                                trigger.Minutes = sunsetMinute;
                            }
                        }

                        program.ReorderTriggers(); // Puts all triggers of all programs in correct order, this is needed since the times of triggers can change due to sunrise and sunset times
                    }
                }
            }

            SaveSettings();
        }

        /// <summary>
        /// Resets all operation modes of all device to Schedule, used at application startup (then users can switch to manual during runtime)
        /// </summary>
        public void ResetOperationModes()
        {
            foreach (var device in Devices)
            {
                device.OperationMode = OperationMode.Schedule;
            }
        }
    }

    public class SettingsFileJsonException : Exception
    {
        public SettingsFileJsonException(Exception e) : base("Error loading settings, corrupt JSON", e)
        {

        }
    }
}
