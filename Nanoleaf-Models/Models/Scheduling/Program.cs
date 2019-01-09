using Nanoleaf_Models.Models.Scheduling.Triggers;
using System.Collections.Generic;

namespace Nanoleaf_Models.Models.Scheduling
{
    public class Program
    {
        public List<TimeTrigger> Triggers { get; set; }

        public Program()
        {
            Triggers = new List<TimeTrigger>();
        }
    }
}
