using Winleafs.Models;
using Winleafs.Models.Enums;
using Winleafs.Nanoleaf;
using Winleafs.Nanoleaf.Endpoints.Interfaces;
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

        public ScreenMirror(PanelLayout panelLayout, INanoleafClient nanoleafClient, ScaleType scaleType, FlipType flipType)
        {
            _externalControlEndpoint = nanoleafClient.ExternalControlEndpoint;
            _panelAreas = new List<Rectangle>();
            _panelIds = new List<int>();
            _deviceType = panelLayout.DeviceType;

            var screenBounds = ScreenBoundsHelper.GetScreenBounds(UserSettings.Settings.ScreenMirrorMonitorIndex);
            var panels = panelLayout.GetScaledPolygons(screenBounds.Width, screenBounds.Height, scaleType, flipType);

            switch (_deviceType)
            {
                case DeviceType.Aurora:
                    LoadPanelsForTriangles(panels);
                    break;
                case DeviceType.Canvas:
                    LoadPanelsForSquares(panels);
                    break;
                case DeviceType.Shapes:
                    LoadPanelsForShapes(panels);
                    break;
                default:
                    throw new NotImplementedException($"Screen mirror constructor for device of type {panelLayout.DeviceType} not implemented");
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

        //Shapes can be either a triangle or a hexagon, depending on the number of points in the polygon
        //(do not rely on Enums.ShapeType since then we would have to handle many cases)
        private void LoadPanelsForShapes(List<DrawablePanel> panels)
        {
            foreach (var panel in panels)
            {
                var polygon = panel.Polygon;

                int squareSize;
                if (polygon.Points.Count == 6)
                {
                    //It is a hexagon
                    //We get the maximum intersection length of a polygon by getting the distance between the first and fourth point
                    //Then divide by two to not go over the boundaries of the hexagon
                    squareSize = (int)Math.Floor(System.Windows.Point.Subtract(polygon.Points[0], polygon.Points[3]).Length / 2);
                }
                else
                {
                    //It is a triangle
                    //Set the square size to 1/3th of the length of a side of the triangle
                    squareSize = (int)Math.Floor(System.Windows.Point.Subtract(polygon.Points[0], polygon.Points[1]).Length / 3);
                }

                //For each panel, draw a square around its midpoint, according to the set square size
                var startX = (int)Math.Floor(panel.MidPoint.X - (squareSize / 2));
                var startY = (int)Math.Floor(panel.MidPoint.Y - (squareSize / 2));

                //Use the absolute coordinates (starting at 0,0) instead of relative coordinates (starting at screenbounds.X, screenbounds.Y), since bitmaps start at 0,0 even if we capture a secondary monitor
                var bitmapArea = new Rectangle(startX, startY, squareSize, squareSize);

                _panelAreas.Add(bitmapArea);
                _panelIds.Add(panel.PanelId);
            }
        }
    }
}
