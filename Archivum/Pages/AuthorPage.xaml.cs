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

    protected override void OnAppearing() {
        _model.SyncAsync();
        base.OnAppearing();
    }

    [RelayCommand]
    async Task SelectMangaAsync(MangaViewModel mangaViewModel) {
        if (mangaViewModel is null) return;
        await Navigation.PushAsync(new MangaPage(mangaViewModel));
    }

    readonly AuthorViewModel _model;
}
