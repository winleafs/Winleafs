using LargestOrthogonalRectangleForVoronoi;
using Supercluster.KDTree;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using VoronoiLib;
using VoronoiLib.Structures;
using Winleafs.Api;
using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Models.Models;
using Winleafs.Wpf.Api.Layouts;
using Winleafs.Wpf.Helpers;
using Winleafs.Wpf.Helpers.Voronoi;

namespace Winleafs.Wpf.Api.Effects.ScreenMirrorEffects
{
    public class NearestNeighbour : IScreenMirrorEffect
    {
        private readonly INanoleafClient _nanoleafClient;
        private readonly IExternalControlEndpoint _externalControlEndpoint;
        private readonly Rectangle _screenBounds;

        private Func<double[], double[], double> EuclideanDistance = (x, y) =>
        {
            double dist = 0;
            for (int i = 0; i < x.Length; i++)
            {
                dist += (x[i] - y[i]) * (x[i] - y[i]);
            }

            return dist;
        };

        public NearestNeighbour(Device device, Orchestrator orchestrator, INanoleafClient nanoleafClient)
        {
            _nanoleafClient = nanoleafClient;

            _externalControlEndpoint = _nanoleafClient.ExternalControlEndpoint;

            _screenBounds = MonitorHelper.GetScreenBounds(device.ScreenMirrorMonitorIndex);

            var panels = orchestrator.PanelLayout.GetScaledTriangles(_screenBounds.Width, _screenBounds.Height, ScaleType.Stretch);

            ConstructScreenshotAreas(panels);
        }

        public Task ApplyEffect()
        {
            throw new NotImplementedException();
        }

        private void ConstructScreenshotAreasPixels(List<DrawablePanel> panels)
        {
            //Prepare the points to search on + the dictionairy to save the results in
            var points = new List<double[]>();
            var pixelsForPanels = new Dictionary<DrawablePanel, List<double[]>>();

            foreach (var panel in panels)
            {
                points.Add(new double[] { panel.MidPoint.X, panel.MidPoint.Y }); //Prepare the points for the KDTree
                pixelsForPanels.Add(panel, new List<double[]>()); //Prepare the dictionairy for results
            }

            //Create a new 2 dimensional tree to increase search time to O(log n) instead of O(n)
            var tree = new KDTree<double, DrawablePanel>(2, points.ToArray(), panels.ToArray(), EuclideanDistance);

            //For each area of 5x5 pixels, find the panel that is closest to it and save it
            for (var i = 0; i < _screenBounds.Width; i += 5)
            {
                for (var j = 0; j < _screenBounds.Height; j += 5)
                {
                    var closestPanel = tree.NearestNeighbors(new double[] { i, j }, 1);

                    pixelsForPanels[closestPanel[0].Item2].Add(closestPanel[0].Item1);
                }
            }

            //TODO: manhatten distance pakken zodat je rechthoeken uit het algoritme krijgt?
        }

        private void ConstructScreenshotAreas(List<DrawablePanel> panels)
        {
            LargestOrthogonalRectangleConstructor.Construct(panels.Select(p => new PointF((float)p.MidPoint.X, (float)p.MidPoint.Y)).ToList(), _screenBounds.Width, _screenBounds.Height);
        }
    }
}
