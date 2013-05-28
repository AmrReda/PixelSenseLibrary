using System;
using System.Reflection;
using Microsoft.Surface;
using PixelSenseLibrary.Utilities;


namespace Base.Helpers
{
    /// <summary>
    /// Static class containing helper methods for manipulating the <see cref="SurfaceEnvironment"/> class.
    /// </summary>
    public static class SurfaceEnvironmentHelper
    {
        #region Fields

        private const string IsSurfaceInputAvailableFieldName = "isSurfaceInputAvailable";
        private static readonly FieldInfo s_IsSurfaceInputAvailableFieldInfo = typeof(SurfaceEnvironment).GetField(IsSurfaceInputAvailableFieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.GetField);

        private static bool s_IsSurfaceInputSuppressed = false;

        #endregion

        #region CLR Properties

        #region IsSurfaceInputSuppressed

        /// <summary>
        /// Gets whether Surface Input has been suppressed.
        /// </summary>
        public static bool IsSurfaceInputSuppressed { get { return s_IsSurfaceInputSuppressed; } }

        #endregion

        #region IsRunningOnSurfaceHardware

        /// <summary>
        /// Gets whether this application is running on actual Surface Hardware.
        /// </summary>
        /// <remarks>
        /// This is implemented by checking if the Surface Digitizer is available in Surface's HID Input Provider APIs.
        /// </remarks>
        public static bool IsRunningOnSurfaceHardware
        {
            get
            {
                var contextMapTypeName = ReflectionUtilities.ConstructAssemblyQualifiedName("Microsoft.Surface.Core", "Microsoft.Surface.Core.ContextMap");
                var contextMapType = Type.GetType(contextMapTypeName, false, false);

                // Get ContextMap instance
                PropertyInfo instanceInfo = contextMapType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty);
                var contextMap = instanceInfo.GetValue(null, null);

                // Ensure InputProvider
                MethodInfo ensureInputProviderInfo = contextMap.GetType().GetMethod("EnsureInputProvider", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic);
                ensureInputProviderInfo.Invoke(contextMap, null);

                // Get InputProvider
                PropertyInfo inputProviderInfo = contextMap.GetType().GetProperty("InputProvider", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty);
                var inputProvider = inputProviderInfo.GetValue(contextMap, null);

                // Get SurfaceDigitizer
                PropertyInfo digitizerInfo = inputProvider.GetType().GetProperty("Digitizer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
                var surfaceDigitizer = digitizerInfo.GetValue(inputProvider, null);

                // Return whether the Surface Digitizer is present or not.
                return surfaceDigitizer != null;
            }
        }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Suppresses Surface Input. Throws <see cref="InvalidOperationException"/> if suppression fails.
        /// </summary>
        /// <remarks>
        /// If you want suppression on a <c>SurfaceWindow</c> you must call this function
        /// before calling the constructor of the <c>SurfaceWindow</c>. A best practise is to override the <c>OnStartup()</c> method on the <c>Application</c>
        /// class of your application and suppress Surface Input from there.
        /// Use <see cref="TrySuppressSurfaceInput"/> to avoid exceptions being thrown.
        /// </remarks>
        public static void SuppressSurfaceInput()
        {
            if (s_IsSurfaceInputSuppressed)
            {
                // Already suppressed.
                return;
            }

            // Suppress Surface Input
            s_IsSurfaceInputAvailableFieldInfo.SetValue(null, new bool?(false));
            if (SurfaceEnvironment.IsSurfaceInputAvailable)
            {
                throw new InvalidOperationException("Surface Input Suppression Failed");
            }

            s_IsSurfaceInputSuppressed = true;
        }

        /// <summary>
        /// Tries to suppress Surface Input and returns whether suppression succeeded or not.
        /// </summary>
        /// <returns>True if suppression of Surface Input was successful</returns>
        /// <remarks>
        /// This method internally calls <see cref="SuppressSurfaceInput"/> and transforms the (potentially) thrown exception into a bool and returns it.
        /// </remarks>
        public static bool TrySuppressSurfaceInput()
        {
            try
            {
                SuppressSurfaceInput();
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Suppresses Surface Input if the current <c>Application</c> is running on non-Surface hardware. Throws <see cref="InvalidOperationException"/> if suppression fails.
        /// </summary>
        /// <remarks>
        /// Internally calls <see cref="SuppressSurfaceInput"/>.
        /// </remarks>
        public static void SuppressSurfaceInputOnNonSurfaceHardware()
        {
            if (!IsRunningOnSurfaceHardware)
            {
                SuppressSurfaceInput();
            }
        }

        /// <summary>
        /// Tries to suppress Surface Input if the current <c>Application</c> is running on non-Surface hardware.
        /// </summary>
        /// <returns>True if suppression of Surface input was successful or if the current <c>Application</c>
        /// is running on non-Surface hardware.</returns>
        /// <remarks>
        /// Internally calls <see cref="SuppressSurfaceInputOnNonSurfaceHardware"/>.
        /// </remarks>
        public static bool TrySuppressSurfaceInputOnNonSurfaceHardware()
        {
            try
            {
                SuppressSurfaceInputOnNonSurfaceHardware();
                return true;
            }
            catch { return false; }
        }

        #endregion
    }
}