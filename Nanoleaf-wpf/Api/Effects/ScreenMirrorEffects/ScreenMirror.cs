using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Winleafs.Api;
using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Wpf.Api.Layouts;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Api.Effects.ScreenMirrorEffects
{
    public class ScreenMirror : IScreenMirrorEffect
    {
        private readonly INanoleafClient _nanoleafClient;
        private readonly IExternalControlEndpoint _externalControlEndpoint;
        private readonly List<DrawablePanel> _panels;
        private readonly int _rectangleSize;

        public ScreenMirror(INanoleafClient nanoleafClient, Orchestrator orchestrator)
        {
            _nanoleafClient = nanoleafClient;

            _externalControlEndpoint = _nanoleafClient.ExternalControlEndpoint;

            var screenBounds = MonitorHelper.GetScreenBounds();

            _panels = orchestrator.PanelLayout.GetScaledTriangles(screenBounds.Width, screenBounds.Height);

            //Set the rectangle size to 1/3th of the length of a side of the triangle. TODO: test what size is best
            var triangle = _panels.FirstOrDefault().Polygon;
            _rectangleSize = (int)Math.Floor(Point.Subtract(triangle.Points[0], triangle.Points[1]).Length / 3);
        }

        /// <summary>
        /// Applies the screen mirror effect to the ligts.
        /// Screen mirror takes the average color of each triangle and applies it to that triangle
        /// </summary>
        public async Task ApplyEffect()
        {        
            foreach (var panel in _panels)
            {
                //For each panel, draw a rectangle around its midpoint, according to the set rectangle size
                //Then get the average color of that rectangle and apply the color to the panel

                var startX = (int)Math.Floor(panel.MidPoint.X - (_rectangleSize / 2));
                var startY = (int)Math.Floor(panel.MidPoint.Y - (_rectangleSize / 2));

                var color = ScreenGrabber.CalculateAverageColor(startX, startY, _rectangleSize, _rectangleSize);

                await _externalControlEndpoint.SetPanelColorAsync(panel.PanelId, color.R, color.G, color.B);
            }
        }
    }
}
