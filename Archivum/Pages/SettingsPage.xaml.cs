using System.Threading;
using System.Threading.Tasks;
using Archivum.ViewModels;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace Archivum.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsViewModel Model { get; }

    public SettingsPage(SettingsViewModel viewModel, IFolderPicker folderPicker) {
        Model = viewModel;
        _folderPicker = folderPicker;
        InitializeComponent();
        BindingContext = this;
    }

    [RelayCommand]
    async Task CancelAsync() {
        await Navigation.PopModalAsync();
    }

    [RelayCommand]
    async Task SaveAsync() {
        var settings = Model.Apply();
        await MauiProgram.SaveSettings(settings);
        await Navigation.PopModalAsync();
    }

    [RelayCommand]
    static async Task LaunchSettingsFolderAsync() {
        await MauiProgram.LaunchSettingsFolderAsync();
    }

    [RelayCommand]
    async Task BrowseFolderAsync() {
        var result = await _folderPicker.PickAsync(CancellationToken.None);
        if (result != null && result.IsSuccessful) {
            Model.FolderPath = result.Folder.Path;
        }
    }

    readonly IFolderPicker _folderPicker;
}
