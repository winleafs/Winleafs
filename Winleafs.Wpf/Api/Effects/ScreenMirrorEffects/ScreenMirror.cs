using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Winleafs.Api;
using Winleafs.Api.Endpoints.Interfaces;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Wpf.Api.Layouts;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Api.Effects.ScreenMirrorEffects
{
    public class ScreenMirror : IScreenMirrorEffect
    {
        private readonly IExternalControlEndpoint _externalControlEndpoint;

        private readonly List<Rectangle> _panelAreas;
        private readonly List<int> _panelIds;

        private readonly DeviceType _deviceType;

        public ScreenMirror(Orchestrator orchestrator, INanoleafClient nanoleafClient, ScaleType scaleType, FlipType flipType)
        {
            _externalControlEndpoint = nanoleafClient.ExternalControlEndpoint;
            _panelAreas = new List<Rectangle>();
            _panelIds = new List<int>();
            _deviceType = orchestrator.PanelLayout.DeviceType;

            var screenBounds = ScreenBoundsHelper.GetScreenBounds(UserSettings.Settings.ScreenMirrorMonitorIndex);
            var panels = orchestrator.PanelLayout.GetScaledPolygons(screenBounds.Width, screenBounds.Height, scaleType, flipType);
            
            switch (_deviceType)
            {
                case DeviceType.Aurora:
                    LoadPanelsForTriangles(panels);
                    break;
                case DeviceType.Canvas:
                    LoadPanelsForSquares(panels);
                    break;
                default:
                    throw new NotImplementedException($"Screen mirror constructor for device of type {orchestrator.PanelLayout.DeviceType} not implemented");
            }
        }

        /// <summary>
        /// Applies the screen mirror effect to the lights.
        /// This method is called X times per second by <see cref="ScreenMirrorEffect"/>
        /// </summary>
        public async Task ApplyEffect()
        {
            var colors = ScreenGrabber.CalculateAverageColor(_panelAreas, 0);

            if (colors == null)
            {
                //This can happen when before the first screen shot is taken when the effect is enabled
                return;
            }

            _externalControlEndpoint.SetPanelsColors(_deviceType, _panelIds, colors);
        }

        private void LoadPanelsForTriangles(List<DrawablePanel> panels)
        {
            //Set the square size to 1/3th of the length of a side of the triangle
            var triangle = panels.FirstOrDefault().Polygon;
            var squareSize = (int)Math.Floor(System.Windows.Point.Subtract(triangle.Points[0], triangle.Points[1]).Length / 3);

            foreach (var panel in panels)
            {
                //For each panel, draw a square around its midpoint, according to the set square size
                var startX = (int)Math.Floor(panel.MidPoint.X - (squareSize / 2));
                var startY = (int)Math.Floor(panel.MidPoint.Y - (squareSize / 2));

                //Use the absolute coordinates (starting at 0,0) instead of relative coordinates (starting at screenbounds.X, screenbounds.Y), since bitmaps start at 0,0 even if we capture a secondary monitor
                var bitmapArea = new Rectangle(startX, startY, squareSize, squareSize);

                _panelAreas.Add(bitmapArea);
                _panelIds.Add(panel.PanelId);
            }
        }

        private void LoadPanelsForSquares(List<DrawablePanel> panels)
        {
            //Set the square size to half of the length of a side of the square,
            //We do this since the squares can be placed in a diamond shape, then the largest square that can be drawn in such a diamond shape
            //is a square half the length of a side of the square
            var square = panels.FirstOrDefault().Polygon;
            var rectangleSize = (int)Math.Floor(System.Windows.Point.Subtract(square.Points[0], square.Points[1]).Length / 2);

            foreach (var panel in panels)
            {
                //For each panel, draw a square around its midpoint, according to the set rectangle size
                var startX = (int)Math.Floor(panel.MidPoint.X - (rectangleSize / 2));
                var startY = (int)Math.Floor(panel.MidPoint.Y - (rectangleSize / 2));

                //Use the absolute coordinates (starting at 0,0) instead of relative coordinates (starting at screenbounds.X, screenbounds.Y), since bitmaps start at 0,0 even if we capture a secondary monitor
                var bitmapArea = new Rectangle(startX, startY, rectangleSize, rectangleSize);

                _panelAreas.Add(bitmapArea);
                _panelIds.Add(panel.PanelId);
            }
        }
    }
}
