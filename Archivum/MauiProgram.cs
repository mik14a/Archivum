using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using Archivum.Pages;
using Archivum.Repositories;
using Archivum.ViewModels;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using Archivum.Resources.Fonts;

#if WINDOWS10_0_17763_0_OR_GREATER
using Microsoft.Extensions.Options;
using Microsoft.Maui.Controls;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;
#endif

namespace Archivum;

public static class MauiProgram
{
    static MauiProgram() {
        var path = Path.GetDirectoryName(Environment.ProcessPath) ?? Environment.CurrentDirectory;
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
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile(_settingFile, optional: true);

        builder.Services
            .Configure<Models.Settings>(EnsureInitializeSettings)
            .Configure<Models.Settings>(builder.Configuration)
            .AddSingleton<IFolderPicker>(FolderPicker.Default)
            .AddSingleton<IMangaRepository, LocalMangaRepository>()
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
                => wndLifeCycleBuilder.OnWindowCreated(window
                    => {
                        var backdrop = Application.Current!.Windows[0]!.Page!.Handler!.MauiContext!.Services!.GetService<IOptions<Models.Settings>>()!.Value.Backdrop;
                        var systemBackdrop = Enum.TryParse<Controls.SystemBackdrop>(backdrop, out var parsedBackdrop) ? parsedBackdrop : Controls.SystemBackdrop.Mica;
                        window.SystemBackdrop = CreateSystemBackdrop(systemBackdrop);
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

    static void EnsureInitializeSettings(Models.Settings settings) {
        settings.FolderPath ??= Models.Settings.Default.FolderPath;
        settings.ImageExtensions ??= Models.Settings.Default.ImageExtensions;
        settings.FolderPattern ??= Models.Settings.Default.FolderPattern;
        settings.FilePattern ??= Models.Settings.Default.FilePattern;
        settings.Backdrop ??= Models.Settings.Default.Backdrop;
    }

#if WINDOWS10_0_17763_0_OR_GREATER
    static SystemBackdrop CreateSystemBackdrop(Controls.SystemBackdrop systemBackdrop) {
        return systemBackdrop switch {
            Controls.SystemBackdrop.Mica => new MicaBackdrop { Kind = MicaKind.Base },
            Controls.SystemBackdrop.MicaAlt => new MicaBackdrop { Kind = MicaKind.BaseAlt },
            Controls.SystemBackdrop.Acrylic => new DesktopAcrylicBackdrop(),
            _ => throw new System.NotImplementedException(),
        };
    }
#endif

    static readonly string _settingFile;
    static readonly JsonSerializerOptions _jsonSerializerOptions = new() {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true
    };
}
