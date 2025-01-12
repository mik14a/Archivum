using System;
using System.Diagnostics;

namespace Archivum.Models;

/// <summary>
/// Represents a title entity in the system with associated metadata.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Title
{
    /// <summary>
    /// Gets or sets the name of the title.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the name of the author associated with this title.
    /// </summary>
    public required string Author { get; set; }

    /// <summary>
    /// Gets or sets the total count of manga volumes associated with this title.
    /// </summary>
    public required int Count { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of when this title's information was last modified.
    /// </summary>
    public required DateTime LastModified { get; set; }

    /// <summary>
    /// Gets or sets the path or URL to the title's cover image.
    /// </summary>
    public required string Cover { get; set; }

    /// <summary>
    /// Returns a string representation of the title for debugging purposes.
    /// </summary>
    /// <returns>A string containing the author's name in square brackets followed by the title name.</returns>
    string GetDebuggerDisplay() {
        return $"[{Author}] {Name}";
    }
}
