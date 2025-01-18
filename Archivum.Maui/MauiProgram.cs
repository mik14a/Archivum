using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using Archivum.Contracts.Services;
using Archivum.Pages;
using Archivum.Repositories;
using Archivum.Resources.Fonts;
using Archivum.Services;
using Archivum.ViewModels;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

#if DEBUG
using Microsoft.Extensions.Logging;
#endif

#if WINDOWS10_0_17763_0_OR_GREATER
using Microsoft.Extensions.Options;
using Microsoft.Maui.Controls;
#endif

namespace Archivum;

public static class MauiProgram
{
    static MauiProgram() {
#if WINDOWS
        var path = Path.GetDirectoryName(Environment.ProcessPath) ?? Directory.GetCurrentDirectory();
#elif ANDROID
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
#else
        var path = string.Empty ?? throw new NotSupportedException();
#endif
        _settingFile = Path.Combine(path, "settings.json");
    }

    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        if (!Directory.Exists(Models.Settings.DefaultFolderPath)) {
            Directory.CreateDirectory(Models.Settings.DefaultFolderPath);
        }

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(_settingFile, optional: true);

        builder.Services
            .Configure<Models.Settings>(Models.Settings.EnsureInitializeSettings)
            .Configure<Models.Settings>(builder.Configuration)
            .AddSingleton<IBackdropSelectorService, BackdropSelectorService>()
            .AddSingleton<IThemeSelectorService, ThemeSelectorService>()
            .AddSingleton<IFolderPicker>(FolderPicker.Default)
            .AddSingleton<IMangaRepository, LocalMangaRepository>()
            .AddSingleton<HomeViewModel>()
            .AddSingleton<MangasViewModel>()
            .AddSingleton<MangasPage>()
            .AddSingleton<AuthorsViewModel>()
            .AddSingleton<AuthorsPage>()
            .AddSingleton<TitlesViewModel>()
            .AddSingleton<TitlesPage>()
            .AddSingleton<SettingsViewModel>()
            .AddSingleton<SettingsPage>();

        builder.ConfigureLifecycleEvents(events => {
#if WINDOWS10_0_17763_0_OR_GREATER
            events.AddWindows(wndLifeCycleBuilder
                => wndLifeCycleBuilder.OnWindowCreated(async window
                    => {
                        var serviceProvider = Application.Current!.Windows[0]!.Page!.Handler!.MauiContext!.Services!;
                        if (serviceProvider.GetService<IBackdropSelectorService>() is BackdropSelectorService backdropSelectorService) {
                            var backdropText = serviceProvider.GetService<IOptions<Models.Settings>>()!.Value.Backdrop;
                            var backdrop = Enum.TryParse<Controls.SystemBackdrop>(backdropText, out var parsedBackdrop) ? parsedBackdrop : Controls.SystemBackdrop.Default;
                            await backdropSelectorService.InitializeAsync(window, backdrop);
                        }
                    }));
#endif
        });

        return builder.Build();
    }

    public static async Task LaunchSettingsFolderAsync() {
        var folderPath = Path.GetDirectoryName(_settingFile) ?? Environment.CurrentDirectory;
        await Launcher.OpenAsync(folderPath);
    }

    public static async Task SaveSettings(Models.Settings settings) {
        var json = JsonSerializer.Serialize(settings, _jsonSerializerOptions);
        await File.WriteAllTextAsync(_settingFile, json);
    }

    static readonly string _settingFile;
    static readonly JsonSerializerOptions _jsonSerializerOptions = new() {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true
    };
}
