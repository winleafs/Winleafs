using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Winleafs.Api;
using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Wpf.Api.Layouts;
using Winleafs.Wpf.Helpers;
using Winleafs.Models.Models;
using System.Drawing;
using Winleafs.Models.Enums;

namespace Winleafs.Wpf.Api.Effects.ScreenMirrorEffects
{
    public class ScreenMirror : IScreenMirrorEffect
    {
        private readonly IExternalControlEndpoint _externalControlEndpoint;

        /// <summary>
        /// Collection of panel ids
        /// </summary>
        private readonly List<int> _panelIds;

        /// <summary>
        /// Collection of <see cref="Rectangle"/>s and each element
        /// corresponds to a panel id in <see cref="_panelIds"/>.
        /// Note that we chose to not put these in a dictionary for
        /// performance reasons.
        /// </summary>
        private readonly List<Rectangle> _screenshotAreas;

        public ScreenMirror(Device device, Orchestrator orchestrator, INanoleafClient nanoleafClient, ScaleType scaleType)
        {
            _externalControlEndpoint = nanoleafClient.ExternalControlEndpoint;
            _panelIds = new List<int>();
            _screenshotAreas = new List<Rectangle>();

            var screenBounds = ScreenBoundsHelper.GetScreenBounds(UserSettings.Settings.ScreenMirrorMonitorIndex);
            var panels = orchestrator.PanelLayout.GetScaledPolygons(screenBounds.Width, screenBounds.Height, scaleType);
            
            switch (orchestrator.PanelLayout.DeviceType)
            {
                case DeviceType.Triangles:
                    LoadPanelsForTriangles(screenBounds, panels);
                    break;
                case DeviceType.Squares:
                    LoadPanelsForSquares(screenBounds, panels);
                    break;
                default:
                    throw new NotImplementedException($"Screen mirror constructor for device of type {orchestrator.PanelLayout.DeviceType.ToString()} not implemented");
            }
        }

        /// <summary>
        /// Applies the screen mirror effect to the ligts.
        /// Screen mirror takes the average color of each triangle and applies it to that triangle
        /// </summary>
        public async Task ApplyEffect()
        {        
            var colors = ScreenGrabber.CalculateAverageColor(_screenshotAreas, 0);

            for (var i = 0; i < _panelIds.Count; i++)
            {
                _externalControlEndpoint.SetPanelColor(_panelIds[i], colors[i].R, colors[i].G, colors[i].B);
            }
        }

        private void LoadPanelsForTriangles(Rectangle screenBounds, List<DrawablePanel> panels)
        {
            //Set the square size to 1/3th of the length of a side of the triangle
            var triangle = panels.FirstOrDefault().Polygon;
            var squareSize = (int)Math.Floor(System.Windows.Point.Subtract(triangle.Points[0], triangle.Points[1]).Length / 3);

            foreach (var panel in panels)
            {
                //For each panel, draw a square around its midpoint, according to the set square size
                var startX = (int)Math.Floor(panel.MidPoint.X - (squareSize / 2));
                var startY = (int)Math.Floor(panel.MidPoint.Y - (squareSize / 2));

                // In multi monitor setup, all screens are joined in one larger pixel area. For example, if you want to take a screenshot of the second from left monitor,
                // you need to start at the right of the first left monitor. Hence, we need to add _screenBounds X and Y here to the location of the square we want to capture
                var bounds = new Rectangle(screenBounds.X + startX, screenBounds.Y + startY, squareSize, squareSize);

                _panelIds.Add(panel.PanelId);
                _screenshotAreas.Add(bounds);
            }
        }

        private void LoadPanelsForSquares(Rectangle screenBounds, List<DrawablePanel> panels)
        {
            //Set the square size to half of the length of a side of the square,
            //We do this since the squares can be placed in a diamond shape, then the largest square that can be drawn in such a diamond shape
            //is a square half the length of a side of the square
            var square = panels.FirstOrDefault().Polygon;
            var rectangleSize = (int)Math.Floor(System.Windows.Point.Subtract(square.Points[0], square.Points[1]).Length / 2);

            foreach (var panel in panels)
            {
                //For each panel, draw a square around its midpoint, according to the set square size
                var startX = (int)Math.Floor(panel.MidPoint.X - (rectangleSize / 2));
                var startY = (int)Math.Floor(panel.MidPoint.Y - (rectangleSize / 2));

                // In multi monitor setup, all screens are joined in one larger pixel area. For example, if you want to take a screenshot of the second from left monitor,
                // you need to start at the right of the first left monitor. Hence, we need to add _screenBounds X and Y here to the location of the square we want to capture
                var bounds = new Rectangle(screenBounds.X + startX, screenBounds.Y + startY, rectangleSize, rectangleSize);

                _panelIds.Add(panel.PanelId);
                _screenshotAreas.Add(bounds);
            }
        }
    }
}
