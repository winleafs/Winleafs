using System.Collections.Generic;
using System.Drawing;

namespace Winleafs.Models.Models.Layouts
{
    public class Frame
    {
        public IDictionary<int, Color> PanelColors { get; set; }

        public Frame()
        {
            PanelColors = new Dictionary<int, Color>();
        }
    }
}
