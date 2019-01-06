using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nanoleaf_Api.Models.Effects;

namespace Nanoleaf_Api.Endpoints.Interfaces
{
    public interface IEffectsEndpoint
    {
        /// <summary>
        /// Gets a list of all available effects.
        /// </summary>
        /// <returns>A list of values that represent available effects.</returns>
        Task<IEnumerable<string>> GetEffectsListAsync();

        /// <summary>
        /// Gets the currently selected effect.
        /// </summary>
        /// <returns>A value representing the currently selected effect.</returns>
        Task<string> GetSelectedEffectAsync();

        /// <summary>
        /// Set the current effect to the Nanoleaf.
        /// </summary>
        /// <param name="effectName">The name of the effect wanting to be set.</param>
        /// <returns>An awaitable task.</returns>
        Task SetSelectedEffectAsync(string effectName);

        /// <summary>
        /// Gets the details of an effect based on the name.
        /// </summary>
        /// <param name="effectName">The name of the effect wanting to be gotten.</param>
        /// <returns>The details about the effect.</returns>
        Task<Effect> GetEffectDetailsAsync(string effectName);
    }
}
