using Archivum.Contracts.Repositories;
using Archivum.Models;

namespace Archivum.Repositories;

public class LocalMangaRepository : IMangaRepository
{
    public LocalMangaRepository(string path) {
        _path = path;
    }

    public async Task<IEnumerable<Manga>> GetAllAsync() {
        var directory = new DirectoryInfo(_path);
        var files = directory.EnumerateFiles("*.zip", SearchOption.AllDirectories);
        return files.Select(ToMangaFile);

        static Manga ToMangaFile(FileInfo file) => new() {
            IsDirectory = false,
            Title = file.Name,
            Path = file.FullName,
            Created = file.CreationTime,
            Modified = file.LastWriteTime,
            Size = file.Length,
        };
    }

    private readonly string _path;
}
