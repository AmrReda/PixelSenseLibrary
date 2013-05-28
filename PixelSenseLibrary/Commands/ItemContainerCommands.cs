using System.Windows;
using System.Windows.Input;

namespace PixelSenseLibrary.Commands
{
    /// <summary>
    /// Commands for ItemContainer controls 
    /// </summary>
    public static class ItemContainerCommands
    {
        #region AddItem Command

        private static RoutedUICommand _cmdAddItem = new RoutedUICommand("Add Item", "AddItem", typeof(FrameworkElement));

        public static RoutedUICommand AddItem
        {
            get
            {
                return ItemContainerCommands._cmdAddItem;
            }
        } 

        #endregion

        #region RemoveItem Command
        
        private static RoutedUICommand _cmdRemoveItem = new RoutedUICommand("Remove Item", "RemoveItem", typeof(FrameworkElement));

        public static RoutedUICommand RemoveItem
        {
            get
            {
                return ItemContainerCommands._cmdRemoveItem;
            }
        } 

        #endregion

        #region OpenItem Command
        
        private static RoutedUICommand _cmdOpenItem = new RoutedUICommand("Open Item", "OpenItem", typeof(FrameworkElement));

        public static RoutedUICommand OpenItem
        {
            get
            {
                return ItemContainerCommands._cmdOpenItem;
            }
        } 

        #endregion

        #region CloseItem Command
        
        private static RoutedUICommand _cmdCloseItem = new RoutedUICommand("Close Item", "CloseItem", typeof(FrameworkElement));

        public static RoutedUICommand CloseItem
        {
            get
            {
                return ItemContainerCommands._cmdCloseItem;
            }
        } 

        #endregion  
    }
}