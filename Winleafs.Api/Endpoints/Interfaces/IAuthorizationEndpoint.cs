using System.Threading.Tasks;

namespace Winleafs.Nanoleaf.Endpoints.Interfaces
{
    public interface IAuthorizationEndpoint
    {
        /// <summary>
        /// Gets the API token from the Nanoleaf device.
        /// </summary>
        /// <returns>The Nanoleaf API token.</returns>
        Task<string> GetAuthTokenAsync();

        /// <inheritdoc cref="GetAuthTokenAsync"/>
		string GetAuthToken();
    }
}
