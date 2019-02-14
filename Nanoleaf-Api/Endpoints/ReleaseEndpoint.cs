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
        public async Task<string> GetLatestVersion()
        {
            var githubClient = new GitHubClient(new ProductHeaderValue("winleafs"));
            return (await githubClient.Repository.Release.GetLatest(USERNAME, REPOSITORY_NAME)).Name;
        }
    }
}