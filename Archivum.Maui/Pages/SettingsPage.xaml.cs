using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using Archivum.ViewModels;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace Archivum.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsViewModel Model { get; }

    public SettingsPage(SettingsViewModel viewModel, IMangaRepository mangaRepository, IFolderPicker folderPicker) {
        Model = viewModel;
        _repository = mangaRepository;
        _folderPicker = folderPicker;
        InitializeComponent();
        BindingContext = this;
    }

    [RelayCommand]
    async Task ArrangeMangasAsync() {
        var progress = new System.Progress<int>(value => System.Diagnostics.Debug.WriteLine(value));
        await _repository.ReorganizeMangaFiles(Model.FolderPath, Model.FolderPattern, Model.FilePattern, progress);
        await Navigation.PopModalAsync();
    }

    [RelayCommand]
    async Task CancelAsync() {
        Model.Cancel();
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

    readonly IMangaRepository _repository;
    readonly IFolderPicker _folderPicker;
}
