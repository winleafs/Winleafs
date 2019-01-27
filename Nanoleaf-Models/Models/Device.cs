using Nanoleaf_Models.Enums;
using Nanoleaf_Models.Models.Effects;
using Nanoleaf_Models.Models.Scheduling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoleaf_Models.Models
{
    public class Device
    {
        public string Name { get; set; }
        public string AuthToken { get; set; }
        public int Port { get; set; }
        public string IPAddress { get; set; }

        /// <summary>
        /// Determines which of the devices is currently active and being edited in the GUI
        /// </summary>
        public bool ActiveInGUI { get; set; }

        /// <summary>
        /// OperationMode is Schedule by default but user can override it to Manual
        /// </summary>
        public OperationMode OperationMode { get; set; }

        public List<Schedule> Schedules { get; set; }
        public List<Effect> Effects { get; set; }

        [JsonIgnore]
        public Schedule ActiveSchedule
        {
            get
            {
                return Schedules.FirstOrDefault(s => s.Active);
            }
        }

        public Device()
        {
            Schedules = new List<Schedule>();

            Effects = new List<Effect>();
        }

        public Program GetTodaysProgram()
        {
            if (Schedules.Any(s => s.Active))
            {
                var dayOfWeek = DateTime.Now.DayOfWeek; //Sunday = 0

                var index = dayOfWeek == DayOfWeek.Sunday ? 6 : (int)dayOfWeek - 1;

                return Schedules.FirstOrDefault(s => s.Active).Programs[index];
            }
            else //It is possible that a user deletes the active schedule, then there is no active program
            {
                return null;
            }
        }

        public void LoadEffectsFromNameList(IEnumerable<string> effectNames)
        {
            foreach (var effectName in effectNames)
            {
                Effects.Add(new Effect { Name = effectName });
            }

            Effects = Effects.OrderBy(eff => eff.Name).ToList();
        }

        /// <summary>
        /// Used by setup view
        /// </summary>
        public override string ToString()
        {
            return $"{Name} ({IPAddress}:{Port})";
        }
    }
}
