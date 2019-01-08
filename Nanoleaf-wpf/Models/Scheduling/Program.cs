using Nanoleaf_wpf.Models.Scheduling.Triggers;
using System.Collections.Generic;

namespace Nanoleaf_wpf.Models.Scheduling
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
