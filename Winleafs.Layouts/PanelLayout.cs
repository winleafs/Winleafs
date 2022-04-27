using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Winleafs.Models;
using Winleafs.Models.Enums;
using Winleafs.Models.Layouts;
using Winleafs.Nanoleaf;

namespace Winleafs.Layouts
{
    public class PanelLayout
    {
        private static readonly SolidColorBrush _borderColor = (SolidColorBrush)System.Windows.Application.Current.FindResource("NanoleafBlack");

        private INanoleafClient _nanoleafClient;

        private Layout _layout;

        private GlobalOrientation _globalOrientation;

        /// <summary>
        /// Panels represented by the Nanoleaf coordinate system, unscaled and therefore unsuitable for display
        /// </summary>
        private List<DrawablePanel> _unscaledPolygons { get; set; }

        public DeviceType DeviceType { get; set; }

        public PanelLayout(Device device)
        {
            _nanoleafClient = NanoleafClientFactory.Create(device);

            _unscaledPolygons = new List<DrawablePanel>();

            GetLayout();
        }

        public void GetLayout()
        {
            _layout = _nanoleafClient.LayoutEndpoint.GetLayout();
            _globalOrientation = _nanoleafClient.LayoutEndpoint.GetGlobalOrientation();

            if (_layout == null || _layout.NumberOfPanels <= 0)
            {
                return;
            }

            ConstructPanelsAsPolygons();

            //Set the device type according to the type of panels (which panel type occurs most)
            switch (_layout.PanelPositions.GroupBy(panel => panel.ShapeType).OrderByDescending(group => group.Count()).First().Key)
            {
                case ShapeType.ContolSquarePassive:
                case ShapeType.ControlSquarePrimary:
                case ShapeType.Square:
                    DeviceType = DeviceType.Canvas;
                    break;
                case ShapeType.Triangle:
                    DeviceType = DeviceType.Aurora;
                    break;
                case ShapeType.ShapeHexagon:
                case ShapeType.ShapeTriangle:
                case ShapeType.ShapeMiniTriangle:
                    DeviceType = DeviceType.Shapes;
                    break;
                default:
                    DeviceType = DeviceType.Unknown;
                    break;
            }
        }

        #region Polygon construction
        private void ConstructPanelsAsPolygons()
        {
            //Reverse every Y coordinate since Y = 0 means top for the canvas but for Nanoleaf it means bottom
            foreach (var panelPosition in _layout.PanelPositions)
            {
                panelPosition.Y = panelPosition.Y * -1;
            }

            //Calculate the transform for the global orientation. All panels should rotate over the center of all panels
            var minX = _layout.PanelPositions.Min(pp => pp.X);
            var maxX = _layout.PanelPositions.Max(pp => pp.X);
            var minY = _layout.PanelPositions.Min(pp => pp.Y);
            var maxY = _layout.PanelPositions.Max(pp => pp.Y);

            var globalRotationTransform = new RotateTransform(_globalOrientation.Value, Math.Abs(maxX - minX) / 2, Math.Abs(maxY - minY) / 2);

            //Create the polygons
            foreach (var panelPosition in _layout.PanelPositions)
            {
                CreatePolygon(panelPosition.X, panelPosition.Y, panelPosition.Orientation, panelPosition.PanelId, globalRotationTransform, panelPosition.ShapeType);
            }

            //Can be empty in theory, when a device only exists of a power supply for example
            if (!_unscaledPolygons.Any())
            {
                return;
            }

            //Normalize the polygon positions such that the coordinates start at 0
            double minPolygonX = _unscaledPolygons.SelectMany(p => p.Polygon.Points).Min(p => p.X);
            double minPolygonY = _unscaledPolygons.SelectMany(p => p.Polygon.Points).Min(p => p.Y);

            foreach (var polygon in _unscaledPolygons)
            {
                //Move MidPoint
                polygon.MidPoint = new Point(polygon.MidPoint.X - minPolygonX, polygon.MidPoint.Y - minPolygonY);

                //Move polygon
                for (var i = 0; i < polygon.Polygon.Points.Count; i++)
                {
                    var x = polygon.Polygon.Points[i].X - minPolygonX;
                    var y = polygon.Polygon.Points[i].Y - minPolygonY;

                    polygon.Polygon.Points[i] = new Point(x, y);
                }
            }
        }

