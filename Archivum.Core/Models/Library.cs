using System;

namespace Archivum.Models;

/// <summary>
/// Represents a library containing collections of manga, authors, and titles with associated metadata.
/// </summary>
public class Library
{
    /// <summary>
    /// Gets the collection of manga entries in the library.
    /// </summary>
    public required Manga[] Mangas { get; init; }

    /// <summary>
    /// Gets the collection of authors registered in the library.
    /// </summary>
    public required Author[] Authors { get; init; }

    /// <summary>
    /// Gets the collection of titles available in the library.
    /// </summary>
    public required Title[] Titles { get; init; }

    /// <summary>
    /// Gets the timestamp when the library was last updated.
    /// </summary>
    public required DateTime LastUpdated { get; init; }
}
