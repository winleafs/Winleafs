using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Nanoleaf_Api.Models.State;

namespace Nanoleaf_Api.Endpoints.Interfaces
{
    public interface IStateEndpoint
    {
        /// <summary>
        /// Gets the state of the Nanoleaf.
        /// </summary>
        /// <returns>A value to indicate whether the Nanoleaf is on or off.</returns>
        Task<bool> GetState();

        /// <summary>
        /// Sets the state of the Nanoleaf.
        /// </summary>
        /// <param name="state">The state that the Nanoleaf should be.</param>
        /// <returns>An awaitable task.</returns>
        Task SetState(bool state);

        /// <summary>
        /// Gets the current brightness of the Nanoleaf.
        /// </summary>
        /// <returns>The current brightness.</returns>
        Task<StateModel> GetBrightness();

        /// <summary>
        /// Sets the brightness of the Nanoleaf.
        /// </summary>
        /// <param name="value">The value wanting the brightness to be set to.</param>
        /// <param name="duration">The duration for how long the brightness should be set to.</param>
        /// <returns>An awaitable task.</returns>
        Task SetBrightness(int value, int duration);

        /// <summary>
        /// Increment the brightness of the Nanoleaf.
        /// </summary>
        /// <param name="increment">How much should be added to the current brightness.</param>
        /// <returns>An awaitable task.</returns>
        Task IncrementBrightness(int increment);

        /// <summary>
        /// Gets the current hue of the Nanoleaf.
        /// </summary>
        /// <returns>The hue of the Nanoleaf.</returns>
        Task<StateModel> GetHue();

        /// <summary>
        /// Sets the hue of the Nanoleaf.
        /// </summary>
        /// <param name="value">The value the hue should be set to.</param>
        /// <returns>An awaitable task.</returns>
        Task SetHue(int value);

        /// <summary>
        /// Increments the hue of the Nanoleaf.
        /// </summary>
        /// <param name="increment">The increment that should be added to the current value.</param>
        /// <returns>An awaitable task.</returns>
        Task IncrementHue(int increment);

        /// <summary>
        /// Gets the current saturation of the Nanoleaf.
        /// </summary>
        /// <returns>The saturation of the Nanoleaf.</returns>
        Task<StateModel> GetSaturation();

        /// <summary>
        /// Sets the saturation of the Nanoleaf.
        /// </summary>
        /// <param name="value">The value wanting it to be set to.</param>
        /// <returns>An awaitable task.</returns>
        Task SetSaturation(int value);

        /// <summary>
        /// Increments the saturation of the Nanoleaf.
        /// </summary>
        /// <param name="increment">The amount wanting to be incremented with.</param>
        /// <returns>An awaitable task.</returns>
        Task IncrementSaturation(int increment);

        /// <summary>
        /// Gets the current color temperature of the Nanoleaf.
        /// </summary>
        /// <returns>The color temperature of the Nanoleaf.</returns>
        Task<StateModel> GetColorTemperature();

        /// <summary>
        /// Sets the color temperature of the Nanoleaf.
        /// </summary>
        /// <param name="value">The value wanting it to be set to.</param>
        /// <returns>An awaitable task.</returns>
        Task SetColorTemperature(int value);

        /// <summary>
        /// Increments the color temperature of the Nanoleaf.
        /// </summary>
        /// <param name="increment">The amount wanting to be incremented with.</param>
        /// <returns>An awaitable task.</returns>
        Task IncrementColorTemperature(int increment);

        /// <summary>
        /// Gets the current color mode of the Nanoleaf.
        /// </summary>
        /// <returns>The color mode of the Nanoleaf.</returns>
        Task<string> GetColorMode();
    }
}
