using System.Windows.Input;
using PixelSenseLibrary.Controls.ItemsCollections;

namespace PixelSenseLibrary.Commands
{
    public class BookPageCommands
    {
        // Fields
        private static RoutedUICommand m_cmdDisableDragging = new RoutedUICommand("Disable Dragging of Book Page", "DisableDragging", typeof(Book));
        private static RoutedUICommand m_cmdEnableDragging = new RoutedUICommand("Enable Dragging of Book Page", "EnableDragging", typeof(Book));

        // Properties
        public static RoutedUICommand DisableDragging
        {
            get
            {
                return m_cmdDisableDragging;
            }
        }

        public static RoutedUICommand EnableDragging
        {
            get
            {
                return m_cmdEnableDragging;
            }
        }

    }
}