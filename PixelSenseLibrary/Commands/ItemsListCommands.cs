namespace PixelSenseLibrary.Commands
{
    using System.Windows;
    using System.Windows.Input;

    public class ItemsListCommands
    {
        private static RoutedUICommand m_cmdSelectFirst = new RoutedUICommand("Select First", "SelectFirst", typeof(FrameworkElement));
        private static RoutedUICommand m_cmdSelectLast = new RoutedUICommand("Select Last", "SelectLast", typeof(FrameworkElement));
        private static RoutedUICommand m_cmdSelectNext = new RoutedUICommand("Select Next", "SelectNext", typeof(FrameworkElement));
        private static RoutedUICommand m_cmdSelectPrevious = new RoutedUICommand("Select Previous", "SelectPrevious", typeof(FrameworkElement));

        public static RoutedUICommand SelectFirst
        {
            get
            {
                return m_cmdSelectFirst;
            }
        }

        public static RoutedUICommand SelectLast
        {
            get
            {
                return m_cmdSelectLast;
            }
        }

        public static RoutedUICommand SelectNext
        {
            get
            {
                return m_cmdSelectNext;
            }
        }

        public static RoutedUICommand SelectPrevious
        {
            get
            {
                return m_cmdSelectPrevious;
            }
        }
    }
}