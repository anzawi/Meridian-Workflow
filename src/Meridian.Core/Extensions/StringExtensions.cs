namespace Meridian.Core.Extensions;

/// <summary>
/// Provides extension methods for <see cref="System.String"/> to add additional functionality.
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// Converts a given string to PascalCase format.
    /// Words are split based on non-alphanumeric characters,
    /// and the first letter of each resulting word is capitalized.
    /// </summary>
    /// <param name="value">The input string to convert to PascalCase.</param>
    /// <returns>A PascalCase version of the input string, or an empty string if the input is null or whitespace.</returns>
    public static string ToPascalCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;
        
        var words = System.Text.RegularExpressions.Regex
            .Split(value, @"[^a-zA-Z0-9]+")
            .Where(w => !string.IsNullOrWhiteSpace(w));

        var result = string.Concat(words.Select(w =>
            char.ToUpperInvariant(w[0]) + w[1..].ToLowerInvariant()));

        return result;
    }
}