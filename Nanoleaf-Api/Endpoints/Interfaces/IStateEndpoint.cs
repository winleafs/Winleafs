using System.Threading.Tasks;

using Winleafs.Models.Models.State;

namespace Winleafs.Api.Endpoints.Interfaces
{
    public interface IStateEndpoint
    {
        /// <summary>
        /// Gets the state of the Nanoleaf.
        /// </summary>
        /// <returns>A value to indicate whether the Nanoleaf is on or off.</returns>
        Task<OnOffModel> GetStateAsync();

		/// <summary>
		/// Gets the state of the Nanoleaf.
		/// </summary>
		/// <returns>A value to indicate whether the Nanoleaf is on or off.</returns>
		OnOffModel GetState();

        /// <summary>
        /// Sets the state of the Nanoleaf.
        /// </summary>
        /// <param name="state">The state that the Nanoleaf should be.</param>
        /// <returns>An awaitable task.</returns>
        Task SetStateAsync(bool state);

		/// <summary>
		/// Sets the state of the Nanoleaf.
		/// </summary>
		/// <param name="state">The state that the Nanoleaf should be.</param>
		void SetState(bool state);

        /// <summary>
        /// Sets the state of the Nanoleaf but checks the current state before setting the state.
        /// </summary>
        /// <param name="state">The state that the Nanoleaf should be.</param>
        /// <returns>An awaitable task.</returns>
        Task SetStateWithStateCheckAsync(bool state);

		/// <summary>
		/// Sets the state of the Nanoleaf but checks the current state before setting the state.
		/// </summary>
		/// <param name="state">The state that the Nanoleaf should be.</param>
		void SetStateWithStateCheck(bool state);

        /// <summary>
        /// Gets the current brightness of the Nanoleaf.
        /// </summary>
        /// <returns>The current brightness.</returns>
        Task<StateModel> GetBrightnessAsync();

		/// <summary>
		/// Gets the current brightness of the Nanoleaf.
		/// </summary>
		/// <returns>The current brightness.</returns>
		StateModel GetBrightness();

        /// <summary>
        /// Sets the brightness of the Nanoleaf.
        /// </summary>
        /// <param name="value">The value wanting the brightness to be set to.</param>
        /// <param name="duration">The duration for how long the brightness should be set to.</param>
        /// <returns>An awaitable task.</returns>
        Task SetBrightnessAsync(int value, int? duration = null);

		/// <summary>
		/// Sets the brightness of the Nanoleaf.
		/// </summary>
		/// <param name="value">The value wanting the brightness to be set to.</param>
		/// <param name="duration">The duration for how long the brightness should be set to.</param>
		void SetBrightness(int value, int? duration = null);

        /// <summary>
        /// Increment the brightness of the Nanoleaf.
        /// </summary>
        /// <param name="increment">How much should be added to the current brightness.</param>
        /// <returns>An awaitable task.</returns>
        Task IncrementBrightnessAsync(int increment);

		/// <summary>
		/// Increment the brightness of the Nanoleaf.
		/// </summary>
		/// <param name="increment">How much should be added to the current brightness.</param>
		void IncrementBrightness(int increment);

        /// <summary>
        /// Gets the current hue of the Nanoleaf.
        /// </summary>
        /// <returns>The hue of the Nanoleaf.</returns>
        Task<StateModel> GetHueAsync();

		/// <summary>
		/// Gets the current hue of the Nanoleaf.
		/// </summary>
		/// <returns>The hue of the Nanoleaf.</returns>
		StateModel GetHue();

        /// <summary>
        /// Sets the hue of the Nanoleaf.
        /// </summary>
        /// <param name="value">The value the hue should be set to.</param>
        /// <returns>An awaitable task.</returns>
        Task SetHueAsync(int value);

		/// <summary>
		/// Sets the hue of the Nanoleaf.
		/// </summary>
		/// <param name="value">The value the hue should be set to.</param>
		void SetHue(int value);

