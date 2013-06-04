using System.Windows.Input;
using PixelSenseLibrary.Controls.ItemsCollections;

namespace PixelSenseLibrary.Commands
{
    public class BookCommands
    {
        // Fields
        private static RoutedUICommand m_cmdTurnLeftPage = new RoutedUICommand("Turn Left Page", "TurnLeftPage", typeof(Book));
        private static RoutedUICommand m_cmdTurnRightPage = new RoutedUICommand("Turn Right Page", "TurnRightPage", typeof(Book));
        public static RoutedUICommand NavigateToPage = new RoutedUICommand("Navigate to a specified page", "NavigateToPage", typeof(BookCommands));

        // Properties
        public static RoutedUICommand TurnLeftPage
        {
            get
            {
                return m_cmdTurnLeftPage;
            }
        }

        public static RoutedUICommand TurnRightPage
        {
            get
            {
                return m_cmdTurnRightPage;
            }
        }

    }
}