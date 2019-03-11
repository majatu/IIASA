namespace IIASA.Common.Extensions
{
    public static class StringExtensions
    {
        public static string TrimOrEmpty(this string value)
        {
            return value != null ? value.Trim() : string.Empty;
        }
    }
}
