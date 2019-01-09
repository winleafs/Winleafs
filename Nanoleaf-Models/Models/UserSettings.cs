using System.IO;
using System.Collections.Generic;
using System;
using Nanoleaf_Models.Models.Scheduling;
using Nanoleaf_Models.Models.Effects;
using Newtonsoft.Json;

namespace Nanoleaf_Models.Models
{
    public class UserSettings
    {
        private static readonly string SettingsFileName = "Settings.txt";

        public List<Schedule> Schedules { get; set; }
        public List<Effect> Effects { get; set; }

        public static void SaveSettings(List<Schedule> schedules, List<Effect> effects)
        {
            var settings = new UserSettings
            {
                Schedules = schedules,
                Effects = effects
            };

            var json = JsonConvert.SerializeObject(settings);

            File.WriteAllText(SettingsFileName, json);
        }

        public static UserSettings LoadSettings()
        {
            if (!File.Exists(SettingsFileName))
            {
                throw new NoSettingsFileException();
            }

            try
            {
                var json = File.ReadAllText(SettingsFileName);

                return JsonConvert.DeserializeObject<UserSettings>(json);
            }
            catch
            {
                throw new SettingsFileJsonException();
            }
        }
    }

    public class SettingsFileJsonException : Exception
    {
        public SettingsFileJsonException() : base("Error loading settings, corrupt JSON")
        {

        }
    }

    public class NoSettingsFileException : Exception
    {
        public NoSettingsFileException() : base("Error loading settings, no settings file yet")
        {

        }
    }
}