        /// <summary>
        /// Increments the hue of the Nanoleaf.
        /// </summary>
        /// <param name="increment">The increment that should be added to the current value.</param>
        /// <returns>An awaitable task.</returns>
        Task IncrementHueAsync(int increment);

		/// <summary>
		/// Increments the hue of the Nanoleaf.
		/// </summary>
		/// <param name="increment">The increment that should be added to the current value.</param>
		void IncrementHue(int increment);

        /// <summary>
        /// Gets the current saturation of the Nanoleaf.
        /// </summary>
        /// <returns>The saturation of the Nanoleaf.</returns>
        Task<StateModel> GetSaturationAsync();

		/// <summary>
		/// Gets the current saturation of the Nanoleaf.
		/// </summary>
		/// <returns>The saturation of the Nanoleaf.</returns>
		StateModel GetSaturation();

        /// <summary>
        /// Sets the saturation of the Nanoleaf.
        /// </summary>
        /// <param name="value">The value wanting it to be set to.</param>
        /// <returns>An awaitable task.</returns>
        Task SetSaturationAsync(int value);

		/// <summary>
		/// Sets the saturation of the Nanoleaf.
		/// </summary>
		/// <param name="value">The value wanting it to be set to.</param>
		void SetSaturation(int value);

        /// <summary>
        /// Increments the saturation of the Nanoleaf.
        /// </summary>
        /// <param name="increment">The amount wanting to be incremented with.</param>
        /// <returns>An awaitable task.</returns>
        Task IncrementSaturationAsync(int increment);

		/// <summary>
		/// Increments the saturation of the Nanoleaf.
		/// </summary>
		/// <param name="increment">The amount wanting to be incremented with.</param>
		void IncrementSaturation(int increment);

        /// <summary>
        /// Gets the current color temperature of the Nanoleaf.
        /// </summary>
        /// <returns>The color temperature of the Nanoleaf.</returns>
        Task<StateModel> GetColorTemperatureAsync();

		/// <summary>
		/// Gets the current color temperature of the Nanoleaf.
		/// </summary>
		/// <returns>The color temperature of the Nanoleaf.</returns>
		StateModel GetcolorTemperature();

        /// <summary>
        /// Sets the color temperature of the Nanoleaf.
        /// </summary>
        /// <param name="value">The value wanting it to be set to.</param>
        /// <returns>An awaitable task.</returns>
        Task SetColorTemperatureAsync(int value);

		/// <summary>
		/// Sets the color temperature of the Nanoleaf.
		/// </summary>
		/// <param name="value">The value wanting it to be set to.</param>
		void SetColorTemperature(int value);

        /// <summary>
        /// Increments the color temperature of the Nanoleaf.
        /// </summary>
        /// <param name="increment">The amount wanting to be incremented with.</param>
        /// <returns>An awaitable task.</returns>
        Task IncrementColorTemperatureAsync(int increment);

		/// <summary>
		/// Increments the color temperature of the Nanoleaf.
		/// </summary>
		/// <param name="increment">The amount wanting to be incremented with.</param>
		void IncrementColorTemprature(int increment);

        /// <summary>
        /// Gets the current color mode of the Nanoleaf.
        /// </summary>
        /// <returns>The color mode of the Nanoleaf.</returns>
        Task<string> GetColorModeAsync();

		/// <summary>
		/// Gets the current color mode of the Nanoleaf.
		/// </summary>
		/// <returns>The color mode of the Nanoleaf.</returns>
		string GetColorMode();

        /// <summary>
        /// Sets the hue and saturation of the Nanoleaf.
        /// </summary>
        /// <param name="hue">Hue in degrees</param>
        /// <param name="saturation">Saturation in percentages, 0 to 100</param>
        void SetHueAndSaturation(int hue, int saturation);

        /// <summary>
        /// Sets the hue and saturation of the Nanoleaf.
        /// </summary>
        /// <param name="hue">Hue in degrees</param>
        /// <param name="saturation">Saturation in percentages, 0 to 100</param>
        /// <returns>An awaitable task.</returns>
        Task SetHueAndSaturationAsync(int hue, int saturation);
    }
}
