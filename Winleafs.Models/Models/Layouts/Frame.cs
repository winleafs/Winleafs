using System.Collections.Generic;

namespace Winleafs.Models.Models.Layouts
{
    public class Frame
    {
        public IDictionary<int, uint> PanelColors { get; set; }

        public Frame()
        {
            PanelColors = new Dictionary<int, uint>();
        }
    }
}
