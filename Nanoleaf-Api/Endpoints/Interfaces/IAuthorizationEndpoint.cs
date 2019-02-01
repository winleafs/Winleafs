using System.Threading.Tasks;

namespace Winleafs.Api.Endpoints.Interfaces
{
    public interface IAuthorizationEndpoint
    {
        Task<string> GetAuthTokenAsync();

		string GetAuthToken();
    }
}