        /// <summary>
        /// Draws a polygon based on the shape type from the given center point and rotation. Also applies the global rotation
        /// </summary>
        private void CreatePolygon(double x, double y, double rotation, int panelId,
            Transform globalRotationTransform, ShapeType shapeType)
        {
            var rotateTransform = new RotateTransform(rotation, x, y);

            //Apply transformation and add points to polygon
            Polygon polygon;

            switch (shapeType)
            {
                case ShapeType.Triangle:
                    {
                        polygon = CreateRotatedTriangle(x, y, rotateTransform, globalRotationTransform, 150);
                        break;
                    }
                case ShapeType.ShapeTriangle:
                    {
                        //Shape triangles are slighty smaller than original triangles
                        polygon = CreateRotatedTriangle(x, y, rotateTransform, globalRotationTransform, 135);
                        break;
                    }
                case ShapeType.ShapeMiniTriangle:
                    {
                        polygon = CreateRotatedTriangle(x, y, rotateTransform, globalRotationTransform, 68);
                        break;
                    }
                case ShapeType.ControlSquarePrimary:
                case ShapeType.ContolSquarePassive:
                case ShapeType.Square:
                    {
                        polygon = CreateRotatedSquare(x, y, rotateTransform, globalRotationTransform);
                        break;
                    }

                case ShapeType.ShapeHexagon:
                    {
                        polygon = CreateRotatedHexagon(x, y, rotateTransform, globalRotationTransform);
                        break;
                    }
                // If the type is unknown or an unsupported shape (e.g. power supply), return instead of making a wrong polygon
                default:
                    return;
            }

            polygon.Stroke = _borderColor;
            polygon.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            polygon.VerticalAlignment = VerticalAlignment.Top;
            polygon.StrokeThickness = 2;

            _unscaledPolygons.Add(new DrawablePanel
            {
                MidPoint = globalRotationTransform.Transform(new Point(x, y)),
                PanelId = panelId,
                Polygon = polygon
            });
        }

