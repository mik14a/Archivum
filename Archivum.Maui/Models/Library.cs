using System;

namespace Archivum.Models;

public class Library
{
    public required Manga[] Mangas { get; init; }
    public required Author[] Authors { get; init; }
    public required Title[] Titles { get; init; }
    public required DateTime LastUpdated { get; init; }
}
