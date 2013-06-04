using System;
using System.Runtime.InteropServices;

namespace FlipBookSample
{
    /// <summary>
    /// Wraps win32 method to retrieve known folder paths
    /// </summary>
    public static class CommonFolder
    {
        /// <summary>
        /// Contain lists of Guids to known folders
        /// </summary>
        private static class KnownFolder
        {
            public static readonly Guid SamplePictures = new Guid("C4900540-2379-4C75-844B-64E6FAF8716B");
            public static readonly Guid SampleVideos = new Guid("859EAD94-2E85-48AD-A71A-0969CB56A6CD");
        }

        /// <summary>
        /// Retrieves the folder path containing sample photos
        /// </summary>
        /// <returns>Full path to sample photos</returns>
        public static string GetPhotoPath()
        {
            return GetFolderPath(KnownFolder.SamplePictures);
        }

        /// <summary>
        /// Retrieves the folder path containing sample videos
        /// </summary>
        /// <returns>Full path to sample videos</returns>
        public static string GetVideoPath()
        {
            return GetFolderPath(KnownFolder.SampleVideos);
        }

        /// <summary>
        /// Retrieves known folder path from shell
        /// </summary>
        /// <param name="guidKnownFolder">Guid of the known folder</param>
        /// <returns>Full path to the specified folder</returns>
        private static string GetFolderPath(Guid guidKnownFolder)
        {
            string folderPath = "";

            IntPtr pPath;
            if (NativeMethods.SHGetKnownFolderPath(guidKnownFolder, 0, IntPtr.Zero, out pPath) == 0)
            {
                folderPath = Marshal.PtrToStringUni(pPath);
                Marshal.FreeCoTaskMem(pPath);
            }

            return folderPath;
        }
    }
}
