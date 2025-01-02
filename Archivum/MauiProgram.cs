using Archivum.Contracts.Repositories;
using Archivum.Pages;
using Archivum.Repositories;
using Archivum.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

        return builder.Build();
    }

    static void EnsureInitializeSettings(Models.Settings settings) {
        settings.FolderPath ??= Models.Settings.Default.FolderPath;
        settings.FilePattern ??= Models.Settings.Default.FilePattern;
    }

    static readonly string _settingFile;
}
