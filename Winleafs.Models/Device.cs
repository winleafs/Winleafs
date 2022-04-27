using System.Collections.Generic;
using System.Linq;
using Winleafs.Models.Effects;
using Winleafs.Models.Enums;
using Winleafs.Models.Layouts;

namespace Winleafs.Models
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

        public List<Effect> Effects { get; set; }

        public string ManualEffect { get; set; }

        public int ManualBrightness { get; set; }

        public PercentageProfile PercentageProfile { get;set;}

        public ScreenMirrorAlgorithm ScreenMirrorAlgorithm { get; set; }

        public ScreenMirrorFlip ScreenMirrorFlip { get; set; }

        public Device()
        {
            Effects = new List<Effect>();
            OperationMode = OperationMode.Schedule;
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

        public void UpdateEffect(Effect newEffect)
        {
            Effects[Effects.FindIndex(effect => effect.Name.Equals(newEffect.Name))] = newEffect;
        }
    }
}
