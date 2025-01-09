using System;
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

    public async Task<IEnumerable<Models.Manga>> GetMangasAsync() {
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

    public async Task<IEnumerable<Models.Author>> GetAuthorsAsync() {
        await _semaphore.WaitAsync();
        try {
            var authors = _mangas.Values
                .GroupBy(m => m.Author)
                .Select(g => new Models.Author {
                    Name = g.Key,
                    Count = g.Count(),
                    LastModified = g.Max(m => m.Modified)
                })
                .OrderBy(a => a.Name)
                .ToArray();

            foreach (var author in authors) {
                if (_authors.TryGetValue(author.Name, out var existing)) {
                    existing.Count = author.Count;
                    existing.LastModified = author.LastModified;
                } else {
                    _authors.Add(author.Name, author);
                }
            }
            return [.. _authors.Values];
        } finally {
            _semaphore.Release();
        }
    }

    public async Task<IEnumerable<Models.Title>> GetTitlesAsync() {
        await _semaphore.WaitAsync();
        try {
            var titles = _mangas.Values
                .GroupBy(m => m.Title)
                .Select(g => new Models.Title {
                    Name = g.Key,
                    Author = g.First().Author,
                    Count = g.Count(),
                    LastModified = g.Max(m => m.Modified)
                })
                .OrderBy(t => t.Name)
                .ToArray();

            foreach (var title in titles) {
                if (_titles.TryGetValue(title.Name, out var existing)) {
                    existing.Author = title.Author;
                    existing.Count = title.Count;
                    existing.LastModified = title.LastModified;
                } else {
                    _titles.Add(title.Name, title);
                }
            }
            return [.. _titles.Values];
        } finally {
            _semaphore.Release();
        }
    }

    public async Task LoadLibraryAsync() {
        await _semaphore.WaitAsync();
        try {
            if (!File.Exists(_cacheFile)) return;
            var json = await File.ReadAllTextAsync(_cacheFile);
            var library = JsonSerializer.Deserialize<Models.Library>(json);
            if (library is null) return;

            foreach (var manga in library.Mangas) {
                _mangas.Add(manga.Path, manga);
            }
            foreach (var author in library.Authors) {
                _authors.Add(author.Name, author);
            }
            foreach (var title in library.Titles) {
                _titles.Add(title.Name, title);
            }

            _lastUpdated = library.LastUpdated;
        } finally {
            _semaphore.Release();
        }
    }

    public async Task SaveLibraryAsync() {

        var library = new Models.Library {
            Mangas = [.. _mangas.Values],
            Authors = [.. _authors.Values],
            Titles = [.. _titles.Values],
            LastUpdated = DateTime.Now
        };

        var json = JsonSerializer.Serialize(library, _jsonSerializerOptions);
        await File.WriteAllTextAsync(_cacheFile, json);
    }

    readonly Models.Settings _setting;
    readonly string _cacheFile;
    readonly Dictionary<string, Models.Manga> _mangas = [];
    readonly Dictionary<string, Models.Author> _authors = [];
    readonly Dictionary<string, Models.Title> _titles = [];
    DateTime _lastUpdated;

    readonly SemaphoreSlim _semaphore = new(1);

    static readonly JsonSerializerOptions _jsonSerializerOptions = new() {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true
    };
}
