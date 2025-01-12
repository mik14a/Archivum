using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Archivum.Models;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Manga
{
    public required string Author { get; set; }
    public required string Title { get; set; }
    public required string Volume { get; set; }

    public required string Path { get; set; }
    public required int Cover { get; set; }
    public required DateTime Created { get; set; }
    public required DateTime Modified { get; set; }
    public required long Size { get; set; }

    public DateTime LastRead { get; set; }

    public static readonly string AuthorPattern = "{author}";
    public static readonly string TitlePattern = "{title}";
    public static readonly string VolumePattern = "{volume}";

    public static Manga CreateFrom(FileInfo file, string filePattern) {
        var fileName = System.IO.Path.GetFileNameWithoutExtension(file.Name);
        var pattern = Regex.Escape(filePattern)
            .Replace(Regex.Escape(Manga.AuthorPattern), "(?<author>.+)")
            .Replace(Regex.Escape(Manga.TitlePattern), "(?<title>.+)")
            .Replace(Regex.Escape(Manga.VolumePattern), "(?<volume>.+)");
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

    private string GetDebuggerDisplay() {
        return $"[{Author}] {Title} #{Volume} ({Path})";
    }
}
