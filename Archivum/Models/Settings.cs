using System;
using System.IO;

namespace Archivum.Models;

public class Settings
{
    public string FolderPath { get; set; } = DefaultFolderPath;
    public string? FilePattern { get; set; }
    public string? Backdrop { get; set; }

    public static Settings Default => new() {
        FolderPath = DefaultFolderPath,
        FilePattern = $"[{Manga.AuthorPattern}] {Manga.TitlePattern} {Manga.VolumePattern}",
        Backdrop = nameof(Controls.SystemBackdrop.Mica)
    };

    public static string DefaultFolderPath
        => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(Archivum));
}