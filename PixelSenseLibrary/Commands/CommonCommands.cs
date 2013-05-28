using System.Windows.Input;

namespace PixelSenseLibrary.Commands
{
    public class CommonCommands
    {
        #region Dispose Command
        
        private static RoutedUICommand _cmdDispose = new RoutedUICommand("Dispose", "Dispose", typeof(CommonCommands));

        public static RoutedUICommand Dispose
        {
            get { return _cmdDispose; }
        } 

        #endregion

        #region Clear Command
        
        private static RoutedUICommand _cmdClear = new RoutedUICommand("Clear", "Clear", typeof(CommonCommands));

        public static RoutedUICommand Clear
        {
            get { return _cmdClear; }
        } 

        #endregion 
    }
}