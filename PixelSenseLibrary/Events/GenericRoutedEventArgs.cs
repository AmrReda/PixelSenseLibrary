using System.Windows;

namespace PixelSenseLibrary.Events
{
    public class GenericRoutedEventArgs<T> : RoutedEventArgs
    {
        private T m_refParam;

        public T Param
        {
            get
            {
                return this.m_refParam;
            }
        }

        public GenericRoutedEventArgs(T refParam)
        {
            this.m_refParam = refParam;
        }
    }
    public class GenericRoutedEventArgs<T, U> : RoutedEventArgs
    {
        private T m_refFirstParam;
        private U m_refSecondParam;

        public T FirstParam
        {
            get
            {
                return this.m_refFirstParam;
            }
        }

        public U SecondParam
        {
            get
            {
                return this.m_refSecondParam;
            }
        }

        public GenericRoutedEventArgs(T refFirstParam, U refSecondParam)
        {
            this.m_refFirstParam = refFirstParam;
            this.m_refSecondParam = refSecondParam;
        }
    }
}