using System.Drawing;

namespace Winleafs.Models.Models.Layouts
{
    public class ScreenMirrorPanel
    {
        public int PanelId { get; set; }

        public Color CurrentColor { get; set; }

        public Rectangle BitmapArea { get; set; }
    }
}
