using Nanoleaf_Models.Models.Scheduling;
using System.Collections.Generic;

namespace Nanoleaf_Models.Models
{
    public class Device
    {
        public string IPAddress { get; set; }
        public string Name { get; set; }

        public List<Schedule> Schedules { get; set; }

        //public TimeTriggerTimer timeTriggerTimer { get; set; }

        public Device()
        {
            Schedules = new List<Schedule>();
        }
    }
}
