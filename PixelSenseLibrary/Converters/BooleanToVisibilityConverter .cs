using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PixelSenseLibrary.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        // Fields
        protected bool m_bTrueMeansVisible = true;
        protected Visibility m_eNonVisibleState = Visibility.Collapsed;

        // Methods
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = (bool)value;
            if (!this.m_bTrueMeansVisible)
            {
                flag = !flag;
            }
            return (flag ? ((object)0) : ((object)this.m_eNonVisibleState));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = ((Visibility)value) == Visibility.Visible;
            if (!this.m_bTrueMeansVisible)
            {
                flag = !flag;
            }
            return flag;
        }

        // Properties
        public Visibility NonVisibleState
        {
            get
            {
                return this.m_eNonVisibleState;
            }
            set
            {
                if (value != Visibility.Visible)
                {
                    this.m_eNonVisibleState = value;
                }
            }
        }

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