using System;
using System.Diagnostics;

namespace Archivum.Models;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Title
{
    public required string Name { get; set; }
    public required string Author { get; set; }
    public required int Count { get; set; }
    public required DateTime LastModified { get; set; }
    public required string Cover { get; set; }

    private string GetDebuggerDisplay() {
        return $"[{Author}] {Name}";
    }
}
