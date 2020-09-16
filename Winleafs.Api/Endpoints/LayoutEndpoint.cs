using RestSharp;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Models.Models.Layouts;

namespace Winleafs.Api.Endpoints
{
    public class LayoutEndpoint : NanoleafEndpoint, ILayoutEndpoint
    {
        private const string BaseEndpoint = "panelLayout";

        /// <inheritdoc />
        public LayoutEndpoint(NanoleafClient client)
        {
            Client = client;
        }

        public GlobalOrientation GetGlobalOrientation()
        {
            return SendRequest<GlobalOrientation>($"{BaseEndpoint}/globalOrientation", Method.GET);
        }

        public async Task<GlobalOrientation> GetGlobalOrientationAsync()
        {
            return await SendRequestAsync<GlobalOrientation>($"{BaseEndpoint}/globalOrientation", Method.GET);
        }

        /// <inheritdoc />
        public Layout GetLayout()
        {
            //Hexagon test response
            /*           
            return JsonConvert.DeserializeObject<Layout>(@"{
				""numPanels"": 4,
				""sideLength"": 67,
				""positionData"": [{
						""panelId"": 19544,
						""x"": 100,
						""y"": 174,
						""o"": 0,
						""shapeType"": 7
					},
					{
						""panelId"": 55960,
						""x"": 0,
						""y"": 116,
						""o"": 180,
						""shapeType"": 7
					},
					{
						""panelId"": 46666,
						""x"": 0,
						""y"": 0,
						""o"": 180,
						""shapeType"": 7
					},
					{
						""panelId"": 0,
						""x"": 41,
						""y"": 208,
						""o"": 60,
						""shapeType"": 12
					}
				]
			}");
             */
            return SendRequest<Layout>($"{BaseEndpoint}/layout", Method.GET);
        }

        /// <inheritdoc />
        public async Task<Layout> GetLayoutAsync()
        {
            return await SendRequestAsync<Layout>($"{BaseEndpoint}/layout", Method.GET).ConfigureAwait(false);
        }
    }
}
