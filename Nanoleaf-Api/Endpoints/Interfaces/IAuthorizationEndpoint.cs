using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nanoleaf_Api.Endpoints.Interfaces
{
    public interface IAuthorizationEndpoint
    {
        Task<string> GetAuthToken(string ip, int port);
    }
}
