using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winleafs.Models.Models.Effects
{
    public class CustomEffectCommand
    {
        /// <summary>
        /// The command can be either be add, update or display:
        /// add - will create a new custom effect, or update one if it has the same animName
        /// update - will update an existing effect with the same animName
        /// display - will show the effect on the device but will not save it
        /// </summary>
        [JsonProperty("command")]
        public string Command { get; set; } 

        /// <summary>
        /// The version of the JSON schema.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; } = "1.0";

        /// <summary>
        /// The name of the custom effect
        /// </summary>
        [JsonProperty("animName")]
        public string AnimName { get; set; }

        /// <summary>
        /// The type of animation, can be either custom or static.
        /// </summary>
        [JsonProperty("animType")]
        public string AnimType { get; set; }

        /// <summary>
        /// A data structure that specifies the speed at which frames transition from coor to color. 
        /// See 3.2.6.1 Custom effect at https://forum.nanoleaf.me/docs/openapi for more information.
        /// </summary>
        [JsonProperty("animData")]
        public string AnimData { get; set; }

        /// <summary>
        /// Determines whether the custom effect will repeat.
        /// </summary>
        [JsonProperty("loop")]
        public bool Loop { get; set; }

        /// <summary>
        /// This has no functional use for the custom effect, it just sets
        /// the colors to be displayed in apps that list the effect. 
        /// </summary>
        [JsonProperty("palette")]
        public IList<Palette> Palette { get; set; } = new List<Palette>();
    }
}
