using System;
using System.Threading.Tasks;
using Archivum.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace Archivum.Pages;

public partial class HomePage : ContentPage
{
    public HomeViewModel Model { get; }

    public HomePage(HomeViewModel model) {
        Model = model;
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing() {
        await Model.SyncAsync();
        base.OnAppearing();
    }

    [RelayCommand]
    async Task RefreshAsync() {
        await Model.SyncAsync();
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
}
