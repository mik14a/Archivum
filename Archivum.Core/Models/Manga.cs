using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Archivum.Models;

/// <summary>
/// Represents a manga entry in the library with associated metadata and file information.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Manga
{
    /// <summary>
    /// Gets or sets the name of the manga's author.
    /// </summary>
    public required string Author { get; set; }

    /// <summary>
    /// Gets or sets the title of the manga.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the volume number or identifier of the manga.
    /// </summary>
    public required string Volume { get; set; }

    /// <summary>
    /// Gets or sets the full file path to the manga file.
    /// </summary>
    public required string Path { get; set; }

    /// <summary>
    /// Gets or sets the index of the cover page within the manga file.
    /// </summary>
    public required int Cover { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp of the manga file.
    /// </summary>
    public required DateTime Created { get; set; }

    /// <summary>
    /// Gets or sets the last modification timestamp of the manga file.
    /// </summary>
    public required DateTime Modified { get; set; }

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public required long Size { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the manga was last read.
    /// </summary>
    public DateTime LastRead { get; set; }

    /// <summary>
    /// Pattern placeholder for the author field in file naming templates.
    /// </summary>
    public static readonly string AuthorPattern = "{author}";

    /// <summary>
    /// Pattern placeholder for the title field in file naming templates.
    /// </summary>
    public static readonly string TitlePattern = "{title}";

    /// <summary>
    /// Pattern placeholder for the volume field in file naming templates.
    /// </summary>
    public static readonly string VolumePattern = "{volume}";

    /// <summary>
    /// Creates a new Manga instance from a file using the specified naming pattern.
    /// </summary>
    /// <param name="file">The FileInfo object representing the manga file.</param>
    /// <param name="filePattern">The pattern used to extract metadata from the filename.</param>
    /// <returns>A new Manga instance populated with metadata from the file.</returns>
    public static Manga CreateFrom(FileInfo file, string filePattern) {
        var fileName = System.IO.Path.GetFileNameWithoutExtension(file.Name);
        var pattern = Regex.Escape(filePattern)
            .Replace(Regex.Escape(AuthorPattern), "(?<author>.+)")
            .Replace(Regex.Escape(TitlePattern), "(?<title>.+)")
            .Replace(Regex.Escape(VolumePattern), "(?<volume>.+)");
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);
        var match = regex.Match(fileName);
        var author = match.Groups.TryGetValue("author", out var authorValue) ? authorValue.Value : null;
        var title = match.Groups.TryGetValue("title", out var titleValue) ? titleValue.Value : null;
        var volume = match.Groups.TryGetValue("volume", out var volumeValue) ? volumeValue.Value : null;
        return new() {
            Author = author ?? string.Empty, Title = title ?? fileName, Volume = volume ?? string.Empty,
            Path = file.FullName, Cover = 0, Created = file.CreationTime, Modified = file.LastWriteTime, Size = file.Length,
            LastRead = DateTime.MinValue,
        };
    }

    /// <summary>
    /// Returns a string representation of the manga for debugging purposes.
    /// </summary>
    /// <returns>A formatted string containing the manga's author, title, volume, and path.</returns>
    string GetDebuggerDisplay() {
        return $"[{Author}] {Title} #{Volume} ({Path})";
    }
}
