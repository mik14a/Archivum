using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;

namespace Archivum.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty]
    public partial int MangaCount { get; set; }

    [ObservableProperty]
    public partial int AuthorCount { get; set; }

    [ObservableProperty]
    public partial int TitleCount { get; set; }

    [ObservableProperty]
    public partial long TotalSize { get; set; }

    public ObservableCollection<MangaViewModel> RecentMangas { get; } = [];
    public ObservableCollection<AuthorViewModel> FavoriteAuthors { get; } = [];
    public ObservableCollection<TitleViewModel> PopularTitles { get; } = [];

    public HomeViewModel(IMangaRepository repository, IOptions<Models.Settings> settings) {
        _repository = repository;
        _settings = settings.Value!;

        RecentMangas.CollectionChanged += MangasCollectionChanged;
        FavoriteAuthors.CollectionChanged += AuthorsCollectionChanged;
        PopularTitles.CollectionChanged += TitlesCollectionChanged;
    }

    public async Task SyncAsync() {
        var mangas = await _repository.GetMangasAsync();
        MangaCount = mangas.Count();

        var authors = await _repository.GetAuthorsAsync();
        AuthorCount = authors.Count();

        var titles = await _repository.GetTitlesAsync();
        TitleCount = titles.Count();

        TotalSize = mangas.Sum(m => m.Size);

        RecentMangas.Clear();
        foreach (var manga in mangas.Where(m => m.LastRead != DateTime.MinValue).OrderByDescending(m => m.LastRead).Take(10)) {
            RecentMangas.Add(new(manga, _repository, _settings));
        }

        FavoriteAuthors.Clear();
        foreach (var author in authors.Where(a => a.Favorite).Take(10)) {
            FavoriteAuthors.Add(new(author, _repository, _settings));
        }

        PopularTitles.Clear();
        foreach (var title in mangas.Where(m => m.LastRead != DateTime.MinValue).GroupBy(m => m.Title).OrderByDescending(g => g.Max(m => m.LastRead)).Take(10)) {
            PopularTitles.Add(new(titles.Single(m => m.Name == title.Key), _repository, _settings));
        }
    }

    async void MangasCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add) {
            foreach (var item in e.NewItems!.OfType<MangaViewModel>()) {
                await item.LoadCoverAsync().ConfigureAwait(false);
            }
        }
    }

    async void AuthorsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add) {
            foreach (var item in e.NewItems!.OfType<AuthorViewModel>()) {
                await item.LoadCoverAsync().ConfigureAwait(false);
            }
        }
    }

    async void TitlesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add) {
            foreach (var item in e.NewItems!.OfType<TitleViewModel>()) {
                await item.LoadCoverAsync().ConfigureAwait(false);
            }
        }
    }

    readonly IMangaRepository _repository;
    readonly Models.Settings _settings;
}
