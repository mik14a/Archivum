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

    public async Task<IEnumerable<Models.Manga>> GetMangasFromAuthorAsync(string author) {
        await _semaphore.WaitAsync();
        try {
            return [.. _mangas.Values.Where(m => m.Author == author)];
        } finally {
            _semaphore.Release();
        }
    }

    public async Task<IEnumerable<Models.Manga>> GetMangasFromTitleAsync(string title) {
        await _semaphore.WaitAsync();
        try {
            return [.. _mangas.Values.Where(m => m.Title == title)];
        } finally {
            _semaphore.Release();
        }
    }

    public async Task<IEnumerable<Models.Author>> GetAuthorsAsync() {
        await _semaphore.WaitAsync();
        try {
            var authors = GetAuthors(_mangas.Values).ToArray();

            // First, remove authors that are no longer in the manga list
            foreach (var author in _authors.Keys) {
                if (!authors.Any(a => author == a.Name)) {
                    _authors.Remove(author);
                }
            }

            // Then, add or update authors that are in the manga list
            foreach (var author in authors) {
                if (_authors.TryGetValue(author.Name, out var existing)) {
                    existing.Count = author.Count;
                    existing.LastModified = author.LastModified;
                    existing.Cover = author.Cover;
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
            var titles = GetTitles(_mangas.Values).ToArray();

            // First, remove titles that are no longer in the manga list
            foreach (var title in _titles.Keys) {
                if (!titles.Any(t => title == t.Name)) {
                    _titles.Remove(title);
                }
            }

            // Then, add or update titles that are in the manga list
            foreach (var title in titles) {
                if (_titles.TryGetValue(title.Name, out var existing)) {
                    existing.Author = title.Author;
                    existing.Count = title.Count;
                    existing.LastModified = title.LastModified;
                    existing.Cover = title.Cover;
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
            Mangas = [.. _mangas.Values.OrderBy(m => m.Author).ThenBy(m => m.Title).ThenBy(m => m.Volume)],
            Authors = [.. _authors.Values.OrderBy(a => a.Name)],
            Titles = [.. _titles.Values.OrderBy(t => t.Name).ThenBy(t => t.Author)],
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

    static IEnumerable<Models.Author> GetAuthors(IEnumerable<Models.Manga> mangas) {
        return mangas
            .GroupBy(m => m.Author)
            .Select(g => new Models.Author {
                Name = g.Key,
                Count = g.Count(),
                LastModified = g.Max(m => m.Modified),
                Cover = $"{g.First().Path},0"
            })
            .OrderBy(a => a.Name);
    }

    static IEnumerable<Models.Title> GetTitles(IEnumerable<Models.Manga> mangas) {
        return mangas
            .GroupBy(m => m.Title)
            .Select(g => new Models.Title {
                Name = g.Key,
                Author = g.First().Author,
                Count = g.Count(),
                LastModified = g.Max(m => m.Modified),
                Cover = $"{g.First().Path},0"
            })
            .OrderBy(t => t.Name);
    }

    static readonly JsonSerializerOptions _jsonSerializerOptions = new() {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true
    };

}
