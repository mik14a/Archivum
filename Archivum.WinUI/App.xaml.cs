using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using Archivum.Contracts.Services;
using Archivum.Repositories;
using Archivum.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.UI.Xaml;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Archivum;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Static constructor that initializes the settings file path.
    /// Attempts to use the application's process directory, falling back to the current directory if unavailable.
    /// The settings file 'settings.json' will be located in this directory.
    /// </summary>
    static App() {
        var path = Path.GetDirectoryName(Environment.ProcessPath) ?? Environment.CurrentDirectory;
        _settingFile = Path.Combine(path, "settings.json");
    }

    /// <summary>
    /// Gets the current application instance as a strongly-typed App object.
    /// This property shadows the base Application.Current property to provide direct access to App-specific functionality.
    /// </summary>
    /// <returns>The current application instance cast to App type.</returns>
    /// <remarks>
    /// This property uses the null-forgiving operator (!) as the cast is guaranteed to succeed
    /// when accessed within the application's context.
    /// </remarks>
    public static new App Current => (Application.Current as App)!;

    /// <summary>
    /// Retrieves a service of the specified type from the application's dependency injection container.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve. Must be a reference type.</typeparam>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the requested service type is not registered in the dependency injection container.</exception>
    /// <remarks>
    /// This method provides a convenient way to access services registered in the application's DI container.
    /// Services must be registered in the ConfigureServices method during application startup.
    /// </remarks>
    public static T GetService<T>() where T : class {
        var services = Current._host.Services;
        return services.GetService<T>() is T service
            ? service
            : throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
    }

    /// <summary>
    /// Asynchronously saves the application settings to the settings file.
    /// </summary>
    /// <param name="settings">The settings object containing the configuration to save.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    /// <remarks>
    /// The settings are serialized to JSON format using the application's configured JSON serializer options,
    /// which include full Unicode support and pretty printing. The resulting JSON is written to the
    /// settings file specified during application initialization.
    /// </remarks>
    public static async Task SaveSettings(Models.Settings settings) {
        var json = JsonSerializer.Serialize(settings, _jsonSerializerOptions);
        await File.WriteAllTextAsync(_settingFile, json);
    }

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App() {
        InitializeComponent();

        var builder = Host.CreateApplicationBuilder();

        builder.Configuration
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile(_settingFile, optional: true);

        builder.Services
            .Configure<Models.Settings>(Models.Settings.EnsureInitializeSettings)
            .Configure<Models.Settings>(builder.Configuration)
            .AddSingleton<IBackdropSelectorService, BackdropSelectorService>()
            .AddSingleton<IThemeSelectorService, ThemeSelectorService>()
            .AddSingleton<IMangaRepository, LocalMangaRepository>()
            .AddSingleton<ViewModels.SettingsViewModel>()
            .AddSingleton<ViewModels.MangasViewModel>();

        _host = builder.Build();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args) {
        _window = new MainWindow {
            MinWidth = 320, MinHeight = 240,
            Content = new AppShell(),
            ExtendsContentIntoTitleBar = true
        };
        var shell = (AppShell)_window.Content;
        _window.SetTitleBar(shell.AppTitleBar);
        _window.SetWindowSize(800, 600);
        var setting = _host.Services.GetRequiredService<IOptions<Models.Settings>>().Value!;
        var backdropText = setting.Backdrop ?? Models.Settings.DefaultBackdrop;
        var backdrop = Enum.TryParse(backdropText, out Controls.SystemBackdrop backdropType) ? backdropType : Controls.SystemBackdrop.Default;
        var themeText = setting.Theme ?? Models.Settings.DefaultTheme;
        var theme = Enum.TryParse(themeText, out Controls.SystemTheme themeType) ? themeType : Controls.SystemTheme.System;
        var backdropSelectorService = _host.Services.GetService<IBackdropSelectorService>() as BackdropSelectorService;
        backdropSelectorService?.InitializeAsync(_window, backdrop);
        var themeSelectorService = _host.Services.GetService<IThemeSelectorService>() as ThemeSelectorService;
        themeSelectorService?.InitializeAsync(_window, theme);
        _window.Activate();
    }

    readonly IHost _host;
    Window? _window;

    static readonly string _settingFile;
    static readonly JsonSerializerOptions _jsonSerializerOptions = new() {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true
    };
}
