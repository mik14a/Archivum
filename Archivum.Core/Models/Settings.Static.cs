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
#if WINDOWS
    public static readonly string DefaultFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(Archivum));
#elif ANDROID
    public static readonly string DefaultFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#else
    public static readonly string DefaultFolderPath = Path.GetDirectoryName(null) ?? throw new NotSupportedException();
#endif

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
    public static readonly string DefaultBackdrop = nameof(Controls.SystemBackdrop.Default);

    /// <summary>
    /// Gets the default system theme type.
    /// </summary>
    public static readonly string DefaultTheme = nameof(Controls.SystemTheme.System);

    /// <summary>
    /// Gets a new Settings instance initialized with default values.
    /// </summary>
    public static Settings Default => new() {
        FolderPath = DefaultFolderPath,
        ImageExtensions = DefaultImageExtensions,
        FolderPattern = DefaultFolderPattern,
        FilePattern = DefaultFilePattern,
        Backdrop = DefaultBackdrop,
        Theme = DefaultTheme
    };

    /// <summary>
    /// Ensures that all settings properties have valid values by initializing any null properties with their corresponding default values.
    /// </summary>
    /// <param name="settings">The Settings instance to initialize.</param>
    /// <remarks>
    /// This method performs a null-coalescing assignment for each property, using the values from <see cref="Default"/> as fallbacks.
    /// Properties that already have non-null values are left unchanged.
    /// </remarks>
    public static void EnsureInitializeSettings(Settings settings) {
        settings.FolderPath ??= Settings.Default.FolderPath;
        settings.ImageExtensions ??= Settings.Default.ImageExtensions;
        settings.FolderPattern ??= Settings.Default.FolderPattern;
        settings.FilePattern ??= Settings.Default.FilePattern;
        settings.Backdrop ??= Settings.Default.Backdrop;
        settings.Theme ??= Settings.Default.Theme;
    }
}
