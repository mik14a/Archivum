using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Archivum.Models;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Author
{
    public required string Name { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Favorite { get; set; }
    public required int Count { get; set; }
    public required DateTime LastModified { get; set; }
    public required string Cover { get; set; }

    private string GetDebuggerDisplay() {
        return $"[{Name}]";
    }
}
