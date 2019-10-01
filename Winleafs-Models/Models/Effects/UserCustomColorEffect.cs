using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Winleafs.Models.Models.Effects
{
    public class UserCustomColorEffect
    {
        /// <summary>
        /// The name of the effect supplied by the user.
        /// </summary>
        public string EffectName { get; set; }

        /// <summary>
        /// The color that the nanoleaf device should be.
        /// </summary>
        public Color Color { get; set; }
    }
}
