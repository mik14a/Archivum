using System;

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

    public static readonly string AuthorPattern = "author";
    public static readonly string TitlePattern = "title";
    public static readonly string VolumePattern = "volume";
}
