using System;

namespace Archivum.Models;

public class Title
{
    public required string Name { get; set; }
    public required string Author { get; set; }
    public required int Count { get; set; }
    public required DateTime LastModified { get; set; }
} 
