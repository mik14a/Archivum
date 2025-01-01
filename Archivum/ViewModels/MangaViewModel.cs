using Archivum.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Archivum.ViewModels;

public partial class MangaViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; }

    [ObservableProperty]
    public partial string Path { get; set; }

    [ObservableProperty]
    public partial DateTime Created { get; set; }

    [ObservableProperty]
    public partial DateTime Modified { get; set; }

    [ObservableProperty]
    public partial long Size { get; set; }

    public MangaViewModel(Manga model) {
        Title = model.Title;
        Path = model.Path;
        Created = model.Created;
        Modified = model.Modified;
        Size = model.Size;
    }
}
