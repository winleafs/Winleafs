using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Winleafs.Models.Models.Scheduling.Triggers;
using Winleafs.Wpf.Helpers;

/*
Memory address locations:

{
  "Health_maximum": {
    "baseAddress": "32506856",
    "pointers": [ "0", "1760", "664", "940", "88" ]
  },

  "Health_current": {
    "baseAddress": "32506856",
    "pointers": [ "0", "1760", "664", "940", "108" ]
  },

  "Shield_maximum": {
    "baseAddress": "32506856",
    "pointers": [ "0", "1760", "664", "952", "88" ]
  },

  "Shield_current": {
    "baseAddress": "32506856",
    "pointers": [ "0", "1760", "664", "952", "108" ]
  }
}
*/

namespace Winleafs.Wpf.Api.Events
{
    /*public class Borderlands2HealthEventTrigger : BaseProcessPercentageEventTrigger
    {
        private static readonly string _processName = "Borderlands2";
        private static readonly int _maxHealthBaseAddress = 32506856;
        private static readonly int[] _maxHealthPointers = { 0, 1760, 664, 940, 88 };
        private static readonly int _currentHealthBaseAddress = 32506856;
        private static readonly int[] _currentHealthPointers = { 0, 1760, 664, 940, 108 };

        public Borderlands2HealthEventTrigger(ITrigger trigger, Orchestrator orchestrator) : base(trigger, orchestrator, _processName)
        {

        }

        /// <summary>
        /// Called by the base class, calculates the current health of the Borderlands character and then applies the percentage effect
        /// </summary>
        protected override async Task ApplyEffectLocalAsync(MemoryReader memoryReader)
        {
            var maxHealth = memoryReader.ReadFloat(_maxHealthBaseAddress, _maxHealthPointers);
            var currentHealth = memoryReader.ReadFloat(_currentHealthBaseAddress, _currentHealthPointers);

            try
            {
                var percentage = (100 / maxHealth) * currentHealth;
                await ApplyPercentageEffect(percentage);
            }
            catch
            {
                //Do nothing, percentage is applied with UDP and if that fails, it is not interesting
            }
        }
    }*/
}
