using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Archivum.Controls;
using Archivum.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;

namespace Archivum.ViewModels;

public partial class MangaViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool IsDirectory { get; set; }

    [ObservableProperty]
    public partial string Author { get; set; }
    [ObservableProperty]
    public partial string Title { get; set; }
    [ObservableProperty]
    public partial string Volume { get; set; }

    [ObservableProperty]
    public partial string Path { get; set; }
    [ObservableProperty]
    public partial DateTime Created { get; set; }
    [ObservableProperty]
    public partial DateTime Modified { get; set; }
    [ObservableProperty]
    public partial long Size { get; set; }

    [ObservableProperty]
    public partial int Pages { get; protected set; } = 0;
    [ObservableProperty]
    public partial int Index { get; protected set; } = -1;

    public ImageSource?[] Images => _images;

    public MangaViewModel(Manga model) {
        IsDirectory = false;
        Author = model.Author;
        Title = model.Title;
        Volume = model.Volume;
        Path = model.Path;
        Created = model.Created;
        Modified = model.Modified;
        Size = model.Size;
    }

    public async Task LoadAsync() {
        if (IsDirectory) return;

        using var archive = System.IO.Compression.ZipFile.OpenRead(Path);
        var imageFiles = archive.Entries
            .Where(e => e.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                       e.Name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                       e.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase));

        foreach (var entry in imageFiles) {
            using var stream = entry.Open();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            _imageSources.Add(memoryStream.ToArray());
        }

        Pages = 2;
        Index = 0;
    }

    public void SetSingleFrameView() {
        Pages = 1;
    }

    public void SetSpreadFrameView() {
        Pages = 2;
    }

    public void MoveToPreviousFrame() {
        Index = Math.Max(0, Index - 1);
    }

    public void MoveToNextFrame() {
        Index = Math.Min(_imageSources.Count - 1, Index + 1);
    }

    public void MoveToPreviousView() {
        Index = Math.Max(0, Index - Pages);
    }

    public void MoveToNextView() {
        Index = Math.Min(_imageSources.Count - Pages, Index + Pages);
    }

    partial void OnPagesChanged(int value) {
        if (value < 1 || 2 < value) return;
        _images = new ImageSource[value];
        if (Index < 0 || _imageSources.Count <= Index) return;
        for (var i = 0; i < value; i++) {
            System.Diagnostics.Debug.WriteLine($"_images[{i}] = _imageSources[{Index + i}]");
            _images[i] = new MemoryImageSource(_imageSources[Index + i]);
        }
        OnPropertyChanged(nameof(Images));
    }

    partial void OnIndexChanged(int value) {
        if (value < 0 || _imageSources.Count <= value) return;
        for (var i = 0; i < Pages; i++) {
            System.Diagnostics.Debug.WriteLine($"_images[{i}] = _imageSources[{value + i}]");
            _images[i] = new MemoryImageSource(_imageSources[value + i]);
        }
        OnPropertyChanged(nameof(Images));
    }

    ImageSource?[] _images = [];
    readonly List<byte[]> _imageSources = [];
}
