using System.Windows;
using System.Windows.Interop;

namespace PixelSenseLibrary.Extensions
{
    /// <summary>
    /// Static class containing a set of extension methods related to WPF Touch APIs and Surface SDK APIs.
    /// </summary>
    public static class TouchExtensions
    {
        /// <summary>
        /// Enables <see cref="NativeTouchDevice"/> and disables tablet gestures (flicks, hold gesture, etc.) on the <see cref="Window"/>.
        /// </summary>
        /// <param name="window">The <see cref="Window"/> to enable <see cref="NativeTouchDevice"/> on.</param>
        /// <returns>True if the operation succeeded</returns>
        public static bool TryEnableNativeTouch(this Window window)
        {
            try
            {
                // Enable NativeTouchDevice on the Window
                NativeTouchDevice.RegisterEvents(window);

                // Disable tablet gestures (flicks, etc.)
                try
                {
                    TabletUtil.DisableTabletGestures(new WindowInteropHelper(window).EnsureHandle());
                }
                catch { }

                return true;
            }
            catch { return false; }
        }
    }
}