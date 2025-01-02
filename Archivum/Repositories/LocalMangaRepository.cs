using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using Archivum.Models;
using Microsoft.Extensions.Options;

namespace Archivum.Repositories;

public class LocalMangaRepository : IMangaRepository
{

    public LocalMangaRepository(IOptions<Settings> setting) {
        _setting = setting.Value;
    }

    public async Task<IEnumerable<Manga>> GetAllAsync() {
        var directory = new DirectoryInfo(_setting.FolderPath);
        if (!directory.Exists) return [];

        var files = directory.EnumerateFiles("*.zip", SearchOption.AllDirectories);
        return files.Select(ToMangaFile);

        Manga ToMangaFile(FileInfo file) {
            var fileName = Path.GetFileNameWithoutExtension(file.Name);
            var pattern = Regex.Escape(_setting.FilePattern!)
                .Replace(Manga.AuthorPattern, "(?<author>.+)")
                .Replace(Manga.TitlePattern, "(?<title>.+)")
                .Replace(Manga.VolumePattern, "(?<volume>.+)");
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var match = regex.Match(fileName);
            var author = match.Groups.TryGetValue("author", out var authorValue) ? authorValue.Value : null;
            var title = match.Groups.TryGetValue("title", out var titleValue) ? titleValue.Value : null;
            var volume = match.Groups.TryGetValue("volume", out var volumeValue) ? volumeValue.Value : null;
            return new() {
                Author = author ?? string.Empty, Title = title ?? fileName, Volume = volume ?? string.Empty,
                Path = file.FullName, Created = file.CreationTime, Modified = file.LastWriteTime, Size = file.Length,
            };
        }
    }

    readonly Settings _setting;
}
