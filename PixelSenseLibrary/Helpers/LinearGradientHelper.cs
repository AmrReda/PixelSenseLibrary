using System;
using System.Windows;
using PixelSenseLibrary.Enums;

namespace PixelSenseLibrary.Helpers
{
    public static class NLinearGradientHelper
    {
        public static void ComputePoints(double controlWidth, double controlHeight, double x, double y, double r1, double r2, CornerOrigin fromCorner, out Point startPoint, out Point endPoint)
        {
            double num = x;
            double num2 = -num / y;
            double num3 = -num2;
            double num4 = num / (1.0 - (num2 * num3));
            double num5 = num3 * num4;
            double num6 = Math.Sqrt((num4 * num4) + (num5 * num5));
            double num7 = ((num4 * (r1 + r2)) / num6) + num4;
            double num8 = ((num5 * (r1 + r2)) / num6) + num5;
            switch (fromCorner)
            {
                case CornerOrigin.TopRight:
                    num4 = controlWidth - num4;
                    num7 = controlWidth - num7;
                    break;

                case CornerOrigin.BottomLeft:
                    num5 = controlHeight - num5;
                    num8 = controlHeight - num8;
                    break;

                case CornerOrigin.BottomRight:
                    num4 = controlWidth - num4;
                    num7 = controlWidth - num7;
                    num5 = controlHeight - num5;
                    num8 = controlHeight - num8;
                    break;
            }
            startPoint = new Point(num4 / controlWidth, num5 / controlHeight);
            endPoint = new Point(num7 / controlWidth, num8 / controlHeight);
        }

        public static void ComputePointsFromTop(double controlWidth, double controlHeight, double x, double y, double r1, double r2, CornerOrigin fromCorner, out Point startPoint, out Point endPoint)
        {
            double num = x;
            double num2 = -num / y;
            double num3 = -num2;
            double num4 = num / (1.0 - (num2 * num3));
            double num5 = num3 * num4;
            double num6 = Math.Sqrt((num4 * num4) + (num5 * num5));
            double num7 = num4 - ((num4 * (r1 + r2)) / num6);
            double num8 = num5 - ((num5 * (r1 + r2)) / num6);
            switch (fromCorner)
            {
                case CornerOrigin.TopRight:
                    num4 = controlWidth - num4;
                    num7 = controlWidth - num7;
                    break;

                case CornerOrigin.BottomLeft:
                    num5 = controlHeight - num5;
                    num8 = controlHeight - num8;
                    break;

                case CornerOrigin.BottomRight:
                    num4 = controlWidth - num4;
                    num7 = controlWidth - num7;
                    num5 = controlHeight - num5;
                    num8 = controlHeight - num8;
                    break;
            }
            startPoint = new Point(num4 / controlWidth, num5 / controlHeight);
            endPoint = new Point(num7 / controlWidth, num8 / controlHeight);
        }
 
    }
}