using System.Threading.Tasks;
using RestSharp;
using Winleafs.Models.Layouts;
using Winleafs.Nanoleaf.Endpoints.Interfaces;

namespace Winleafs.Nanoleaf.Endpoints
{
    public class LayoutEndpoint : NanoleafEndpoint, ILayoutEndpoint
    {
        private const string BaseEndpoint = "panelLayout";

        public LayoutEndpoint(NanoleafConnection connection) : base(connection)
        {
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

		Shape Mini Triangles (#203):

		return JsonConvert.DeserializeObject<Layout>("{\"numPanels\":29,\"sideLength\":0,\"positionData\":[{\"panelId\":34842,\"x\":636,\"y\":348,\"o\":0,\"shapeType\":9},{\"panelId\":36029,\"x\":569,\"y\":387,\"o\":300,\"shapeType\":7},{\"panelId\":6201,\"x\":536,\"y\":290,\"o\":60,\"shapeType\":8},{\"panelId\":14320,\"x\":569,\"y\":193,\"o\":240,\"shapeType\":8},{\"panelId\":21152,\"x\":603,\"y\":135,\"o\":300,\"shapeType\":9},{\"panelId\":39145,\"x\":569,\"y\":77,\"o\":240,\"shapeType\":8},{\"panelId\":51617,\"x\":670,\"y\":58,\"o\":300,\"shapeType\":8},{\"panelId\":34715,\"x\":770,\"y\":77,\"o\":240,\"shapeType\":8},{\"panelId\":30460,\"x\":837,\"y\":116,\"o\":300,\"shapeType\":8},{\"panelId\":10658,\"x\":569,\"y\":0,\"o\":300,\"shapeType\":8},{\"panelId\":3094,\"x\":469,\"y\":406,\"o\":60,\"shapeType\":8},{\"panelId\":31138,\"x\":502,\"y\":464,\"o\":240,\"shapeType\":9},{\"panelId\":50952,\"x\":469,\"y\":484,\"o\":180,\"shapeType\":9},{\"panelId\":27007,\"x\":469,\"y\":522,\"o\":0,\"shapeType\":9},{\"panelId\":52562,\"x\":402,\"y\":561,\"o\":60,\"shapeType\":7},{\"panelId\":17027,\"x\":335,\"y\":638,\"o\":300,\"shapeType\":8},{\"panelId\":29479,\"x\":301,\"y\":696,\"o\":240,\"shapeType\":9},{\"panelId\":29503,\"x\":268,\"y\":754,\"o\":60,\"shapeType\":8},{\"panelId\":47039,\"x\":167,\"y\":774,\"o\":120,\"shapeType\":8},{\"panelId\":32578,\"x\":100,\"y\":851,\"o\":120,\"shapeType\":7},{\"panelId\":18316,\"x\":33,\"y\":774,\"o\":120,\"shapeType\":8},{\"panelId\":37457,\"x\":100,\"y\":696,\"o\":60,\"shapeType\":8},{\"panelId\":36123,\"x\":0,\"y\":832,\"o\":60,\"shapeType\":9},{\"panelId\":10210,\"x\":201,\"y\":870,\"o\":180,\"shapeType\":8},{\"panelId\":13224,\"x\":268,\"y\":870,\"o\":0,\"shapeType\":9},{\"panelId\":13021,\"x\":402,\"y\":638,\"o\":0,\"shapeType\":9},{\"panelId\":7419,\"x\":670,\"y\":406,\"o\":60,\"shapeType\":8},{\"panelId\":8799,\"x\":703,\"y\":464,\"o\":240,\"shapeType\":9},{\"panelId\":0,\"x\":636,\"y\":319,\"o\":180,\"shapeType\":12}]}");

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
