using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Winleafs.Models.Enums;
using Winleafs.Models.Models.Effects;
using Winleafs.Models.Models.Layouts;
using Winleafs.Models.Models.Scheduling;
using Winleafs.Models.Models.Scheduling.Triggers;

namespace Winleafs.Models.Models
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

        public string OverrideEffect { get; set; }
        public int OverrideBrightness { get; set; }

        public PercentageProfile PercentageProfile { get;set;}

        public ScreenMirrorAlgorithm ScreenMirrorAlgorithm { get; set; }

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

        public TimeTrigger GetActiveTimeTrigger()
        {
            if (Schedules.Any(s => s.Active))
            {
                return Schedules.FirstOrDefault(s => s.Active).GetActiveTimeTrigger();
            }
            else //It is possible that a user deletes the active schedule, then there is no active program
            {
                return null;
            }
        }

        public void LoadEffectsFromNameList(IEnumerable<string> effectNames)
        {
            Effects.Clear();

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
