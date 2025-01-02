using System.Collections.ObjectModel;
using Archivum.Models;
using CommunityToolkit.Mvvm.ComponentModel;

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
    public partial int Index { get; set; } = -1;
    [ObservableProperty]
    public partial ImageSource? Image { get; set; }

    public ObservableCollection<byte[]> Images { get; } = [];

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
            Images.Add(memoryStream.ToArray());
        }
    }

    partial void OnIndexChanged(int value) {
        if (value < 0 || Images.Count <= value) return;
        Image = ImageSource.FromStream(() => new MemoryStream(Images[value]));
    }
}
