using Microsoft.Maui;
using Microsoft.Maui.Hosting;

#if !DEBUG
using System;
using System.IO;
#endif

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Archivum.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App() {
        InitializeComponent();
#if !DEBUG
        UnhandledException += AppUnhandledException;
        DeleteUnhandledExceptionLog();
#endif
    }

#if !DEBUG
    static void AppUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e) {
        try {
            WriteUnhandledException(e);
        } catch (Exception ex) {
            Console.WriteLine($"Failed to save exception: {ex}");
        }
    }

    static void WriteUnhandledException(Microsoft.UI.Xaml.UnhandledExceptionEventArgs e) {
        var directory = Path.GetDirectoryName(Environment.ProcessPath) ?? Environment.CurrentDirectory;
        var exceptionText = $"Unhandled Exception: {e.Exception}\n\nStack Trace:\n{e.Exception.StackTrace}";
        var fileName = $"Archivum.UnhandledException_{DateTime.Now:yyyyMMdd_HHmmssfff}.txt";
        var filePath = Path.Combine(directory, fileName);
        File.WriteAllText(filePath, exceptionText);
    }

    static void DeleteUnhandledExceptionLog() {
        var directory = Path.GetDirectoryName(Environment.ProcessPath) ?? Environment.CurrentDirectory;
        var fileName = "Archivum.UnhandledException_*.txt";
        foreach (var file in Directory.GetFiles(directory, fileName)) {
            File.Delete(file);
        }
    }
#endif

    protected override MauiApp CreateMauiApp() {
        return MauiProgram.CreateMauiApp();
    }
}
