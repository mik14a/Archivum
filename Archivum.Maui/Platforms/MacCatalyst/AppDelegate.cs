using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Archivum.Platforms.MacCatalyst;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() {
        return MauiProgram.CreateMauiApp();
    }
}