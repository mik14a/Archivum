using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Archivum.Models;

/// <summary>
/// Represents an author entity in the system with associated metadata.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Author
{
    /// <summary>
    /// Gets or sets the name of the author.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this author is marked as a favorite.
    /// This property is ignored during JSON serialization when set to its default value.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Favorite { get; set; }

    /// <summary>
    /// Gets or sets the total count of works associated with this author.
    /// </summary>
    public required int Count { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of when this author's information was last modified.
    /// </summary>
    public required DateTime LastModified { get; set; }

    /// <summary>
    /// Gets or sets the path or URL to the author's cover image.
    /// </summary>
    public required string Cover { get; set; }

    /// <summary>
    /// Returns a string representation of the author for debugging purposes.
    /// </summary>
    /// <returns>A string containing the author's name in square brackets.</returns>
    private string GetDebuggerDisplay() {
        return $"[{Name}]";
    }
}
