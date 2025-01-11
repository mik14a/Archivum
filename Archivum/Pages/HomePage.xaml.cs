using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using Archivum.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using Microsoft.Maui.Controls;

namespace Archivum.Pages;

public partial class HomePage : ContentPage
{
    public int MangaCount { get; set; }
    public int AuthorCount { get; set; }
    public int TitleCount { get; set; }
    public double TotalSize { get; set; }

    public ObservableCollection<MangaViewModel> RecentMangas { get; } = [];
    public ObservableCollection<AuthorViewModel> FavoriteAuthors { get; } = [];
    public ObservableCollection<TitleViewModel> PopularTitles { get; } = [];

    public HomePage(IMangaRepository repository, IOptions<Models.Settings> settings) {
        _repository = repository;
        _settings = settings.Value!;
        InitializeComponent();
        BindingContext = this;

        RecentMangas.CollectionChanged += MangasCollectionChanged;
        FavoriteAuthors.CollectionChanged += AuthorsCollectionChanged;
        PopularTitles.CollectionChanged += TitlesCollectionChanged;
    }

    protected override async void OnAppearing() {
        await RefreshAsync();
        base.OnAppearing();
    }

    [RelayCommand]
    async Task RefreshAsync() {
        var mangas = await _repository.GetMangasAsync();
        MangaCount = mangas.Count();
        OnPropertyChanged(nameof(MangaCount));

        var authors = await _repository.GetAuthorsAsync();
        AuthorCount = authors.Count();
        OnPropertyChanged(nameof(AuthorCount));

        var titles = await _repository.GetTitlesAsync();
        TitleCount = titles.Count();
        OnPropertyChanged(nameof(TitleCount));

        TotalSize = mangas.Sum(m => m.Size) / (1024.0 * 1024.0 * 1024.0);
        OnPropertyChanged(nameof(TotalSize));

        RecentMangas.Clear();
        foreach (var manga in mangas.OrderByDescending(m => m.LastRead).Take(10)) {
            RecentMangas.Add(new(manga, _repository, _settings));
        }

        FavoriteAuthors.Clear();
        foreach (var author in authors.Where(a => a.Favorite).Take(10)) {
            FavoriteAuthors.Add(new(author, _repository, _settings));
        }

        PopularTitles.Clear();
        foreach (var title in mangas.GroupBy(m => m.Title).OrderByDescending(g => g.Max(m => m.LastRead)).Take(10)) {
            PopularTitles.Add(new(titles.Single(m => m.Name == title.Key), _repository, _settings));
        }
    }

    [RelayCommand]
    async Task SelectMangaAsync(MangaViewModel mangaViewModel) {
        if (mangaViewModel is null) return;
        await Navigation.PushAsync(new MangaPage(mangaViewModel));
        _MangasCollectionView.SelectedItem = null;
    }

    [RelayCommand]
    async Task SelectAuthorAsync(AuthorViewModel authorViewModel) {
        if (authorViewModel is null) return;
        await Navigation.PushAsync(new AuthorPage(authorViewModel));
        _AuthorsCollectionView.SelectedItem = null;
    }

    [RelayCommand]
    async Task SelectTitleAsync(TitleViewModel titleViewModel) {
        if (titleViewModel is null) return;
        await Navigation.PushAsync(new TitlePage(titleViewModel));
        _TitlesCollectionView.SelectedItem = null;
    }

    [RelayCommand]
    async Task OpenPropertiesAsync(ObservableObject viewModel) {
        ContentPage propertiesPage = viewModel switch {
            MangaViewModel mangaViewModel => new Editor.MangaEditPage(mangaViewModel),
            AuthorViewModel authorViewModel => new Editor.AuthorEditPage(authorViewModel),
            TitleViewModel titleViewModel => new Editor.TitleEditPage(titleViewModel),
            _ => throw new InvalidOperationException("Invalid view model type"),
        };
        await Navigation.PushModalAsync(propertiesPage);
    }

    void MangasCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add) {
            foreach (var item in e.NewItems!.OfType<MangaViewModel>()) {
                item.LoadCoverAsync();
            }
        }
    }
    void AuthorsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add) {
            foreach (var item in e.NewItems!.OfType<AuthorViewModel>()) {
                item.LoadCoverAsync();
            }
        }
    }

    void TitlesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add) {
            foreach (var item in e.NewItems!.OfType<TitleViewModel>()) {
                item.LoadCoverAsync();
            }
        }
    }

    readonly IMangaRepository _repository;
    readonly Models.Settings _settings;
}
