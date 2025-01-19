using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using Archivum.Contracts.Services;
using Archivum.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Archivum.Pages;

/// <summary>
/// A Settings page that can be used on its own or navigated to within a Frame.
/// </summary>
[INotifyPropertyChanged]
public sealed partial class SettingsPage : Page
{
    public SettingsViewModel Model { get; }

    [ObservableProperty]
    public partial int ProgressArrangeMangas { get; private set; }

    public SettingsPage() {
        _navigationService = App.GetService<INavigationService>();
        _repository = App.GetService<IMangaRepository>();
        Model = App.GetService<SettingsViewModel>();
        InitializeComponent();
        DataContext = this;
    }

    [RelayCommand]
    async Task SelectFolderAsync() {
        await Task.CompletedTask;
    }

    [RelayCommand]
    async Task ArrangeMangasAsync() {
        var progress = new System.Progress<int>(value => ProgressArrangeMangas = value);
        await _repository.ReorganizeMangaFiles(Model.FolderPath, Model.FolderPattern, Model.FilePattern, progress);
        await _navigationService.PopAsync();
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
    readonly IMangaRepository _repository;
}
