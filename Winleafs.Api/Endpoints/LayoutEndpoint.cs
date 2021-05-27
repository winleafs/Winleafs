using RestSharp;
using System.Threading.Tasks;
using Winleafs.Api.DTOs;
using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Models.Models.Layouts;

namespace Winleafs.Api.Endpoints
{
	public class LayoutEndpoint : NanoleafEndpoint, ILayoutEndpoint
	{
		private const string BaseEndpoint = "panelLayout";

		/// <inheritdoc />
		public LayoutEndpoint(ClientDto client)
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
			return SendRequest<Layout>($"{BaseEndpoint}/layout", Method.GET);
		}

		/// <inheritdoc />
		public async Task<Layout> GetLayoutAsync()
		{
			return await SendRequestAsync<Layout>($"{BaseEndpoint}/layout", Method.GET).ConfigureAwait(false);
		}

		#region TestResponses
		/*
		Call with:  return JsonConvert.DeserializeObject<Layout>

		ShapeTriangles:

		return JsonConvert.DeserializeObject<Layout>("{\"numPanels\":10,\"sideLength\":0,\"positionData\":[{\"panelId\":17587,\"x\":201,\"y\":348,\"o\":0,\"shapeType\":8},{\"panelId\":24027,\"x\":134,\"y\":270,\"o\":300,\"shapeType\":8},{\"panelId\":48590,\"x\":100,\"y\":174,\"o\":360,\"shapeType\":8},{\"panelId\":614,\"x\":33,\"y\":96,\"o\":540,\"shapeType\":8},{\"panelId\":31646,\"x\":0,\"y\":0,\"o\":840,\"shapeType\":8},{\"panelId\":34370,\"x\":234,\"y\":444,\"o\":300,\"shapeType\":8},{\"panelId\":10501,\"x\":301,\"y\":522,\"o\":600,\"shapeType\":8},{\"panelId\":62516,\"x\":335,\"y\":618,\"o\":780,\"shapeType\":8},{\"panelId\":57725,\"x\":402,\"y\":696,\"o\":960,\"shapeType\":8},{\"panelId\":0,\"x\":234,\"y\":299,\"o\":180,\"shapeType\":12}]}");

		Hexagons:      
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

		#endregion
	}
}
