using System;

namespace Archivum.Models;

public class Author
{
    public required string Name { get; set; }
    public required int Count { get; set; }
    public required DateTime LastModified { get; set; }
}
