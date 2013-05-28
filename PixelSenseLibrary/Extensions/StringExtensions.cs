namespace PixelSenseLibrary.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string input)
        {
            return !string.IsNullOrEmpty(input);
        }  
    }
}