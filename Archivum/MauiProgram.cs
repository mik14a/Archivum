using Archivum.Contracts.Repositories;
using Archivum.Pages;
using Archivum.Repositories;
using Archivum.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Hosting;
using System.IO;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;

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
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
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
            .AddSingleton<IMangaRepository, LocalMangaRepository>()
            .AddSingleton<MangasViewModel>()
            .AddSingleton<MangasPage>();

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

    static void EnsureInitializeSettings(Models.Settings settings) {
        settings.FolderPath ??= Models.Settings.Default.FolderPath;
        settings.FilePattern ??= Models.Settings.Default.FilePattern;
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
}
