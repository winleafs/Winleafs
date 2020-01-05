using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Winleafs.Api;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Layouts;

namespace Winleafs.Wpf.Api.Layouts
{
    public class PanelLayout
    {
        private static readonly SolidColorBrush _borderColor = (SolidColorBrush)Application.Current.FindResource("NanoleafBlack");

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
            _nanoleafClient = NanoleafClient.GetClientForDevice(device);

            _unscaledPolygons = new List<DrawablePanel>();

            GetLayout();
        }

        public void GetLayout()
        {
            _layout = _nanoleafClient.LayoutEndpoint.GetLayout();
            _globalOrientation = _nanoleafClient.LayoutEndpoint.GetGlobalOrientation();

            if (_layout != null)
            {
                ConstructPanelsAsPolygons();

                //Set the device type according to the type of panels
                switch (_layout.PanelPositions.ElementAt(0).ShapeType)
                {
                    case ShapeType.ContolSquarePassive:
                    case ShapeType.ControlSquarePrimary:
                    case ShapeType.Square:
                        DeviceType = DeviceType.Canvas;
                        break;
                    case ShapeType.Triangle:
                        DeviceType = DeviceType.Aurora;
                        break;
                }
            }
        }

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

            //Create the triangles
            foreach (var panelPosition in _layout.PanelPositions)
            {
                CreatePolygon(panelPosition.X, panelPosition.Y, panelPosition.Orientation, panelPosition.PanelId, globalRotationTransform, panelPosition.ShapeType);
            }

            //Normalize the triangle positions such that the coordinates start at 0
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
        /// Draws an equilateral polygon from the given center point and rotation. Also applies the global rotation
        /// </summary>
        private void CreatePolygon(double x, double y, double rotation, int panelId,
            Transform globalRotationTransform, ShapeType shapeType)
        {
            var rotateTransform = new RotateTransform(rotation, x, y);

            //Apply transformation and add points to polygon
            var polygon = new Polygon();

            switch (shapeType)
            {
                case ShapeType.Triangle:
                {
                    //First assume that we draw the triangle facing up:
                    //     A
                    //    /\
                    //   /  \
                    //  /____\
                    // B      C

                    var triangleSize = _layout.SideLength;
                    var a = new Point(x, y - ((Math.Sqrt(3) / 3) * triangleSize));
                    var b = new Point(x - (triangleSize / 2), y + ((Math.Sqrt(3) / 6) * triangleSize));
                    var c = new Point(x + (triangleSize / 2), y + ((Math.Sqrt(3) / 6) * triangleSize));

                    polygon.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(a)));
                    polygon.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(b)));
                    polygon.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(c)));
                    break;
                }
                case ShapeType.ControlSquarePrimary:
                case ShapeType.ContolSquarePassive:
                case ShapeType.Square:
                {
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
                    var distanceToCenter = _layout.SideLength / (double)2;
                    var a = new Point(x - distanceToCenter, y + distanceToCenter);
                    var b = new Point(x - distanceToCenter, y - distanceToCenter);
                    var c = new Point(x + distanceToCenter, y - distanceToCenter);
                    var d = new Point(x + distanceToCenter, y + distanceToCenter);
                    polygon.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(a)));
                    polygon.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(b)));
                    polygon.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(c)));
                    polygon.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(d)));
                    break;
                }
            }

            polygon.Stroke = _borderColor;
            polygon.HorizontalAlignment = HorizontalAlignment.Left;
            polygon.VerticalAlignment = VerticalAlignment.Top;
            polygon.StrokeThickness = 2;

            _unscaledPolygons.Add(new DrawablePanel
            {
                MidPoint = globalRotationTransform.Transform(new Point(x, y)),
                PanelId = panelId,
                Polygon = polygon
            });
        }

        /// <summary>
        /// Returns the panels represented as <see cref="DrawablePanel"/>s scaled to fit the desired width and height.
        /// Returns null if there is no layout
        /// </summary>
        public List<DrawablePanel> GetScaledPolygons(int width, int height, ScaleType scaleType = ScaleType.Fit)
        {
            if (_layout == null)
            {
                GetLayout(); // Try to retrieve the layout one more time

                if (_layout == null) // Layout retrieval failed, return null
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
                    polygon.Points.Add(scaleTransform.Transform(point));
                }

                scaledPolygons.Add(new DrawablePanel
                {
                    MidPoint = scaleTransform.Transform(panel.MidPoint),
                    PanelId = panel.PanelId,
                    Polygon = polygon
                });
            }

            return scaledPolygons;
        }
    }
}
