using System.Threading.Tasks;

namespace Winleafs.External.interfaces
{
    public interface IReleaseClient
    {
        /// <summary>
        /// Gets the latest version of the Winleafs application.
        /// </summary>
        /// <param name="usePreRelease">Determines if the PreRelease should be used.</param>
        /// <returns>The name of the latest release.</returns>
        Task<string> GetLatestVersion(bool usePreRelease = false);
    }
}