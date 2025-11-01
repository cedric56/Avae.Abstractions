using Avalonia.Controls;
using Avalonia.Platform;
using System.Text.RegularExpressions;

namespace Avae.Implementations
{
    public static class Extensions
    {
        public static WindowIcon? GetIcon(string url)
        {
            try
            {
                return new WindowIcon(AssetLoader.Open(new Uri(url)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static IEnumerable<string> SplitOnCapitals(this string text)
        {
            var regex = new Regex(@"\p{Lu}\p{Ll}*");
            foreach (Match match in regex.Matches(text))
            {
                yield return match.Value;
            }
        }

        public static T ToEnum<T>(this string value, T defaultValue = default) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return Enum.TryParse(value, true, out T result) ? result : defaultValue;
        }

        public static T TryParse<T>(this Enum val) where T : struct => Enum.TryParse(val.ToString(), out T value) ? value : default;
    }
}
