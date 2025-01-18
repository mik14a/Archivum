using System.Threading.Tasks;
using Archivum.Contracts.Services;
using Archivum.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Archivum.Pages;

/// <summary>
/// A Settings page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsPage : Page
{
    public SettingsViewModel Model { get; }

    public SettingsPage() {
        _navigationService = App.GetService<INavigationService>();
        Model = App.GetService<SettingsViewModel>();
        InitializeComponent();
        DataContext = this;
    }

    [RelayCommand]
    async Task SelectFolderAsync() {
        await Task.CompletedTask;
    }

    [RelayCommand]
    async Task SaveAsync() {
        var setting = Model.Apply();
        await App.SaveSettings(setting);
        await _navigationService.PopAsync();
    }

    [RelayCommand]
    async Task CancelAsync() {
        Model.Cancel();
        await _navigationService.PopAsync();
    }

    readonly INavigationService _navigationService;
}
