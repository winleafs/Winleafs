using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Winleafs.Api;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Layouts;

namespace Winleafs.Wpf.Api.Layouts
{
    public class PanelLayout
    {
        private static readonly SolidColorBrush _borderColor = (SolidColorBrush)Application.Current.FindResource("NanoleafBlack");
        private static readonly int _defaultTriangleSize = 148; //The default size in pixels of a triangle side as reported by the Nanoleaf API

        private INanoleafClient _nanoleafClient;

        private Layout _layout;

        private GlobalOrientation _globalOrientation;

        /// <summary>
        /// Panels represented by the Nanoleaf coordinate system, unscaled and therefore unsuitable for display
        /// </summary>
        private List<DrawablePanel> _unscaledTriangles { get; set; }

        public PanelLayout(Device device)
        {
            _nanoleafClient = NanoleafClient.GetClientForDevice(device);

            _unscaledTriangles = new List<DrawablePanel>();

            GetLayout();
        }

        public void GetLayout()
        {
            _layout = _nanoleafClient.LayoutEndpoint.GetLayout();
            _globalOrientation = _nanoleafClient.LayoutEndpoint.GetGlobalOrientation();

            if (_layout != null)
            {
                ConstructPanelsAsTriangles();
            }
        }

        private void ConstructPanelsAsTriangles()
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
                CreateTriangle(panelPosition.X, panelPosition.Y, panelPosition.Orientation, panelPosition.PanelId, globalRotationTransform);
            }

            //Normalize the triangle positions such that the coordinates start at 0
            double minTriangleX = _unscaledTriangles.SelectMany(p => p.Polygon.Points).Min(p => p.X);
            double minTriangleY = _unscaledTriangles.SelectMany(p => p.Polygon.Points).Min(p => p.Y);

            foreach (var triangle in _unscaledTriangles)
            {
                //Move MidPoint
                triangle.MidPoint = new Point(triangle.MidPoint.X - minTriangleX, triangle.MidPoint.Y - minTriangleY);

                //Move triangle
                for (var i = 0; i < triangle.Polygon.Points.Count; i++)
                {
                    var x = triangle.Polygon.Points[i].X - minTriangleX;
                    var y = triangle.Polygon.Points[i].Y - minTriangleY;

                    triangle.Polygon.Points[i] = new Point(x, y);
                }
            }
        }

        /// <summary>
        /// Draws an equilateral triangle from the given center point and rotation. Also applies the global rotation
        /// </summary>
        private void CreateTriangle(double x, double y, double rotation, int panelId, Transform globalRotationTransform)
        {
            //First assume that we draw the triangle facing up:
            //     A
            //    /\
            //   /  \
            //  /____\
            // B      C

            var A = new Point(x, y - ((Math.Sqrt(3) / 3) * _defaultTriangleSize));
            var B = new Point(x - (_defaultTriangleSize / 2), y + ((Math.Sqrt(3) / 6) * _defaultTriangleSize));
            var C = new Point(x + (_defaultTriangleSize / 2), y + ((Math.Sqrt(3) / 6) * _defaultTriangleSize));

            var rotateTransform = new RotateTransform(rotation, x, y);

            //Apply transformation and add points to polygon
            var triangle = new Polygon();
            triangle.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(A)));
            triangle.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(B)));
            triangle.Points.Add(globalRotationTransform.Transform(rotateTransform.Transform(C)));

            triangle.Stroke = _borderColor;
            triangle.HorizontalAlignment = HorizontalAlignment.Left;
            triangle.VerticalAlignment = VerticalAlignment.Top;
            triangle.StrokeThickness = 2;

            _unscaledTriangles.Add(new DrawablePanel
            {
                MidPoint = globalRotationTransform.Transform(new Point(x, y)),
                PanelId = panelId,
                Polygon = triangle
            });
        }

        /// <summary>
        /// Returns the panels represented as <see cref="DrawablePanel"/>s scaled to fit the desired width and height.
        /// Returns null if there is no layout
        /// </summary>
        public List<DrawablePanel> GetScaledTriangles(int width, int height, ScaleType scaleType = ScaleType.Fit)
        {
            if (_layout == null)
            {
                GetLayout(); // Try to retrieve the layout one more time

                if (_layout == null) // Layout retrieval failed, return null
                {
                    return null;
                }
            }

            var maxX = _unscaledTriangles.SelectMany(p => p.Polygon.Points).Max(p => p.X);
            var maxY = _unscaledTriangles.SelectMany(p => p.Polygon.Points).Max(p => p.Y);

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

            var scaledTriangles = new List<DrawablePanel>();

            foreach (var panel in _unscaledTriangles)
            {
                var polygon = new Polygon();
                foreach (var point in panel.Polygon.Points)
                {
                    polygon.Points.Add(scaleTransform.Transform(point));
                }

                scaledTriangles.Add(new DrawablePanel
                {
                    MidPoint = scaleTransform.Transform(panel.MidPoint),
                    PanelId = panel.PanelId,
                    Polygon = polygon
                });
            }

            return scaledTriangles;
        }
    }
}
