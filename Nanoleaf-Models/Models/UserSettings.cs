using JsonMigrator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Winleafs.Models.Enums;
using Winleafs.Models.Exceptions;
using Winleafs.Models.Models.Scheduling;

namespace Winleafs.Models.Models
{
    public class UserSettings
    {
        public static readonly string APPLICATIONNAME = "Winleafs";
        public static readonly string APPLICATIONVERSION = "v0.4.1";

        public static readonly string SettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATIONNAME);

        private static readonly string _settingsFileName = Path.Combine(SettingsFolder, "Settings.txt");
        private static readonly string _latestSettingsVersion = "3";

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
        public string JsonVersion { get; set; }

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
        public bool AmbilightControlBrightness { get; set; }

        public string UserLocale { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Used in the GUI to determine which device is currently being edited
        /// </summary>
        [JsonIgnore]
        public Device ActiveDevice
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
                var userSettings = new UserSettings
                {
                    // Defaults
                    Devices = new List<Device>(),
                    AmbilightRefreshRatePerSecond = 1,
                    AmbilightMonitorIndex = 0,
                    JsonVersion = _latestSettingsVersion
                };
                _settings = userSettings;
            }
            else
            {
                try
                {
                    var json = File.ReadAllText(_settingsFileName);

                    var jtoken = JToken.Parse(json);

                    if (jtoken["JsonVersion"] == null) //TODO: move this to JsonMigrator?
                    {
                        jtoken["JsonVersion"] = _latestSettingsVersion;
                    }

                    jtoken = JsonMigrator.JsonMigrator.Migrate<UserSettings>(jtoken);

                    var userSettings = jtoken.ToObject<UserSettings>();

                    _settings = userSettings;

                    _settings.SaveSettings();
                }
                catch (Exception e)
                {
                    throw new SettingsFileJsonException(e);
                }
            }
        }

        public static bool HasSettings()
        {
            return File.Exists(_settingsFileName);
        }

        public static void DeleteSettings()
        {
            if (HasSettings())
            {
                File.Delete(_settingsFileName);
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

            File.WriteAllText(_settingsFileName, json);
        }

        public void AddDevice(Device device)
        {
            Devices.Add(device);
            SaveSettings();
        }

        public void DeleteActiveDevice()
        {
            var device = ActiveDevice;
            Devices.Remove(device);
            SaveSettings();
        }

        public void AddSchedule(Schedule schedule, bool makeActive)
        {
            var device = ActiveDevice;

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
            ActiveDevice.Schedules.ForEach(s => s.Active = false);

            schedule.Active = true;
            SaveSettings();
        }

        public void DeleteSchedule(Schedule schedule)
        {
            ActiveDevice.Schedules.Remove(schedule);
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
                            switch (trigger.TriggerType)
                            {
                                case TriggerType.Sunrise:
                                    trigger.Hours = sunriseHour;
                                    trigger.Minutes = sunriseMinute;
                                    break;
                                case TriggerType.Sunset:
                                    trigger.Hours = sunsetHour;
                                    trigger.Minutes = sunsetMinute;
                                    break;
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

        public void SetActiveDevice(string deviceName)
        {
            var newActiveDevice = Devices.FirstOrDefault(d => d.Name.Equals(deviceName));

            if (ActiveDevice != null)
            {
                ActiveDevice.ActiveInGUI = false;
            }

            newActiveDevice.ActiveInGUI = true;

            SaveSettings();
        }
        #endregion

        #region Migration methods
        [Migration("1", "2")]
        private static JToken Migration_1_2(JToken jToken)
        {
            return jToken; //Just update the version
        }

        [Migration("2", "3")]
        private static JToken Migration_2_3(JToken jToken)
        {
            jToken[nameof(AmbilightControlBrightness)] = false;          

            return jToken;
        }
        #endregion
    }
}
