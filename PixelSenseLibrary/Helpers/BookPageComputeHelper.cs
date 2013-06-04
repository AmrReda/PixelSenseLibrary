using System;
using System.Windows;
using System.Windows.Media;
using PixelSenseLibrary.Controls.ItemsCollections;
using PixelSenseLibrary.Enums;
using PixelSenseLibrary.Structs;

namespace PixelSenseLibrary.Helpers
{
    public static class BookPageComputeHelper
    {
        // Fields
        private static double s_dEPSILON = 0.5;
        private static int s_nANIMATION_DURATION = 500;

        // Methods
        public static void CheckParams(ref BookPage source, ref Point p, CornerOrigin origin)
        {
            switch (origin)
            {
                case CornerOrigin.TopLeft:
                    p.X = source.RenderSize.Width - p.X;
                    CheckParams(ref source, ref p, CornerOrigin.TopRight);
                    p.X = source.RenderSize.Width - p.X;
                    return;

                case CornerOrigin.TopRight:
                    if (origin != CornerOrigin.TopRight)
                    {
                        break;
                    }
                    if (p.Y == 0.0)
                    {
                        p.Y = s_dEPSILON;
                    }
                    if (p.X == 0.0)
                    {
                        p.X = s_dEPSILON;
                    }
                    if (p.Y > 0.0)
                    {
                        double num = Math.Sqrt((p.X * p.X) + (p.Y * p.Y));
                        if (num > source.RenderSize.Width)
                        {
                            double num2 = (p.X * source.RenderSize.Width) / num;
                            double num3 = (p.Y * source.RenderSize.Width) / num;
                            p.X = num2;
                            p.Y = num3;
                        }
                    }
                    else
                    {
                        Math.Sqrt((p.X * p.X) + (p.Y * p.Y));
                        double num4 = Math.Sqrt((source.RenderSize.Width * source.RenderSize.Width) + (source.RenderSize.Height * source.RenderSize.Height));
                        double num5 = ((p.Y * p.Y) / (p.X * p.X)) + 1.0;
                        double num6 = ((-2.0 * p.Y) * source.RenderSize.Height) / p.X;
                        double num7 = (source.RenderSize.Height * source.RenderSize.Height) - (num4 * num4);
                        double d = (num6 * num6) - ((4.0 * num5) * num7);
                        double num9 = 0.0;
                        if (p.X > 0.0)
                        {
                            num9 = (-num6 + Math.Sqrt(d)) / (2.0 * num5);
                        }
                        else
                        {
                            num9 = (-num6 - Math.Sqrt(d)) / (2.0 * num5);
                        }
                        double num10 = (p.Y * num9) / p.X;
                        if (Math.Abs(num9) < Math.Abs(p.X))
                        {
                            p.X = num9;
                            p.Y = num10;
                        }
                    }
                    if ((source.RenderSize.Width - p.X) != p.Y)
                    {
                        break;
                    }
                    p.X += s_dEPSILON;
                    return;

                case CornerOrigin.BottomLeft:
                    p.Y = source.RenderSize.Height - p.Y;
                    p.X = source.RenderSize.Width - p.X;
                    CheckParams(ref source, ref p, CornerOrigin.TopRight);
                    p.Y = source.RenderSize.Height - p.Y;
                    p.X = source.RenderSize.Width - p.X;
                    break;

                case CornerOrigin.BottomRight:
                    p.Y = source.RenderSize.Height - p.Y;
                    CheckParams(ref source, ref p, CornerOrigin.TopRight);
                    p.Y = source.RenderSize.Height - p.Y;
                    return;

                default:
                    return;
            }
        }

        public static int ComputeAnimationDuration(BookPage source, Point p, CornerOrigin origin)
        {
            double num = ComputeProgressRatio(source, p, origin);
            int num2 = Convert.ToInt32((double)(s_nANIMATION_DURATION * ((num / 2.0) + 0.5)));
            if (num2 <= 10)
            {
                num2 = 10;
            }
            return num2;
        }

