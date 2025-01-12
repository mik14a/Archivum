using System;
using System.IO;

namespace Archivum.Models;

/// <summary>
/// Represents application-wide settings and configuration options.
/// This partial class contains static members and default values.
/// </summary>
public partial class Settings
{
    /// <summary>
    /// Gets the default folder path for storing application data, located in the user's Documents folder.
    /// </summary>
    public static readonly string DefaultFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(Archivum));

    /// <summary>
    /// Gets the default list of supported image file extensions, separated by semicolons.
    /// </summary>
    public static readonly string DefaultImageExtensions = ".jpg;.jpeg;.png";

    /// <summary>
    /// Gets the default pattern for organizing folders, using author and title placeholders.
    /// </summary>
    public static readonly string DefaultFolderPattern = $"[{Manga.AuthorPattern}] {Manga.TitlePattern}";

    /// <summary>
    /// Gets the default pattern for naming files, using author, title and volume placeholders.
    /// </summary>
    public static readonly string DefaultFilePattern = $"[{Manga.AuthorPattern}] {Manga.TitlePattern} {Manga.VolumePattern}";

    /// <summary>
    /// Gets the default system backdrop effect type.
    /// </summary>
    public static readonly string DefaultBackdrop = nameof(Controls.SystemBackdrop.Mica);

    /// <summary>
    /// Gets a new Settings instance initialized with default values.
    /// </summary>
    public static Settings Default => new() {
        FolderPath = DefaultFolderPath,
        ImageExtensions = DefaultImageExtensions,
        FolderPattern = DefaultFolderPattern,
        FilePattern = DefaultFilePattern,
        Backdrop = DefaultBackdrop
    };
}
