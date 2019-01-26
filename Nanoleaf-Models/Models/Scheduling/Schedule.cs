using Nanoleaf_Models.Models.Scheduling.Triggers;
using System.Collections.Generic;

namespace Nanoleaf_Models.Models.Scheduling
{
    public class Schedule
    {
        /// <summary>
        /// List of programs, length is always 7, in order of days from monday until sunday, starting at monday
        /// </summary>
        public List<Program> Programs { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Only 1 schedule can be active at any time
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Indicates whether the lights should be turned off when shutting down the application or Windows
        /// </summary>
        public bool TurnOffAtApplicationShutdown { get; set; }

        /// <summary>
        /// A schedule has a list of time-independent event triggers
        /// </summary>
        public List<IEventTrigger> EventTriggers { get; set; }

        public Schedule(bool addPrograms = false)
        {
            Programs = new List<Program>();

            if (addPrograms)
            {
                //Add a program for each day of the week
                Programs.Add(new Program());
                Programs.Add(new Program());
                Programs.Add(new Program());
                Programs.Add(new Program());
                Programs.Add(new Program());
                Programs.Add(new Program());
                Programs.Add(new Program());
            }

            EventTriggers = new List<IEventTrigger>();
        }
    }
}
