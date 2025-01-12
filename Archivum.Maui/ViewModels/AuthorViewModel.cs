using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using Archivum.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace Archivum.ViewModels;

public partial class AuthorViewModel : ObservableObject
{
    [ObservableProperty]
    public partial ImageSource? Image { get; set; }
    [ObservableProperty]
    public partial bool IsFavorite { get; set; }
    [ObservableProperty]
    public partial string Name { get; set; }
    [ObservableProperty]
    public partial int Count { get; set; }
    [ObservableProperty]
    public partial DateTime LastModified { get; set; }
    [ObservableProperty]
    public partial string Cover { get; set; }

    public ObservableCollection<MangaViewModel> Mangas { get; } = [];

    public AuthorViewModel(Models.Author model, IMangaRepository repository, Models.Settings settings) {
        Name = model.Name;
        IsFavorite = model.Favorite;
        Count = model.Count;
        LastModified = model.LastModified;
        Cover = model.Cover;
        Mangas.CollectionChanged += MangasCollectionChanged;

        _model = model;
        _repository = repository;
        _settings = settings;
    }

    public async Task LoadCoverAsync() {
        if (Image != null) return;

        var cover = Cover.Split(',');
        var path = cover.ElementAtOrDefault(0);
        int.TryParse(cover.ElementAtOrDefault(1), out var index);
        if (File.Exists(path)) {
            try {
                using var archive = ZipFile.OpenRead(path);
                var imageFile = archive.Entries.Where(_settings.IsImageEntry).ElementAtOrDefault(index);
                if (imageFile != null) {
                    using var stream = imageFile.Open();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    Image = new MemoryImageSource(memoryStream.ToArray());
                }
            } catch { }
        }
    }

    public async Task SyncAsync() {
        var mangas = await _repository.GetMangasFromAuthorAsync(Name);

        var removed = Mangas.Where(manga => !mangas.Any(m => m.Path == manga.Path)).ToArray();
        foreach (var manga in removed) {
            Mangas.Remove(manga);
        }

        foreach (var manga in mangas) {
            var existing = Mangas.FirstOrDefault(m => m.Path == manga.Path);
            if (existing == null) {
                Mangas.Add(new(manga, _repository, _settings));
            }
        }
    }

    public void ApplyEdit() {
        _model.Name = Name;
        foreach (var manga in Mangas) {
            manga.Author = Name;
            manga.ApplyEdit();
        }
    }

    public void CancelEdit() {
        Name = _model.Name;
    }

    [RelayCommand]
    void ToggleFavorite() {
        IsFavorite = !IsFavorite;
        _model.Favorite = IsFavorite;
    }

    void MangasCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add) {
            foreach (var manga in e.NewItems!.OfType<MangaViewModel>()) {
                manga.LoadCoverAsync();
            }
        }
    }

    readonly Models.Author _model;
    readonly IMangaRepository _repository;
    readonly Models.Settings _settings;
}
