using System.Collections.Generic;

namespace Winleafs.Models.Models.Layouts
{
    /// <summary>
    /// A user created Custom Effect which specifies a static pattern or animation
    /// </summary>
    public class CustomEffect
    {
        /// <summary>
        /// True if this is just one Frame to set each panel's color
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// A series of frames each specifying the color for each panel
        /// </summary>
        public IList<Frame> Frames { get; set; }

        public CustomEffect()
        {
            Frames = new List<Frame>();
        }
    }
}
