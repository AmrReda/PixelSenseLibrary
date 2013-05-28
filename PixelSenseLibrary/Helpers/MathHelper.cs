namespace PixelSenseLibrary.Helpers
{
    /// <summary>
    /// Mathimatical operation Extension Methods
    /// </summary>
    public static class MathHelper
    { 
        public static bool IsFloat(object o)
        {
            return (((o is double) || (o is float)) || (o is decimal));
        }

        public static bool IsInteger(object o)
        {
            return (((((o is int) || (o is long)) || ((o is short) || (o is byte))) || (((o is sbyte) || (o is uint)) || (o is ulong))) || (o is ushort));
        }

        public static bool IsNumeric(object o)
        {
            return ((((((o is int) || (o is double)) || ((o is float) || (o is long))) || (((o is short) || (o is byte)) || ((o is decimal) || (o is sbyte)))) || ((o is uint) || (o is ulong))) || (o is ushort));
        }

        public static bool IsPositiveNumeric(double d)
        {
            return (IsValidNumeric(d) && (d > 0.0));
        }

        public static bool IsValidNumeric(double d)
        {
            return (!double.IsInfinity(d) && !double.IsNaN(d));
        }

        public static bool TryConvertToDouble(object o, out double convertedValue)
        {
            if (o is int)
            {
                int num = (int)o;
                convertedValue = num;
            }
            else if (o is double)
            {
                convertedValue = (double)o;
            }
            else if (o is float)
            {
                float num2 = (float)o;
                convertedValue = num2;
            }
            else if (o is long)
            {
                long num3 = (long)o;
                convertedValue = num3;
            }
            else if (o is short)
            {
                short num4 = (short)o;
                convertedValue = num4;
            }
            else if (o is byte)
            {
                byte num5 = (byte)o;
                convertedValue = num5;
            }
            else if (o is decimal)
            {
                decimal num6 = (decimal)o;
                convertedValue = (double)num6;
            }
            else if (o is sbyte)
            {
                sbyte num7 = (sbyte)o;
                convertedValue = num7;
            }
            else if (o is uint)
            {
                uint num8 = (uint)o;
                convertedValue = num8;
            }
            else if (o is ulong)
            {
                ulong num9 = (ulong)o;
                convertedValue = num9;
            }
            else if (o is ushort)
            {
                ushort num10 = (ushort)o;
                convertedValue = num10;
            }
            else if (o is string)
            {
                string str = o as string;
                if (!string.IsNullOrEmpty(str))
                {
                    double num11;
                    if (double.TryParse(str, out num11))
                    {
                        convertedValue = num11;
                    }
                    else
                    {
                        convertedValue = double.NaN;
                    }
                }
                else
                {
                    convertedValue = double.NaN;
                }
            }
            else
            {
                convertedValue = double.NaN;
                return false;
            }
            return true;
        } 
    }
}