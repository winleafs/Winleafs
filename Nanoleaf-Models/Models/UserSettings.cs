using System.IO;
using System.Collections.Generic;
using System;
using Nanoleaf_Models.Models.Scheduling;
using Nanoleaf_Models.Models.Effects;
using Newtonsoft.Json;
using System.Linq;

namespace Nanoleaf_Models.Models
{
    public class UserSettings
    {
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

        private static readonly string SettingsFileName = "Settings.txt";

        public List<Schedule> Schedules { get; set; }
        public List<Effect> Effects { get; set; }

        public static void LoadSettings()
        {
            if (!File.Exists(SettingsFileName))
            {
                var userSettings = new UserSettings();

                userSettings.Effects = new List<Effect>();

                //TODO: should load from settings, and a user can call a manual function to update the effects and the effects should be loaded when pairing a device
                userSettings.Effects.Add(new Effect { Name = "Flames" });
                userSettings.Effects.Add(new Effect { Name = "Forest" });
                userSettings.Effects.Add(new Effect { Name = "Nemo" });
                userSettings.Effects.Add(new Effect { Name = "Snowfall" });
                userSettings.Effects.Add(new Effect { Name = "Inner Peace" });
                userSettings.Effects = userSettings.Effects.OrderBy(eff => eff.Name).ToList();
                Effect.Effects = userSettings.Effects;

                userSettings.Schedules = new List<Schedule>();
                _settings = userSettings;
            }
            else
            {
                try
                {
                    var json = File.ReadAllText(SettingsFileName);

                    var userSettings = JsonConvert.DeserializeObject<UserSettings>(json);

                    Effect.Effects = userSettings.Effects;

                    _settings = userSettings;
                }
                catch
                {
                    throw new SettingsFileJsonException();
                }
            }
        }

        public void SaveSettings()
        {
            var json = JsonConvert.SerializeObject(this);

            File.WriteAllText(SettingsFileName, json);
        }

        public void AddSchedule(Schedule schedule)
        {
            Schedules.ForEach(s => s.Active = false);
            schedule.Active = true;

            Schedules.Add(schedule);
            Schedules = Schedules.OrderBy(s => s.Name).ToList();
            SaveSettings();
        }

        public void ActivateSchedule(Schedule schedule)
        {
            Schedules.ForEach(s => s.Active = false);

            schedule.Active = true;
            SaveSettings();
        }

        public void DeleteSchedule(Schedule schedule)
        {
            Schedules.Remove(schedule);
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
