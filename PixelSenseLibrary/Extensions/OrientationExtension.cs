using System;
using System.Windows;

namespace PixelSenseLibrary.Extensions
{
    public static class OrientationExtension
    {
        public static bool? CheckValidMovementForOrientation(this NOrientation refOrienationToTest, Point ptOriginPoint, Point ptCurrentPoint, double dMinDisplacement, double dWrongDirectionThreshold)
        {
            bool flag = false;
            bool flag2 = false;
            if (refOrienationToTest != NOrientation.None)
            {
                foreach (NOrientation orientation in Enum.GetValues(typeof(NOrientation)))
                {
                    if ((orientation == NOrientation.None) || ((refOrienationToTest & orientation) != orientation))
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
                        case NOrientation.Left:
                        case NOrientation.Right:
                        case NOrientation.Horizontal:
                            point.Y = 0.0;
                            point2.Y = 0.0;
                            point3.X = 0.0;
                            point4.X = 0.0;
                            if (((orientation == NOrientation.Left) && (point2.X > (point.X + dWrongDirectionThreshold))) || ((orientation == NOrientation.Right) && (point2.X < (point.X - dWrongDirectionThreshold))))
                            {
                                flag3 = true;
                            }
                            break;
                    }
                    if (((orientation == NOrientation.Top) || (orientation == NOrientation.Bottom)) || (orientation == NOrientation.Vertical))
                    {
                        point.X = 0.0;
                        point2.X = 0.0;
                        point3.Y = 0.0;
                        point4.Y = 0.0;
                        if (((orientation == NOrientation.Top) && (point2.Y > (point.Y + dWrongDirectionThreshold))) || ((orientation == NOrientation.Bottom) && (point2.Y < (point.Y - dWrongDirectionThreshold))))
                        {
                            flag3 = true;
                        }
                    }
                    double length = 0.0;
                    double num2 = 0.0;
                    if (((orientation == NOrientation.TopLeft) || (orientation == NOrientation.TopRight)) || ((orientation == NOrientation.BottomLeft) || (orientation == NOrientation.BottomRight)))
                    {
                        Vector vector = (Vector)(point2 - point);
                        double d = 0.0;
                        if (orientation == NOrientation.TopLeft)
                        {
                            d = Vector.AngleBetween(new Vector(-1.0, -1.0), vector) * 0.017453292519943295;
                        }
                        else if (orientation == NOrientation.TopRight)
                        {
                            d = Vector.AngleBetween(new Vector(1.0, -1.0), vector) * 0.017453292519943295;
                        }
                        else if (orientation == NOrientation.BottomLeft)
                        {
                            d = Vector.AngleBetween(new Vector(-1.0, 1.0), vector) * 0.017453292519943295;
                        }
                        else if (orientation == NOrientation.BottomRight)
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
                    if ((orientation != NOrientation.All) && (num2 > dWrongDirectionThreshold))
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