using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Archivum.Models;

/// <summary>
/// Represents application-wide settings and configuration options.
/// </summary>
public partial class Settings
{
    /// <summary>
    /// Gets or sets the root folder path for the application data.
    /// </summary>
    public string FolderPath { get; set; } = DefaultFolderPath;

    /// <summary>
    /// Gets or sets the supported image file extensions, separated by semicolons.
    /// </summary>
    public string? ImageExtensions { get; set; }

    /// <summary>
    /// Gets or sets the pattern used for organizing folders.
    /// </summary>
    public string? FolderPattern { get; set; }

    /// <summary>
    /// Gets or sets the pattern used for naming files.
    /// </summary>
    public string? FilePattern { get; set; }

    /// <summary>
    /// Gets or sets the system backdrop effect type.
    /// </summary>
    public string? Backdrop { get; set; }

    /// <summary>
    /// Gets or sets the system theme type.
    /// </summary>
    public string? Theme { get; set; }

    /// <summary>
    /// Determines whether a ZIP archive entry represents an image file based on its extension.
    /// </summary>
    /// <param name="entry">The ZIP archive entry to check.</param>
    /// <returns>true if the entry is an image file; otherwise, false.</returns>
    public bool IsImageEntry(ZipArchiveEntry entry) {
        var imageExtensions = ImageExtensions!.Split(';', StringSplitOptions.RemoveEmptyEntries);
        return imageExtensions.Contains(Path.GetExtension(entry.Name), StringComparer.OrdinalIgnoreCase);
    }
}
