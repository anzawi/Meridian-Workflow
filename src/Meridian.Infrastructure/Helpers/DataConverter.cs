namespace Meridian.Infrastructure.Helpers;

using System.Text.Json;
using Core.Interfaces;

/// <summary>
/// Provides methods for converting workflow data objects between their JSON representations and
/// Dictionary{string, object?} format.
/// </summary>
internal static class DataConverter
{
    /// <summary>
    /// Converts a dictionary of key-value pairs to an object of type <typeparamref name="T"/>
    /// that implements the <see cref="IWorkflowData"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize into, must implement <see cref="IWorkflowData"/>.</typeparam>
    /// <param name="data">The dictionary containing the data to be deserialized.</param>
    /// <returns>An instance of <typeparamref name="T"/> populated with the data from the provided dictionary.</returns>
    internal static T Deserialize<T>(Dictionary<string, object?> data) where T : IWorkflowData
    {
        var json = JsonSerializer.Serialize(data);
        return JsonSerializer.Deserialize<T>(json)!;
    }

    /// <summary>
    /// Serializes the given object implementing the <see cref="IWorkflowData"/> interface into a dictionary of string keys and object values.
    /// </summary>
    /// <typeparam name="T">The type that implements <see cref="IWorkflowData"/>.</typeparam>
    /// <param name="dto">The object to be serialized.</param>
    /// <returns>A dictionary representation of the object, where keys are property names and values are property values.</returns>
    internal static Dictionary<string, object?> Serialize<T>(T dto) where T : IWorkflowData
    {
        var json = JsonSerializer.Serialize(dto);
        return JsonSerializer.Deserialize<Dictionary<string, object?>>(json)!;
    }
}