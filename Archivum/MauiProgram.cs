using Archivum.Contracts.Repositories;
using Archivum.Pages;
using Archivum.Repositories;
using Archivum.ViewModels;
using Microsoft.Extensions.Logging;

namespace Archivum;

public static class MauiProgram
{
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

        var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var archivumDocument = Path.Combine(folderPath, nameof(Archivum));
        if (!Directory.Exists(archivumDocument)) Directory.CreateDirectory(archivumDocument);

        builder.Services
            .AddSingleton<IMangaRepository>(new LocalMangaRepository(archivumDocument))
            .AddSingleton<MangasViewModel>()
            .AddSingleton<MangasPage>();

        return builder.Build();
    }
}
