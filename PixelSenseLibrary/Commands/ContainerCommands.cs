using System.Windows;
using System.Windows.Input;

namespace PixelSenseLibrary.Commands
{
    /// <summary>
    /// Commands for Container controls
    /// </summary>
    public class ContainerCommands
    {
        #region Open Command
        
        private static RoutedUICommand m_cmdOpen = new RoutedUICommand("Open", "Open", typeof(FrameworkElement));

        public static RoutedUICommand Open
        {
            get
            {
                return ContainerCommands.m_cmdOpen;
            }
        } 

        #endregion

        #region Close Command

        private static RoutedUICommand m_cmdClose = new RoutedUICommand("Close", "Close", typeof(FrameworkElement));
        
        public static RoutedUICommand Close
        {
            get
            {
                return ContainerCommands.m_cmdClose;
            }
        }

        #endregion
         
        #region Shrink Command
        
        private static RoutedUICommand m_cmdShrink = new RoutedUICommand("Shrink", "Shrink", typeof(FrameworkElement));

        public static RoutedUICommand Shrink
        {
            get
            {
                return ContainerCommands.m_cmdShrink;
            }
        } 

        #endregion

        #region Maximize Command

        private static RoutedUICommand m_cmdMaximize = new RoutedUICommand("Maximize", "Maximize", typeof(FrameworkElement));

        public static RoutedUICommand Maximize
        {
            get
            {
                return ContainerCommands.m_cmdMaximize;
            }
        }
        
        #endregion

        #region Resize Command

        private static RoutedUICommand m_cmdResize = new RoutedUICommand("Maximize", "Maximize", typeof(FrameworkElement));

        public static RoutedUICommand Resize
        {
            get
            {
                return ContainerCommands.m_cmdResize;
            }
        }

        #endregion

        #region Pin Command

        private static RoutedUICommand m_cmdPin = new RoutedUICommand("Pin the container", "Pin", typeof(FrameworkElement));

        public static RoutedUICommand Pin
        {
            get { return m_cmdPin; }
        }
        
        #endregion

        #region Unpin Command

        private static RoutedUICommand m_cmdUnpin = new RoutedUICommand("Unpin the container", "Unpin", typeof(FrameworkElement));
  
        public static RoutedUICommand Unpin
        {
            get
            {
                return ContainerCommands.m_cmdUnpin;
            }
        }

        #endregion

        #region StopFlicking Command

        private static RoutedUICommand m_cmdStopFlicking = new RoutedUICommand("Stop the flicking of target element", "StopFlicking", typeof(FrameworkElement));
    
        public static RoutedUICommand StopFlicking
        {
            get
            {
                return ContainerCommands.m_cmdStopFlicking;
            }
        }

        #endregion

        #region UpdateIsScrolling Command

        private static RoutedUICommand m_cmdUpdateIsScrolling = new RoutedUICommand("Update IsScrolling property of target element", "UpdateIsScrolling", typeof(FrameworkElement));

        public static RoutedUICommand UpdateIsScrolling
        {
            get
            {
                return ContainerCommands.m_cmdUpdateIsScrolling;
            }
        } 

        #endregion

        #region ForceCaptureContact Command

        private static RoutedUICommand m_cmdForceCaptureContact = new RoutedUICommand("Force the capture of a contact", "ForceCaptureContact", typeof(FrameworkElement));

        public static RoutedUICommand ForceCaptureContact
        {
            get
            {
                return ContainerCommands.m_cmdForceCaptureContact;
            }
        } 

        #endregion

        #region ResetScrollBarsPositions Command

        private static RoutedUICommand m_cmdResetScrollBarsPositions = new RoutedUICommand("Reset the positions of the scroll bars", "ResetScrollBarsPositions", typeof(FrameworkElement));

        public static RoutedUICommand ResetScrollBarsPositions
        {
            get
            {
                return ContainerCommands.m_cmdResetScrollBarsPositions;
            }
        }
         
        #endregion

        #region ScrollToX Command
        private static RoutedUICommand m_cmdScrollToX = new RoutedUICommand("Scroll to X", "ScrollToX", typeof(FrameworkElement));

        public static RoutedUICommand ScrollToX
        {
            get
            {
                return ContainerCommands.m_cmdScrollToX;
            }
        }
        
        #endregion

        #region ScrollToY Command
        
        private static RoutedUICommand m_cmdScrollToY = new RoutedUICommand("Scroll to Y", "ScrollToY", typeof(FrameworkElement));

        public static RoutedUICommand ScrollToY
        {
            get
            {
                return ContainerCommands.m_cmdScrollToY;
            }
        } 

        #endregion

        #region ChangeScrollContentSize Command
        
        private static RoutedUICommand m_cmdChangeScrollContentSize = new RoutedUICommand("Change the scroll content size (with the Size parameter)", "ChangeScrollContentSize", typeof(FrameworkElement));

        public static RoutedUICommand ChangeScrollContentSize
        {
            get
            {
                return ContainerCommands.m_cmdChangeScrollContentSize;
            }
        } 

        #endregion
    }
}