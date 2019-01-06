using System.Collections.Generic;

namespace Nanoleaf_wpf.Models.Scheduling
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

        public Schedule()
        {
            Programs = new List<Program>();

            //Add a program for each day of the week
            Programs.Add(new Program());
            Programs.Add(new Program());
            Programs.Add(new Program());
            Programs.Add(new Program());
            Programs.Add(new Program());
            Programs.Add(new Program());
            Programs.Add(new Program());
        }
    }
}
