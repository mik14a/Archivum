using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using Microsoft.Extensions.Options;

namespace Archivum.Repositories;

public class LocalMangaRepository : IMangaRepository
{
    public static readonly string CacheFileName = $"{nameof(Archivum)}.json";

    public LocalMangaRepository(IOptions<Models.Settings> setting) {
        _setting = setting.Value;
        _cacheFile = Path.Combine(_setting.FolderPath, CacheFileName);
    }

    public async Task<IEnumerable<Models.Manga>> GetAllAsync() {
        await _semaphore.WaitAsync();
        try {
            var directory = new DirectoryInfo(_setting.FolderPath);
            if (directory.Exists) {
                var files = directory.EnumerateFiles("*.zip", SearchOption.AllDirectories);
                foreach (var file in files) {
                    if (_mangas.ContainsKey(file.FullName)) continue;
                    var manga = Models.Manga.CreateFrom(file, _setting.FilePattern!);
                    _mangas.Add(file.FullName, manga);
                }
            }
            return [.. _mangas.Values];
        } finally {
            _semaphore.Release();
        }
    }

    public async Task LoadCacheAsync() {
        await _semaphore.WaitAsync();
        try {
            if (!File.Exists(_cacheFile)) return;
            var json = await File.ReadAllTextAsync(_cacheFile);
            var mangas = JsonSerializer.Deserialize<Models.Manga[]>(json);
            foreach (var manga in mangas) {
                _mangas.Add(manga.Path, manga);
            }
        } finally {
            _semaphore.Release();
        }
    }

    public async Task SaveCacheAsync() {
        var mangas = _mangas.Values.ToList();
        var json = JsonSerializer.Serialize(mangas, _jsonSerializerOptions);
        await File.WriteAllTextAsync(_cacheFile, json);
    }

    readonly Models.Settings _setting;
    readonly string _cacheFile;
    readonly Dictionary<string, Models.Manga> _mangas = [];

    readonly SemaphoreSlim _semaphore = new(1);

    static readonly JsonSerializerOptions _jsonSerializerOptions = new() {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true
    };
}
