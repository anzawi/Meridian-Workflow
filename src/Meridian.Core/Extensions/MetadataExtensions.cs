namespace Meridian.Core.Extensions;

public static class MetadataExtensions
{
    /// <summary>
    /// Merges source metadata into target dictionary.
    /// </summary>
    /// <param name="target">The target dictionary to merge into.</param>
    /// <param name="source">The read-only source dictionary to merge from.</param>
    /// <param name="overwrite">If true, source values will overwrite existing keys in target.</param>
    public static void MergeMetadata(
        this Dictionary<string, object?> target,
        IReadOnlyDictionary<string, object?> source,
        bool overwrite = false)
    {
        foreach (var kvp in source)
        {
            if (overwrite || !target.ContainsKey(kvp.Key))
            {
                target[kvp.Key] = kvp.Value;
            }
        }
    }
}