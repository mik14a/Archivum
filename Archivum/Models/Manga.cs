using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Archivum.Models;

public class Manga
{
    public string Author { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Volume { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public long Size { get; set; }

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
            Path = file.FullName, Created = file.CreationTime, Modified = file.LastWriteTime, Size = file.Length,
        };
    }
}
