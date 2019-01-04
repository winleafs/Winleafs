using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nanoleaf_wpf.Models
{
    public class Device
    {
        public string Name { get; set; }
        public string IpAddress { get; set; }

        //ToString is used by the list box
        public override string ToString()
        {
            return $"{Name} ({IpAddress})";
        }
    }
}
