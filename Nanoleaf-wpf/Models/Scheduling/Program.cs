using System.Collections.Generic;

namespace Nanoleaf_wpf.Models.Scheduling
{
    public class Program
    {
        public List<Trigger> Triggers { get; set; }

        public Program()
        {
            Triggers = new List<Trigger>();
        }
    }
}
