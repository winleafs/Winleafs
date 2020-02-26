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
using Winleafs.Models.Models.Layouts;

namespace Winleafs.Wpf.Api.Effects.ScreenMirrorEffects
{
    public class ScreenMirror : IScreenMirrorEffect
    {
        private readonly IExternalControlEndpoint _externalControlEndpoint;

        private readonly List<ScreenMirrorPanel> _panels;

        private readonly DeviceType _deviceType;

        public ScreenMirror(Orchestrator orchestrator, INanoleafClient nanoleafClient, ScaleType scaleType)
        {
            _externalControlEndpoint = nanoleafClient.ExternalControlEndpoint;
            _panels = new List<ScreenMirrorPanel>();
            _deviceType = orchestrator.PanelLayout.DeviceType;

            var screenBounds = ScreenBoundsHelper.GetScreenBounds(UserSettings.Settings.ScreenMirrorMonitorIndex);
            var panels = orchestrator.PanelLayout.GetScaledPolygons(screenBounds.Width, screenBounds.Height, scaleType);
            
            switch (_deviceType)
            {
                case DeviceType.Aurora:
                    LoadPanelsForTriangles(screenBounds, panels);
                    break;
                case DeviceType.Canvas:
                    LoadPanelsForSquares(screenBounds, panels);
                    break;
                default:
                    throw new NotImplementedException($"Screen mirror constructor for device of type {orchestrator.PanelLayout.DeviceType.ToString()} not implemented");
            }
        }

        /// <summary>
        /// Applies the screen mirror effect to the lights.
        /// This method is called 10x per second by <see cref="ScreenMirrorEffect"/>
        /// </summary>
        public async Task ApplyEffect()
        {
            const int numberOfPanelsPerIteration = 5; //5x10 = 50hz. Note: 50hz seems to work good, higher values can make Canvas stop its external control
            const int minimumColorDifference = 30;

            var panelsToUpdate = _panels.Take(numberOfPanelsPerIteration * 2).ToList(); //Take 2 times the number of panels, in case any color differences are not large enough

            var colors = ScreenGrabber.CalculateAverageColor(panelsToUpdate.Select(panel => panel.ScreenshotArea), 0);
            var numberOfPanelsChanged = 0;

            for (var i = 0; i < panelsToUpdate.Count; i++)
            {
                if (ColorDistance(panelsToUpdate[i].CurrentColor, colors[i]) > minimumColorDifference)
                {
                    numberOfPanelsChanged++;
                    panelsToUpdate[i].CurrentColor = colors[i];
                    _externalControlEndpoint.SetPanelColor(_deviceType, panelsToUpdate[i].PanelId, colors[i].R, colors[i].G, colors[i].B);

                    _panels.Remove(panelsToUpdate[i]); //Remove the current panel and place it at the back of the list
                    _panels.Add(panelsToUpdate[i]);
                }

                if (numberOfPanelsChanged >= numberOfPanelsPerIteration)
                {
                    break;
                }
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

                _panels.Add(new ScreenMirrorPanel
                {
                    PanelId = panel.PanelId,
                    ScreenshotArea = bounds,
                    CurrentColor = Color.Black
                });
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

                _panels.Add(new ScreenMirrorPanel
                {
                    PanelId = panel.PanelId,
                    ScreenshotArea = bounds,
                    CurrentColor = Color.Black
                });
            }
        }

        /// <summary>
        /// Calculates the distance from 0-100 between the
        /// two given colors
        /// </summary>
        /// <remarks>
        /// Code inspired by: https://www.compuphase.com/cmetric.htm
        /// </remarks>
        private double ColorDistance(Color color1, Color color2)
        {
            var rmean = (color1.R + color2.R) / 2;
            var r = color1.R - color2.R;
            var g = color1.G - color2.G;
            var b = color1.B - color2.B;
            return Math.Sqrt((((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8));
        }
    }
}
