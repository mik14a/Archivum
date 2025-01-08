using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Archivum;

public partial class App : Application
{
    public App() {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState) {
        var window = new Window(new AppShell(activationState?.Context)) {
            Title = nameof(Archivum),
            MinimumWidth = 320, MinimumHeight = 240,
            Width = 800, Height = 600
        };
        return window;
    }
}
