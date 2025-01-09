using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using Archivum.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;

namespace Archivum.ViewModels;

public partial class TitleViewModel : ObservableObject
{
    [ObservableProperty]
    public partial ImageSource? Image { get; set; }
    [ObservableProperty]
    public partial string Name { get; set; }
    [ObservableProperty]
    public partial string Author { get; set; }
    [ObservableProperty]
    public partial int Count { get; set; }
    [ObservableProperty]
    public partial DateTime LastModified { get; set; }

    [ObservableProperty]
    public partial string Cover { get; set; }

    public TitleViewModel(Models.Title model, IMangaRepository repository, Models.Settings settings) {
        Name = model.Name;
        Author = model.Author;
        Count = model.Count;
        LastModified = model.LastModified;
        Cover = model.Cover;
        _repository = repository;
        _settings = settings;
    }

    public async Task LoadCoverAsync() {
        var cover = Cover.Split(',');
        var path = cover.ElementAtOrDefault(0);
        int.TryParse(cover.ElementAtOrDefault(1), out var index);
        if (File.Exists(path)) {
            try {
                using var archive = ZipFile.OpenRead(path);
                var imageFile = archive.Entries.ElementAtOrDefault(index);
                if (imageFile != null) {
                    using var stream = imageFile.Open();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    Image = new MemoryImageSource(memoryStream.ToArray());
                }
            } catch { }
        }
    }

    readonly IMangaRepository _repository;
    readonly Models.Settings _settings;
}
