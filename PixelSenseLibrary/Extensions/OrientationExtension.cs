using System;
using System.Windows;
using PixelSenseLibrary.Enums;

namespace PixelSenseLibrary.Extensions
{
    public static class OrientationExtension
    {
        public static bool? CheckValidMovementForOrientation(this Orientation refOrienationToTest, Point ptOriginPoint, Point ptCurrentPoint, double dMinDisplacement, double dWrongDirectionThreshold)
        {
            bool flag = false;
            bool flag2 = false;
            if (refOrienationToTest != Orientation.None)
            {
                foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
                {
                    if ((orientation == Orientation.None) || ((refOrienationToTest & orientation) != orientation))
                    {
                        continue;
                    }
                    Point point = new Point(ptOriginPoint.X, ptOriginPoint.Y);
                    Point point2 = new Point(ptCurrentPoint.X, ptCurrentPoint.Y);
                    Point point3 = new Point(ptOriginPoint.X, ptOriginPoint.Y);
                    Point point4 = new Point(ptCurrentPoint.X, ptCurrentPoint.Y);
                    bool flag3 = false;
                    switch (orientation)
                    {
                        case Orientation.Left:
                        case Orientation.Right:
                        case Orientation.Horizontal:
                            point.Y = 0.0;
                            point2.Y = 0.0;
                            point3.X = 0.0;
                            point4.X = 0.0;
                            if (((orientation == Orientation.Left) && (point2.X > (point.X + dWrongDirectionThreshold))) || ((orientation == Orientation.Right) && (point2.X < (point.X - dWrongDirectionThreshold))))
                            {
                                flag3 = true;
                            }
                            break;
                    }
                    if (((orientation == Orientation.Top) || (orientation == Orientation.Bottom)) || (orientation == Orientation.Vertical))
                    {
                        point.X = 0.0;
                        point2.X = 0.0;
                        point3.Y = 0.0;
                        point4.Y = 0.0;
                        if (((orientation == Orientation.Top) && (point2.Y > (point.Y + dWrongDirectionThreshold))) || ((orientation == Orientation.Bottom) && (point2.Y < (point.Y - dWrongDirectionThreshold))))
                        {
                            flag3 = true;
                        }
                    }
                    double length = 0.0;
                    double num2 = 0.0;
                    if (((orientation == Orientation.TopLeft) || (orientation == Orientation.TopRight)) || ((orientation == Orientation.BottomLeft) || (orientation == Orientation.BottomRight)))
                    {
                        Vector vector = (Vector)(point2 - point);
                        double d = 0.0;
                        if (orientation == Orientation.TopLeft)
                        {
                            d = Vector.AngleBetween(new Vector(-1.0, -1.0), vector) * 0.017453292519943295;
                        }
                        else if (orientation == Orientation.TopRight)
                        {
                            d = Vector.AngleBetween(new Vector(1.0, -1.0), vector) * 0.017453292519943295;
                        }
                        else if (orientation == Orientation.BottomLeft)
                        {
                            d = Vector.AngleBetween(new Vector(-1.0, 1.0), vector) * 0.017453292519943295;
                        }
                        else if (orientation == Orientation.BottomRight)
                        {
                            d = Vector.AngleBetween(new Vector(1.0, 1.0), vector) * 0.017453292519943295;
                        }
                        length = Math.Cos(d) * vector.Length;
                        num2 = Math.Sin(d) * vector.Length;
                        if (((d < -1.5707963267948966) || (d > 1.5707963267948966)) && (length > dWrongDirectionThreshold))
                        {
                            flag3 = true;
                        }
                    }
                    else
                    {
                        Vector vector2 = (Vector)(point2 - point);
                        Vector vector3 = (Vector)(point4 - point3);
                        length = vector2.Length;
                        num2 = vector3.Length;
                    }
                    if ((orientation != Orientation.All) && (num2 > dWrongDirectionThreshold))
                    {
                        flag3 = true;
                    }
                    if (!flag3)
                    {
                        flag = true;
                        if (length > dMinDisplacement)
                        {
                            flag2 = true;
                            break;
                        }
                    }
                }
            }
            if (flag2)
            {
                return true;
            }
            if (flag)
            {
                return false;
            }
            return null;
        }
 
    }
}