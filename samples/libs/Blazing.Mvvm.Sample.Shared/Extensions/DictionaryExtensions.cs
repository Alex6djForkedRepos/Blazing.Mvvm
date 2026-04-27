using Blazing.Mvvm.Sample.Shared.Models.ParentChild;

namespace Blazing.Mvvm.Sample.Shared.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Dictionary{TKey, TValue}"/> related to child component metadata.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Adds a child component entry to the dictionary with the specified name and default parameters. Set up metadata to be passed when the ChildView is created
    /// </summary>
    /// <param name="dictionary">The dictionary to add the child component metadata to.</param>
    /// <param name="name">The name of the child component.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dictionary"/> or <paramref name="name"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if an element with the same key already exists in the dictionary.</exception>
    public static void AddChildComponent(this Dictionary<string, ChildMetadata> dictionary, string name)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(name);
        dictionary.Add(name, new() { Parameters = new() { ["Text"] = name } });
    }
}
