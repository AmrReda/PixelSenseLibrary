using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace PixelSenseLibrary.Converters
{
    [ValueConversion(typeof(bool), typeof(ScrollBarVisibility))]
    public class BooleanToScrollBarVisibilityConverter : IValueConverter
    {
        // Fields
        protected bool m_bTrueMeansVisible = true;

        // Methods
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = (bool)value;
            if (!this.m_bTrueMeansVisible)
            {
                flag = !flag;
            }
            return (flag ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ScrollBarVisibility visibility = (ScrollBarVisibility)value;
            bool flag = visibility == ScrollBarVisibility.Visible;
            if (!this.m_bTrueMeansVisible)
            {
                flag = !flag;
            }
            return flag;
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