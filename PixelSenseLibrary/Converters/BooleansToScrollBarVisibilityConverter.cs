using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Base.Converters
{
    public class BooleansToScrollBarVisibilityConverter : IMultiValueConverter
    {
        // Fields
        protected bool m_bTrueMeansVisible = true;

        // Methods
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is bool)
                {
                    bool flag = (bool)values[i];
                    if (this.m_bTrueMeansVisible)
                    {
                        if (flag)
                        {
                            return ScrollBarVisibility.Visible;
                        }
                    }
                    else if (!flag)
                    {
                        return ScrollBarVisibility.Visible;
                    }
                }
            }
            return ScrollBarVisibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        // Properties
        public bool TrueMeansVisible
        {
            get
            {
                return this.m_bTrueMeansVisible;
            }
            set
            {
                this.m_bTrueMeansVisible = value;
            }
        }
    }
}