        private Polygon CreateRotatedTriangle(double x, double y, RotateTransform rotateTransform, Transform globalRotationTransform, int triangleSideLength)
        {
            var polygon = new Polygon();

            //First assume that we draw the triangle facing up:
            //     A
            //    /\
            //   /  \
            //  /____\
            // B      C

            //const int triangleSideLength = 150;
            var a = new Point(x, y - ((Math.Sqrt(3) / 3) * triangleSideLength));
            var b = new Point(x - (triangleSideLength / 2), y + ((Math.Sqrt(3) / 6) * triangleSideLength));
            var c = new Point(x + (triangleSideLength / 2), y + ((Math.Sqrt(3) / 6) * triangleSideLength));

            //Then rotate the 3 points
            polygon.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(a)));
            polygon.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(b)));
            polygon.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(c)));

            return polygon;
        }

        private Polygon CreateRotatedSquare(double x, double y, RotateTransform rotateTransform, Transform globalRotationTransform)
        {
            var polygon = new Polygon();

            /*
            * a------------d
            * |            |
            * |            |
            * |     xy     |
            * |            |
            * |            |
            * b------------c
            *
            * The X and Y positions are given as if they are in the
            * middle of the square that should be created.
            * For that reason we always need to edit the X and Y position
            * to create a correct square.
            *
            * The Nanoleaf API returns the SideLength as the full length
            * of the sides of the square. Because the x and y are in the middle
            * Only half of the side length should be either add or removed
            * from the X and Y
            */

            // This is the distance from one of the corners to the center.
            const int squareSideLength = 100;
            var distanceToCenter = squareSideLength / (double)2;
            var a = new Point(x - distanceToCenter, y + distanceToCenter);
            var b = new Point(x - distanceToCenter, y - distanceToCenter);
            var c = new Point(x + distanceToCenter, y - distanceToCenter);
            var d = new Point(x + distanceToCenter, y + distanceToCenter);
            polygon.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(a)));
            polygon.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(b)));
            polygon.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(c)));
            polygon.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(d)));

            return polygon;
        }

        private Polygon CreateRotatedHexagon(double x, double y, RotateTransform rotateTransform, Transform globalRotationTransform)
        {
            var polygon = new Polygon();

            /*      ____
             *     /    \
             *    /      \
             *    |       |
             *    |       |
             *    \      /
             *     \____/
             *
             * A hexagon has 6 sides. Use a loop to draw a hexagon with the x,y midpoint
             * and its side length.
             */
            const int hexagonSideLength = 67;
            for (var currentPoint = 0; currentPoint < 6; currentPoint++)
            {
                var point = new Point(
                    x + hexagonSideLength * (float)Math.Cos(currentPoint * 60 * Math.PI / 180f),
                    y + hexagonSideLength * (float)Math.Sin(currentPoint * 60 * Math.PI / 180f)
                    );
                polygon.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(point)));
            }

            return polygon;
        }
        #endregion

        /// <summary>
        /// Returns the panels represented as <see cref="DrawablePanel"/>s scaled to fit the desired width and height.
        /// Returns null if there is no layout
        /// </summary>
        public List<DrawablePanel> GetScaledPolygons(int width, int height, ScaleType scaleType, FlipType flipType)
        {
            if (_layout == null || !_unscaledPolygons.Any())
            {
                GetLayout(); // Try to retrieve the layout one more time

                if (_layout == null || !_unscaledPolygons.Any()) // Layout retrieval failed, return null
                {
                    return null;
                }
            }

            var maxX = _unscaledPolygons.SelectMany(p => p.Polygon.Points).Max(p => p.X);
            var maxY = _unscaledPolygons.SelectMany(p => p.Polygon.Points).Max(p => p.Y);

            var scaleX = width / maxX;
            var scaleY = height / maxY;

            ScaleTransform scaleTransform;

            if (scaleType == ScaleType.Fit)
            {
                var scale = Math.Min(scaleX, scaleY);
                scaleTransform = new ScaleTransform(scale, scale);
            }
            else //scaleType == Stretch
            {
                scaleTransform = new ScaleTransform(scaleX, scaleY);
            }

            var scaledPolygons = new List<DrawablePanel>();

            foreach (var panel in _unscaledPolygons)
            {
                var polygon = new Polygon();
                foreach (var point in panel.Polygon.Points)
                {
                    var scaledPoint = scaleTransform.Transform(point);
                    var flippedPoint = FlipTransform(scaledPoint, flipType, width, height);
                    polygon.Points.Add(flippedPoint);
                }

                var scaledMidPoint = scaleTransform.Transform(panel.MidPoint);
                var flippedMidPoint = FlipTransform(scaledMidPoint, flipType, width, height);

                scaledPolygons.Add(new DrawablePanel
                {
                    MidPoint = flippedMidPoint,
                    PanelId = panel.PanelId,
                    Polygon = polygon
                });
            }

            return scaledPolygons;
        }

        private Point FlipTransform(Point point, FlipType flipType, double maxX, double maxY)
        {
            switch (flipType)
            {
                case FlipType.Horizontal:
                    point.X = maxX - point.X;
                    break;
                case FlipType.Vertical:
                    point.Y = maxY - point.Y;
                    break;
                case FlipType.HorizontalVertical:
                    point.X = maxX - point.X;
                    point.Y = maxY - point.Y;
                    break;
                case FlipType.None:
                    break; //Do nothing
                default:
                    throw new NotImplementedException($"Flipping for {nameof(FlipType)}.{flipType} not implemented.");
            }

            return point;
        }
    }
}
