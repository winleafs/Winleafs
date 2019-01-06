using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nanoleaf_wpf.Models
{

    public class Effect
    {
        /// <summary>
        /// Container for all effects
        /// </summary>
        public static List<Effect> Effects { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
