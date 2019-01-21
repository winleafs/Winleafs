﻿using System.IO;
using System.Collections.Generic;
using System;
using Nanoleaf_Models.Models.Scheduling;
using Newtonsoft.Json;
using System.Linq;
using Nanoleaf_Models.Models.Effects;

namespace Nanoleaf_Models.Models
{
    public class UserSettings
    {
        private static readonly string SettingsFileName = "Settings.txt";

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

        public List<Device> Devices { get; set; }

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
            if (!File.Exists(SettingsFileName))
            {
                var userSettings = new UserSettings();

                userSettings.Devices = new List<Device>();

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
                catch
                {
                    throw new SettingsFileJsonException();
                }
            }
        }

        public static bool HasSettings()
        {
            return File.Exists(SettingsFileName);
        }

        public void SaveSettings()
        {
            var json = JsonConvert.SerializeObject(this);

            File.WriteAllText(SettingsFileName, json);
        }

        public void AddDevice(Device device)
        {
            Devices.Add(device);
            SaveSettings();
        }

        public void AddSchedule(Schedule schedule)
        {
            var device = ActviceDevice;
            device.Schedules.ForEach(s => s.Active = false);
            schedule.Active = true;

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
    }

    public class SettingsFileJsonException : Exception
    {
        public SettingsFileJsonException() : base("Error loading settings, corrupt JSON")
        {

        }
    }
}
