using System;
using System.Threading.Tasks;
using Archivum.Contracts.Services;
using Archivum.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Archivum.Pages;

/// <summary>
/// An Manga page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MangaPage : Page
{
    public MangaViewModel? Model => _model;

    public MangaPage() {
        _navigationService = App.GetService<INavigationService>();
        InitializeComponent();
        DataContext = this;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e) {
        switch (e.NavigationMode) {
        case NavigationMode.New:
            _model = (MangaViewModel)e.Parameter;
            await _model.LoadAsync();
            break;
        case NavigationMode.Back:
            _model = (MangaViewModel)e.Parameter;
            break;
        }
        base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e) {
        switch (e.NavigationMode) {
        case NavigationMode.Back:
            _model?.Unload();
            break;
        }
        base.OnNavigatedFrom(e);
    }

    [RelayCommand]
    void OpenProperties() {
    }

    [RelayCommand]
    async Task CloseAsync() {
        await _navigationService.PopAsync();
    }

    readonly INavigationService _navigationService;

    MangaViewModel? _model;
}
