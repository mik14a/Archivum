using System;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using Archivum.Repositories;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Archivum;

public partial class AppShell : Shell
{
    public AppShell(IMauiContext? context) {
        _context = context;
        _repository = _context?.Services.GetService<IMangaRepository>() as LocalMangaRepository;
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing() {
        base.OnAppearing();

        try {
            if (_repository != null) {
                await LocalMangaRepository.RequestStoragePermission();
                await _repository.LoadLibraryAsync();
            }
        } catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine($"Cache load failed: {ex.Message}");
        }
    }

    protected override async void OnDisappearing() {
        try {
            if (_repository != null) {
                await _repository.SaveLibraryAsync();
            }
        } catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine($"Cache save failed: {ex.Message}");
        }

        base.OnDisappearing();
    }

    [RelayCommand]
    async Task OpenSettings() {
        var settingsPage = _context?.Services.GetService<Pages.SettingsPage>();
        await Navigation.PushModalAsync(settingsPage);
    }

    readonly IMauiContext? _context;
    readonly LocalMangaRepository? _repository;
}
