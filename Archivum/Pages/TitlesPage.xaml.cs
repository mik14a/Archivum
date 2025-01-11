using System.Threading.Tasks;
using Archivum.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace Archivum.Pages;

public partial class TitlesPage : ContentPage
{
    public TitlesViewModel Model => _model;

    public int ColumnSpan { get; private set; } = 1;

    public TitlesPage(TitlesViewModel viewModel) {
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
        await _model.SyncAsync();
        base.OnAppearing();
    }

    [RelayCommand]
    async Task SelectTitle(TitleViewModel titleViewModel) {
        if (titleViewModel is null) return;
        await Navigation.PushAsync(new TitlePage(titleViewModel));
        _CollectionView.SelectedItem = null;
    }

    [RelayCommand]
    async Task OpenPropertiesAsync(TitleViewModel titleViewModel) {
        if (titleViewModel is null) return;
        await Navigation.PushModalAsync(new Editor.TitleEditPage(titleViewModel));
    }

    readonly TitlesViewModel _model;
}
