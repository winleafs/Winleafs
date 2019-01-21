using Nanoleaf_Models.Models.Effects;
using Nanoleaf_Models.Models.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoleaf_Models.Models
{
    public class Device
    {
        public string Name { get; set; }
        public string AuthToken { get; set; }
        public string IPAddress { get; set; }

        /// <summary>
        /// Determines which of the devices is currently active and being edited in the GUI
        /// </summary>
        public bool ActiveInGUI { get; set; }

        public List<Schedule> Schedules { get; set; }
        public List<Effect> Effects { get; set; }

        public Device()
        {
            Schedules = new List<Schedule>();

            Effects = new List<Effect>();

            //TODO: should load from settings, and a user can call a manual function to update the effects and the effects should be loaded when pairing a device
            Effects.Add(new Effect { Name = "Flames" });
            Effects.Add(new Effect { Name = "Forest" });
            Effects.Add(new Effect { Name = "Nemo" });
            Effects.Add(new Effect { Name = "Snowfall" });
            Effects.Add(new Effect { Name = "Inner Peace" });
            Effects = Effects.OrderBy(eff => eff.Name).ToList();
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
    }
}
