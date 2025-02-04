using System;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using Archivum.Contracts.Services;
using Archivum.Repositories;
using Archivum.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using WinRT.Interop;

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
        _repository = (LocalMangaRepository)App.GetService<IMangaRepository>();
        Model = App.GetService<SettingsViewModel>();
        InitializeComponent();
        DataContext = this;
    }

    [RelayCommand]
    async Task SelectFolderAsync() {
        var folderPicker = new FolderPicker {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };
        var hWnd = WindowNative.GetWindowHandle(App.Current.MainWindow);
        InitializeWithWindow.Initialize(folderPicker, hWnd);
        var folder = await folderPicker.PickSingleFolderAsync();
        if (folder != null) {
            Model.FolderPath = folder.Name;
        }
    }

    [RelayCommand]
    async Task ArrangeMangasAsync() {
        var progress = new System.Progress<int>(value => ProgressArrangeMangas = value);
        await _repository.ReorganizeMangaFiles(Model.FolderPath, Model.FolderPattern, Model.FilePattern, progress);
        await _navigationService.PopAsync();
    }

    [RelayCommand]
    static async Task OpenFolderAsync() {
        var settingFile = await StorageFile.GetFileFromPathAsync(App.GetSettingFile());
        var settingFolder = await StorageFolder.GetFolderFromPathAsync(System.IO.Path.GetDirectoryName(App.GetSettingFile()));
        var folderLauncherOptions = new FolderLauncherOptions();
        folderLauncherOptions.ItemsToSelect.Add(settingFile);
        await Launcher.LaunchFolderAsync(settingFolder, folderLauncherOptions);
    }

    [RelayCommand]
    async Task SaveAsync() {
        var invalidatedFilePattern = Model.FilePatternChanged;
        var setting = Model.Apply();
        if (invalidatedFilePattern) {
            var notificationDialog = new ContentDialog {
                Title = "データベース更新",
                Content = "ファイルパターンが変更されました。データベースの更新を行いますか？",
                PrimaryButtonText = "更新",
                CloseButtonText = "そのまま"
            };
            notificationDialog.XamlRoot = App.Current.MainWindow?.Content.XamlRoot;
            var result = await notificationDialog.ShowAsync();
            if (result == ContentDialogResult.Primary) {
                await _repository.RebuildLibraryAsync();
            }
        }
        await App.SaveSettings(setting);
        await _navigationService.PopAsync();
    }

    [RelayCommand]
    async Task CancelAsync() {
        Model.Cancel();
        await _navigationService.PopAsync();
    }

    readonly INavigationService _navigationService;
    readonly LocalMangaRepository _repository;
}
