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
        var mangas = await _repository.GetMangasAsync();
        foreach (var manga in mangas) {
            var folderName = Model.FolderPattern!
                .Replace(Models.Manga.AuthorPattern, manga.Author)
                .Replace(Models.Manga.TitlePattern, manga.Title)
                .Replace(Models.Manga.VolumePattern, manga.Volume);
            var fileExtension = Path.GetExtension(manga.Path);
            var fileName = Model.FilePattern!
                .Replace(Models.Manga.AuthorPattern, manga.Author)
                .Replace(Models.Manga.TitlePattern, manga.Title)
                .Replace(Models.Manga.VolumePattern, manga.Volume)
                + fileExtension;
            var filePath = Path.Combine(Model.FolderPath, folderName, fileName);
            if (filePath != manga.Path && File.Exists(manga.Path) && !File.Exists(filePath)) {
                var fileDirectory = Path.GetDirectoryName(filePath);
                if (fileDirectory != null) {
                    if (!Directory.Exists(fileDirectory)) Directory.CreateDirectory(fileDirectory);
                    File.Move(manga.Path, filePath, overwrite: false);
                    manga.Path = filePath;
                }
            }
        }
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

    readonly IMangaRepository _repository;
    readonly IFolderPicker _folderPicker;
}
