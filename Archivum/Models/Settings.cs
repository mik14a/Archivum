using System;
using System.IO;

namespace Archivum.Models;

public class Settings
{
    public static readonly string DefaultFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(Archivum));
    public static readonly string DefaultImageExtensions = ".jpg;.jpeg;.png";
    public static readonly string DefaultFilePattern = $"[{Manga.AuthorPattern}] {Manga.TitlePattern} {Manga.VolumePattern}";
    public static readonly string DefaultBackdrop = nameof(Controls.SystemBackdrop.Mica);

    public string FolderPath { get; set; } = DefaultFolderPath;
    public string? ImageExtensions { get; set; }
    public string? FilePattern { get; set; }
    public string? Backdrop { get; set; }

    public static Settings Default => new() {
        FolderPath = DefaultFolderPath,
        ImageExtensions = DefaultImageExtensions,
        FilePattern = DefaultFilePattern,
        Backdrop = DefaultBackdrop
    };
}
