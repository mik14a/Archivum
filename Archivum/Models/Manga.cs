namespace Archivum.Models;

public class Manga
{
    public string Title { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public long Size { get; set; }
}
