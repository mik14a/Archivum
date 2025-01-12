using Microsoft.Extensions.Hosting;
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
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App() {
        InitializeComponent();

        var builder = Host.CreateApplicationBuilder();

        _host = builder.Build();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args) {
        _window = new MainWindow {
            MinWidth = 320, MinHeight = 240,
            Content = new AppShell(),
            ExtendsContentIntoTitleBar = true
        };
        var shell = (AppShell)_window.Content;
        _window.SetTitleBar(shell.AppTitleBar);
        _window.SetWindowSize(800, 600);
        _window.Activate();
    }

    readonly IHost _host;
    Window? _window;
}
