using System.Linq;
using System.Threading.Tasks;
using Octokit;
using Winleafs.Api.Endpoints.Interfaces;

namespace Winleafs.Api.Endpoints
{
    public class ReleaseEndpoint : IReleaseEndpoint
    {
        private const string USERNAME = "StijnOostdam";
        private const string REPOSITORY_NAME = "Winleafs";

        /// <inheritdoc />
        public async Task<string> GetLatestVersion(bool usePreRelease = false)
        {
            var githubClient = new GitHubClient(new ProductHeaderValue("winleafs"));
            if (usePreRelease)
            {
                var releases = await githubClient.Repository.Release.GetAll(USERNAME, REPOSITORY_NAME);
                var release = releases.OrderByDescending(x => x.CreatedAt).First();
                return release.Name;
            }
            
            return (await githubClient.Repository.Release.GetLatest(USERNAME, REPOSITORY_NAME).ConfigureAwait(false)).Name;
        }
    }
}