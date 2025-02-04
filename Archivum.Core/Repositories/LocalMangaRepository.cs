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
using Microsoft.Maui.ApplicationModel;

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
            return GetMangas();
        } finally {
            _semaphore.Release();
        }
    }

    public async Task<IEnumerable<Models.Manga>> GetMangasFromAuthorAsync(string author) {
        await _semaphore.WaitAsync();
        try {
            return _mangas.Values.Where(m => m.Author == author).ToArray();
        } finally {
            _semaphore.Release();
        }
    }

    public async Task<IEnumerable<Models.Manga>> GetMangasFromTitleAsync(string title) {
        await _semaphore.WaitAsync();
        try {
            return _mangas.Values.Where(m => m.Title == title).ToArray();
        } finally {
            _semaphore.Release();
        }
    }

    public async Task<IEnumerable<Models.Author>> GetAuthorsAsync() {
        await _semaphore.WaitAsync();
        try {
            return GetAuthors();
        } finally {
            _semaphore.Release();
        }
    }

    public async Task<IEnumerable<Models.Title>> GetTitlesAsync() {
        await _semaphore.WaitAsync();
        try {
            return GetTitles();
        } finally {
            _semaphore.Release();
        }
    }

    public async Task RebuildLibraryAsync() {
        await _semaphore.WaitAsync();
        try {
            _mangas.Clear();
            _authors.Clear();
            _titles.Clear();
            _ = GetMangas();
            _ = GetAuthors();
            _ = GetTitles();
            await SaveLibraryAsync();
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
                _mangas.TryAdd(manga.Path, manga);
            }
            foreach (var author in library.Authors) {
                _authors.TryAdd(author.Name, author);
            }
            foreach (var title in library.Titles) {
                _titles.TryAdd(title.Name, title);
            }

            _lastUpdated = library.LastUpdated;
        } finally {
            _semaphore.Release();
        }
    }

    public async Task ReorganizeMangaFiles(string folderPath, string folderPattern, string filePattern, IProgress<int> progress) {

        // collect all files to move
        var filesToMove = new Dictionary<Models.Manga, string>();
        foreach (var manga in _mangas.Values) {
            var filePath = BuildMangaFilePath(manga, folderPath, folderPattern, filePattern);
            if (ShouldMoveFile(manga.Path, filePath)) {
                filesToMove.Add(manga, filePath);
            }
        }

        foreach (var (index, (manga, newFilePath)) in filesToMove.Index()) {
            var directoryName = Path.GetDirectoryName(newFilePath);
            if (!Directory.Exists(directoryName)) {
                Directory.CreateDirectory(directoryName!);
            }
            System.Diagnostics.Debug.WriteLine($"Move: {manga.Path} -> {newFilePath}");
            File.Move(manga.Path, newFilePath, overwrite: false);
            manga.Path = newFilePath;
            progress.Report((index + 1) * 100 / filesToMove.Count);
            await Task.Yield();
        }

        // file is not moved if the name is the same or the destination file already exists
        static bool ShouldMoveFile(string currentPath, string newPath) {
            return currentPath != newPath  // name is different
                   && File.Exists(currentPath)  // source file exists
                   && !File.Exists(newPath)  // destination file does not exist
                   && !string.IsNullOrWhiteSpace(Path.GetDirectoryName(newPath));  // destination folder name is valid
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

    IEnumerable<Models.Manga> GetMangas() {
        var directory = new DirectoryInfo(_setting.FolderPath);
        if (directory.Exists) {
            var files = directory.GetFiles("*.zip", SearchOption.AllDirectories);
            foreach (var manga in _mangas.Keys) {
                if (!files.Any(f => manga == f.FullName)) {
                    _mangas.Remove(manga);
                }
            }
            foreach (var file in files) {
                if (_mangas.ContainsKey(file.FullName)) continue;
                var manga = Models.Manga.CreateFrom(file, _setting.FilePattern!);
                _mangas.Add(file.FullName, manga);
            }
        }
        return [.. _mangas.Values];
    }

    IEnumerable<Models.Author> GetAuthors() {
        var authors = _mangas
            .Values
            .GroupBy(m => m.Author)
            .Select(g => new Models.Author {
                Name = g.Key,
                Count = g.Count(),
                LastModified = g.Max(m => m.Modified),
                Cover = $"{g.First().Path},0"
            })
            .OrderBy(a => a.Name).ToArray();

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
    }

    IEnumerable<Models.Title> GetTitles() {
        var titles = _mangas
            .Values
            .GroupBy(m => m.Title)
            .Select(g => new Models.Title {
                Name = g.Key,
                Author = g.First().Author,
                Count = g.Count(),
                LastModified = g.Max(m => m.Modified),
                Cover = $"{g.First().Path},0"
            })
            .OrderBy(t => t.Name).ToArray();

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
    }

    readonly Models.Settings _setting;
    readonly string _cacheFile;

    readonly Dictionary<string, Models.Manga> _mangas = [];
    readonly Dictionary<string, Models.Author> _authors = [];
    readonly Dictionary<string, Models.Title> _titles = [];
    DateTime _lastUpdated;

    readonly SemaphoreSlim _semaphore = new(1);

    public static async Task RequestStoragePermission() {
#if ANDROID
        if (OperatingSystem.IsAndroidVersionAtLeast(30)) { // Android 11 or later
            var activity = Platform.CurrentActivity;
            if (activity != null && !Android.OS.Environment.IsExternalStorageManager) {
                var intent = new Android.Content.Intent(Android.Provider.Settings.ActionManageAllFilesAccessPermission);
                activity.StartActivity(intent);
            }
        }
#endif
        await Permissions.RequestAsync<Permissions.StorageRead>();
        await Permissions.RequestAsync<Permissions.StorageWrite>();
    }

    static string BuildMangaFilePath(Models.Manga manga, string folderPath, string folderPattern, string filePattern) {
        var folderName = ReplacePatterns(folderPattern, manga);
        var fileExtension = Path.GetExtension(manga.Path);
        var fileName = ReplacePatterns(filePattern, manga) + fileExtension;
        return Path.Combine(folderPath, folderName, fileName);

        static string ReplacePatterns(string pattern, Models.Manga manga) {
            return pattern
                .Replace(Models.Manga.AuthorPattern, manga.Author)
                .Replace(Models.Manga.TitlePattern, manga.Title)
                .Replace(Models.Manga.VolumePattern, manga.Volume);
        }
    }

    static readonly JsonSerializerOptions _jsonSerializerOptions = new() {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true
    };

}
