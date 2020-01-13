using JsonMigrator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Winleafs.Models.Enums;
using Winleafs.Models.Exceptions;
using Winleafs.Models.Models.Effects;
using Winleafs.Models.Models.Scheduling;
using Winleafs.Models.Models.Scheduling.Triggers;

namespace Winleafs.Models.Models
{
    public class UserSettings
    {
        public static readonly string APPLICATIONNAME = "Winleafs";
        public static readonly string APPLICATIONVERSION = "v1.0.0";

        public static readonly string SettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATIONNAME);

        public static readonly string CustomColorNamePreface = "Custom Color - ";
        public static readonly string EffectNamePreface = "Winleafs - ";

        private static readonly string _settingsFileName = Path.Combine(SettingsFolder, "Settings.txt");
        private static readonly string _latestSettingsVersion = "9";

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

        public List<UserCustomColorEffect> CustomEffects { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public int? SunriseHour { get; set; }
        public int? SunriseMinute { get; set; }
        public int? SunsetHour { get; set; }
        public int? SunsetMinute { get; set; }

        public bool StartAtWindowsStartup { get; set; }

        public string UserLocale { get; set; }

        public bool MinimizeToSystemTray { get; set; }

        public List<Schedule> Schedules { get; set; }

        public int ScreenMirrorRefreshRatePerSecond { get; set; }

        public int ScreenMirrorMonitorIndex { get; set; }
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

        /// <summary>
        /// Loads the <see cref="UserSettings"/> from the JSON file.
        /// If the user has no settings, the defaults will be generated.
        /// </summary>
        public static void LoadSettings()
        {
            if (!HasSettings())
            {
                var userSettings = new UserSettings
                {
                    // Defaults
                    Devices = new List<Device>(),
                    Schedules = new List<Schedule>(),
                    JsonVersion = _latestSettingsVersion,
                    ScreenMirrorRefreshRatePerSecond = 5,
                    ScreenMirrorMonitorIndex = 0
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

        /// <summary>
        /// Checks if the user has a settings file at the specified location.
        /// </summary>
        /// <returns></returns>
        public static bool HasSettings()
        {
            return File.Exists(_settingsFileName);
        }

        /// <summary>
        /// Deletes the settings file and object if the <see cref="UserSettings"/> exist.
        /// </summary>
        public static void DeleteSettings()
        {
            if (HasSettings())
            {
                File.Delete(_settingsFileName);
                _settings = null;
            }
        }

        /// <summary>
        /// Saves the current <see cref="UserSettings"/> to a JSON configuration file.
        /// </summary>
        public void SaveSettings()
        {
            var json = JsonConvert.SerializeObject(this);

            if (!Directory.Exists(SettingsFolder))
            {
                Directory.CreateDirectory(SettingsFolder);
            }

            File.WriteAllText(_settingsFileName, json);
        }

        /// <summary>
        /// Adds a <see cref="Device"/> to the list of devices and saves the settings.
        /// </summary>
        /// <param name="device">The <see cref="Device"/> to be added.</param>
        public void AddDevice(Device device)
        {
            Devices.Add(device);
            SaveSettings();
        }

        /// <summary>
        /// Deletes the given <see cref="Device"/> and saves the settings.
        /// </summary>
        public void DeleteDevice(string deviceName)
        {
            Devices.Remove(Devices.FirstOrDefault(device => device.Name == deviceName));


            if (Schedules == null)
            {
                SaveSettings();
                return;
            }

            foreach (var schedule in Schedules)
            {
                schedule.AppliesToDeviceNames.Remove(deviceName);
            }

            SaveSettings();
        }

        /// <summary>
        /// Adds a new schedule and saves the settings.
        /// </summary>
        /// <param name="schedule">The <see cref="Schedule"/> object to be added.</param>
        /// <param name="makeActive">If the schedule added should be set as the active schedule.</param>
        public void AddSchedule(Schedule schedule, bool makeActive)
        {
            if (makeActive)
            {
                Schedules.ForEach(s => s.Active = false);
                schedule.Active = true;
            }

            Schedules.Add(schedule);
            Schedules = Schedules.OrderBy(s => s.Name).ToList();
            SaveSettings();
        }

        /// <summary>
        /// Deactivates all schedules and activates the given <paramref name="schedule"/>.
        /// Also saves the settings to the JSON file.
        /// </summary>
        /// <param name="schedule">The <see cref="Schedule"/> object to be activated.</param>
        public void ActivateSchedule(Schedule schedule)
        {
            Schedules.ForEach(s => s.Active = false);

            schedule.Active = true;
            SaveSettings();
        }

        /// <summary>
        /// Deletes the given <paramref name="schedule"/> and saves the settings.
        /// </summary>
        /// <param name="schedule">The <see cref="Schedule"/> object to be deleted.</param>
        public void DeleteSchedule(Schedule schedule)
        {
            Schedules.Remove(schedule);
            SaveSettings();
        }

        /// <summary>
        /// Updates the sunrise and sunset times over all triggers in all devices
        /// and schedules.
        /// Also save the settings to the JSON file.
        /// </summary>
        /// <param name="sunriseHour">The new hour of sunrise.</param>
        /// <param name="sunriseMinute">The new minute of sunrise.</param>
        /// <param name="sunsetHour">The new hour of sunset.</param>
        /// <param name="sunsetMinute">The new minute of sunset.</param>
        public void UpdateSunriseSunset(int sunriseHour, int sunriseMinute, int sunsetHour, int sunsetMinute)
        {
            SunriseHour = sunriseHour;
            SunriseMinute = sunriseMinute;
            SunsetHour = sunsetHour;
            SunsetMinute = sunsetMinute;

            //Update each sunset and sunrise trigger to the new times
            foreach (var schedule in Schedules)
            {
                foreach (var program in schedule.Programs)
                {
                    foreach (var trigger in program.Triggers)
                    {
                        switch (trigger.EventTriggerType)
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
                    // Puts all triggers of all programs in correct order,
                    // this is needed since the times of triggers can change due to sunrise and sunset times
                    program.ReorderTriggers();
                }
            }

            SaveSettings();
        }

        /// <summary>
        /// Resets all operation modes of all device to Schedule,
        /// used at application startup (then users can switch to manual during runtime)
        /// </summary>
        public void ResetOperationModes()
        {
            foreach (var device in Devices)
            {
                device.OperationMode = OperationMode.Schedule;
            }
        }

        /// <summary>
        /// Sets a <see cref="Device"/> as the active one based on the
        /// <paramref name="deviceName"/> and saves the settings.
        /// </summary>
        /// <param name="deviceName">The name of the device to be searched for.</param>
        public void SetActiveDevice(string deviceName)
        {
            var newActiveDevice = Devices.FirstOrDefault(device =>
                device.Name.Equals(deviceName));

            if (ActiveDevice != null)
            {
                ActiveDevice.ActiveInGUI = false;
            }

            newActiveDevice.ActiveInGUI = true;

            SaveSettings();
        }

        [JsonIgnore]
        public Schedule ActiveSchedule
        {
            get
            {
                return Schedules?.FirstOrDefault(s => s.Active);
            }
        }

        public TimeTrigger GetActiveTimeTriggerForDevice(string deviceName)
        {
            if (Schedules?.Any(s => s.Active && s.AppliesToDeviceNames.Contains(deviceName)) == true)
            {
                return Schedules?.FirstOrDefault(s => s.Active && s.AppliesToDeviceNames.Contains(deviceName)).GetActiveTimeTrigger();
            }
            else //It is possible that a user deletes the active schedule, then there is no active program
            {
                return null;
            }
        }

        /// <summary>
        /// Deletes any trigger in all schedules which effect name is
        /// contained within the given <paramref name="effectNames"/>.
        /// </summary>
        public void DeleteTriggers(IEnumerable<string> effectNames)
        {
            foreach (var schedule in Schedules)
            {
                schedule.DeleteTriggers(effectNames);
            }
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
            jToken["ScreenMirrorControlBrightness"] = false;          

            return jToken;
        }

        [Migration("3", "4")]
        private static JToken Migration_3_4(JToken jToken)
        {
            jToken[nameof(MinimizeToSystemTray)] = true;

            return jToken;
        }

        [Migration("4", "5")]
        private static JToken Migration_4_5(JToken jToken)
        {
            var jTokenAsString = jToken.ToString();
            jTokenAsString = jTokenAsString.Replace("Winleafs - Ambilight", "Winleafs - Screen mirror"); //We are renaming the effect so replace all occurences of the name

            jToken = JToken.Parse(jTokenAsString);

            foreach (var device in jToken["Devices"])
            {
                device["ScreenMirrorAlgorithm"] = 0;
                device["ScreenMirrorControlBrightness"] = jToken["AmbilightControlBrightness"];
                device[nameof(ScreenMirrorMonitorIndex)] = jToken["AmbilightMonitorIndex"];
                device[nameof(ScreenMirrorRefreshRatePerSecond)] = jToken["AmbilightRefreshRatePerSecond"];
            }

            jToken["AmbilightRefreshRatePerSecond"].Parent.Remove();
            jToken["AmbilightMonitorIndex"].Parent.Remove();
            jToken["AmbilightControlBrightness"].Parent.Remove();

            return jToken;
        }

        [Migration("5", "6")]
        private static JToken Migration_5_6(JToken jToken)
        {
            jToken[nameof(CustomEffects)] = new JArray();

            return jToken;
        }

        [Migration("6", "7")]
        private static JToken Migration_6_7(JToken jToken)
        {
            foreach (var device in jToken["Devices"])
            {
                device["EffectCounter"] = JToken.FromObject(new Dictionary<string, int>());
            }

            return jToken;
        }
        
        [Migration("7", "8")]
        private static JToken Migration_7_8(JToken jToken)
        {
            jToken[nameof(Schedules)] = new JArray();

            return jToken;
        }

        [Migration("8", "9")]
        private static JToken Migration_8_9(JToken jToken)
        {
            jToken[nameof(ScreenMirrorMonitorIndex)] = 0;
            jToken[nameof(ScreenMirrorRefreshRatePerSecond)] = 5;

            return jToken;
        }
        #endregion
    }
}
