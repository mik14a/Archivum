using System.Threading.Tasks;
using Archivum.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace Archivum.Pages;

public partial class AuthorPage : ContentPage
{
    public AuthorViewModel Model => _model;

    public int ColumnSpan { get; private set; } = 1;

    public AuthorPage(AuthorViewModel model) {
        _model = model;
        InitializeComponent();
        Title = model.Name;
        BindingContext = this;

#if WINDOWS || MACCATALYST
        _Page.SizeChanged += PageSizeChanged;
#endif
    }

#if WINDOWS || MACCATALYST
    void PageSizeChanged(object? sender, System.EventArgs e) {
        ColumnSpan = ((int)_Page.Width + 159) / 160;
        OnPropertyChanged(nameof(ColumnSpan));
    }
#endif

    protected override async void OnAppearing() {
        await _model.SyncAsync();
        base.OnAppearing();
    }

    [RelayCommand]
    async Task SelectMangaAsync(MangaViewModel mangaViewModel) {
        if (mangaViewModel is null) return;
        await Navigation.PushAsync(new MangaPage(mangaViewModel));
        _CollectionView.SelectedItem = null;
    }

    [RelayCommand]
    async Task OpenPropertiesAsync(MangaViewModel mangaViewModel) {
        if (mangaViewModel is null) return;
        await Navigation.PushModalAsync(new Editor.MangaEditPage(mangaViewModel));
    }

    [RelayCommand]
    async Task CloseAsync() {
        await Navigation.PopAsync();
    }

    readonly AuthorViewModel _model;
}