        public static PageParameters? ComputePage(BookPage source, Point p, CornerOrigin origin)
        {
            Point point;
            Point point2;
            Point point3;
            Point point4;
            CheckParams(ref source, ref p, origin);
            PageParameters parameters = new PageParameters(source.RenderSize);
            double num = ComputeProgressRatio(source, p, origin);
            if (num > 1.5)
            {
                num = (2.0 - num) / 0.5;
            }
            else
            {
                num = 1.0;
            }
            parameters.Page0ShadowOpacity = num;
            double width = source.RenderSize.Width;
            double height = source.RenderSize.Height;
            switch (origin)
            {
                case CornerOrigin.TopLeft:
                    p.X = width - p.X;
                    p.Y = height - p.Y;
                    break;

                case CornerOrigin.TopRight:
                    p.Y = height - p.Y;
                    break;

                case CornerOrigin.BottomLeft:
                    p.X = width - p.X;
                    break;
            }
            if (p.X >= width)
            {
                return null;
            }
            double num4 = -(p.Y - height) / (p.X - width);
            double x = ((width + p.X) / 2.0) - (num4 * ((height + p.Y) / 2.0));
            double y = (width - x) / num4;
            double num7 = (num4 * height) + x;
            double num8 = (Math.Atan((width - p.X) / (y - p.Y)) * 180.0) / 3.1415926535897931;
            if ((num4 < 0.0) && (p.Y < y))
            {
                num8 -= 180.0;
            }
            switch (origin)
            {
                case CornerOrigin.TopLeft:
                    parameters.Page1RotateAngle = -num8;
                    parameters.Page1RotateCenterX = width - p.X;
                    parameters.Page1RotateCenterY = height - p.Y;
                    parameters.Page1TranslateX = -p.X;
                    parameters.Page1TranslateY = height - p.Y;
                    break;

                case CornerOrigin.TopRight:
                    parameters.Page1RotateAngle = num8;
                    parameters.Page1RotateCenterX = p.X;
                    parameters.Page1RotateCenterY = height - p.Y;
                    parameters.Page1TranslateX = p.X;
                    parameters.Page1TranslateY = height - p.Y;
                    break;

                case CornerOrigin.BottomLeft:
                    parameters.Page1RotateAngle = num8;
                    parameters.Page1RotateCenterX = width - p.X;
                    parameters.Page1RotateCenterY = p.Y;
                    parameters.Page1TranslateX = -p.X;
                    parameters.Page1TranslateY = p.Y - height;
                    break;

                case CornerOrigin.BottomRight:
                    parameters.Page1RotateAngle = -num8;
                    parameters.Page1RotateCenterX = p.X;
                    parameters.Page1RotateCenterY = p.Y;
                    parameters.Page1TranslateX = p.X;
                    parameters.Page1TranslateY = p.Y - height;
                    break;
            }
            switch (origin)
            {
                case CornerOrigin.TopLeft:
                    if (num8 >= 0.0)
                    {
                        parameters.Page1ClippingFigure.StartPoint = new Point(width, height);
                        parameters.Page1ClippingFigure.Segments.Clear();
                        parameters.Page1ClippingFigure.Segments.Add(new LineSegment(new Point(x, height), false));
                        parameters.Page1ClippingFigure.Segments.Add(new LineSegment(new Point(width, height - y), false));
                        break;
                    }
                    parameters.Page1ClippingFigure.StartPoint = new Point(width, 0.0);
                    parameters.Page1ClippingFigure.Segments.Clear();
                    parameters.Page1ClippingFigure.Segments.Add(new LineSegment(new Point(num7, 0.0), false));
                    parameters.Page1ClippingFigure.Segments.Add(new LineSegment(new Point(width, height - y), false));
                    break;

                case CornerOrigin.TopRight:
                    if (num8 >= 0.0)
                    {
                        parameters.Page1ClippingFigure.StartPoint = new Point(0.0, height);
                        parameters.Page1ClippingFigure.Segments.Clear();
                        parameters.Page1ClippingFigure.Segments.Add(new LineSegment(new Point(width - x, height), false));
                        parameters.Page1ClippingFigure.Segments.Add(new LineSegment(new Point(0.0, height - y), false));
                        break;
                    }
                    parameters.Page1ClippingFigure.StartPoint = new Point(0.0, 0.0);
                    parameters.Page1ClippingFigure.Segments.Clear();
                    parameters.Page1ClippingFigure.Segments.Add(new LineSegment(new Point(width - num7, 0.0), false));
                    parameters.Page1ClippingFigure.Segments.Add(new LineSegment(new Point(0.0, height - y), false));
                    break;

                case CornerOrigin.BottomLeft:
                    if (num8 >= 0.0)
                    {
                        parameters.Page1ClippingFigure.StartPoint = new Point(width, 0.0);
                        parameters.Page1ClippingFigure.Segments.Clear();
                        parameters.Page1ClippingFigure.Segments.Add(new LineSegment(new Point(x, 0.0), false));
                        parameters.Page1ClippingFigure.Segments.Add(new LineSegment(new Point(width, y), false));
                        break;
                    }
                    parameters.Page1ClippingFigure.StartPoint = new Point(width, height);
                    parameters.Page1ClippingFigure.Segments.Clear();
                    parameters.Page1ClippingFigure.Segments.Add(new LineSegment(new Point(num7, height), false));
                    parameters.Page1ClippingFigure.Segments.Add(new LineSegment(new Point(width, y), false));
                    break;

                case CornerOrigin.BottomRight:
                    if (num8 >= 0.0)
                    {
                        parameters.Page1ClippingFigure.StartPoint = new Point(0.0, 0.0);
                        parameters.Page1ClippingFigure.Segments.Clear();
                        parameters.Page1ClippingFigure.Segments.Add(new LineSegment(new Point(width - x, 0.0), false));
                        parameters.Page1ClippingFigure.Segments.Add(new LineSegment(new Point(0.0, y), false));
                        break;
                    }
                    parameters.Page1ClippingFigure.StartPoint = new Point(0.0, height);
                    parameters.Page1ClippingFigure.Segments.Clear();
                    parameters.Page1ClippingFigure.Segments.Add(new LineSegment(new Point(width - num7, height), false));
                    parameters.Page1ClippingFigure.Segments.Add(new LineSegment(new Point(0.0, y), false));
                    break;
            }
            parameters.Page2ClippingFigure.StartPoint = new Point(width - parameters.Page1ClippingFigure.StartPoint.X, parameters.Page1ClippingFigure.StartPoint.Y);
            parameters.Page2ClippingFigure.Segments = parameters.Page1ClippingFigure.Segments.Clone();
            ((LineSegment)parameters.Page2ClippingFigure.Segments[0]).Point = new Point(width - ((LineSegment)parameters.Page2ClippingFigure.Segments[0]).Point.X, ((LineSegment)parameters.Page2ClippingFigure.Segments[0]).Point.Y);
            ((LineSegment)parameters.Page2ClippingFigure.Segments[1]).Point = new Point(width - ((LineSegment)parameters.Page2ClippingFigure.Segments[1]).Point.X, ((LineSegment)parameters.Page2ClippingFigure.Segments[1]).Point.Y);
            CornerOrigin topLeft = CornerOrigin.TopLeft;
            switch (origin)
            {
                case CornerOrigin.TopLeft:
                    topLeft = CornerOrigin.TopRight;
                    break;

                case CornerOrigin.TopRight:
                    topLeft = CornerOrigin.TopLeft;
                    break;

                case CornerOrigin.BottomLeft:
                    topLeft = CornerOrigin.BottomRight;
                    break;

                case CornerOrigin.BottomRight:
                    topLeft = CornerOrigin.BottomLeft;
                    break;
            }
            NLinearGradientHelper.ComputePointsFromTop(width, height, width - num7, height - y, 20.0, 20.0, topLeft, out point, out point2);
            parameters.Page1ReflectionStartPoint = point;
            parameters.Page1ReflectionEndPoint = point2;
            double num9 = Math.Sqrt(Math.Pow(p.X - width, 2.0) + Math.Pow(p.Y - height, 2.0));
            double num10 = num9 / 10.0;
            double num11 = num9 / 10.0;
            NLinearGradientHelper.ComputePoints(width, height, width - num7, height - y, num10, num11, origin, out point3, out point4);
            parameters.Page0ShadowStartPoint = point3;
            parameters.Page0ShadowEndPoint = point4;
            return new PageParameters?(parameters);
        }

        public static double ComputeProgressRatio(BookPage source, Point p, CornerOrigin origin)
        {
            if ((origin != CornerOrigin.BottomLeft) && (origin != CornerOrigin.TopLeft))
            {
                return ((source.RenderSize.Width - p.X) / source.RenderSize.Width);
            }
            return (p.X / source.RenderSize.Width);
        }

        public static PageParameters ResetPage(BookPage source, CornerOrigin origin)
        {
            return new PageParameters(source.RenderSize) { Page0ShadowOpacity = 0.0, Page1ClippingFigure = new PathFigure(), Page1ReflectionStartPoint = new Point(0.0, 0.0), Page1ReflectionEndPoint = new Point(0.0, 0.0), Page1RotateAngle = 0.0, Page1RotateCenterX = 0.0, Page1RotateCenterY = 0.0, Page1TranslateX = 0.0, Page1TranslateY = 0.0, Page2ClippingFigure = new PathFigure(), Page0ShadowStartPoint = new Point(0.0, 0.0), Page0ShadowEndPoint = new Point(0.0, 0.0) };
        }
 
    }
}