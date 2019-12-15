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

        public List<Effect> Effects { get; set; }

        public string ManualEffect { get; set; }
        public int ManualBrightness { get; set; }

        public PercentageProfile PercentageProfile { get;set;}

        public ScreenMirrorAlgorithm ScreenMirrorAlgorithm { get; set; }

        public int ScreenMirrorRefreshRatePerSecond { get; set; }
        public int ScreenMirrorMonitorIndex { get; set; }
        public bool ScreenMirrorControlBrightness { get; set; }

        public Dictionary<string, ulong> EffectCounter { get; set; }

        public Device()
        {
            Effects = new List<Effect>();
            ScreenMirrorRefreshRatePerSecond = 1;
            ScreenMirrorControlBrightness = false;
            OperationMode = OperationMode.Manual;
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
        /// Removes all entrie in <see cref="EffectCounter"/>
        /// which do not appear in the effects list or in the given
        /// <paramref name="customEffectNames"/>.
        /// </summary>
        public void CleanEffectCounter(IEnumerable<string> customEffectNames)
        {
            foreach (var effectName in EffectCounter.Keys.ToList()) //ToList to make a copy so we can safely remove entries
            {
                if (!Effects.Any(effect => effect.Name == effectName) && !customEffectNames.Contains(effectName))
                {
                    EffectCounter.Remove(effectName);
                }
            }
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
