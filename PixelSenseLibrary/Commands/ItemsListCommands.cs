namespace PixelSenseLibrary.Commands
{
    using System.Windows;
    using System.Windows.Input;


    /// <summary>
    /// Commands for ItemsList controls 
    /// </summary>
    public class ItemsListCommands
    {
        #region SelectFirst Command
        
        private static RoutedUICommand m_cmdSelectFirst = new RoutedUICommand("Select First", "SelectFirst", typeof(FrameworkElement));

        public static RoutedUICommand SelectFirst
        {
            get
            {
                return m_cmdSelectFirst;
            }
        }


        #endregion

        #region SelectLast
        
        private static RoutedUICommand m_cmdSelectLast = new RoutedUICommand("Select Last", "SelectLast", typeof(FrameworkElement));

        public static RoutedUICommand SelectLast
        {
            get
            {
                return m_cmdSelectLast;
            }
        } 

        #endregion

        #region SelectNext

        private static RoutedUICommand m_cmdSelectNext = new RoutedUICommand("Select Next", "SelectNext", typeof(FrameworkElement));

        public static RoutedUICommand SelectNext
        {
            get
            {
                return m_cmdSelectNext;
            }
        }

        #endregion

        #region SelectPrevious

        private static RoutedUICommand m_cmdSelectPrevious = new RoutedUICommand("Select Previous", "SelectPrevious", typeof(FrameworkElement));

        public static RoutedUICommand SelectPrevious
        {
            get
            {
                return m_cmdSelectPrevious;
            }
        }

        #endregion
    }
}