using System.Threading.Tasks;
using Archivum.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace Archivum.Pages;

public partial class MangasPage : ContentPage
{
    public MangasViewModel Model => _model;

    public int ColumnSpan { get; private set; } = 1;

    public MangasPage(MangasViewModel viewModel) {
        _model = viewModel;
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

    protected override async void OnAppearing() {
        await _model.LoadAsync();
        base.OnAppearing();
    }

    [RelayCommand]
    async Task SelectMangaAsync(MangaViewModel mangaViewModel) {
        if (mangaViewModel is null) return;
        await Navigation.PushAsync(new MangaPage(mangaViewModel));
    }

    readonly MangasViewModel _model;
}
