using System.Threading.Tasks;
using Archivum.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace Archivum.Pages;

public partial class MangasPage : ContentPage
{
    public MangasViewModel Model => _model;

    public MangasPage(MangasViewModel viewModel) {
        _model = viewModel;
        InitializeComponent();
        BindingContext = this;
    }

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
