using System.Linq;
using System.Threading.Tasks;
using Octokit;
using Winleafs.External.interfaces;

namespace Winleafs.External
{
    public class ReleaseClient : IReleaseClient
    {
        private const string Username = "StijnOostdam";
        private const string RepsitoryName = "Winleafs";

        /// <inheritdoc />
        public async Task<string> GetLatestVersion(bool usePreRelease = false)
        {
            var githubClient = new GitHubClient(new ProductHeaderValue("winleafs"));
            if (!usePreRelease)
            {
                return (await githubClient.Repository.Release.GetLatest(Username, RepsitoryName).ConfigureAwait(false))
                    .TagName;
            }

            var releases = await githubClient.Repository.Release.GetAll(Username, RepsitoryName).ConfigureAwait(false);
            var release = releases.OrderByDescending(x => x.CreatedAt).First();
            return release.TagName;

        }
    }
}