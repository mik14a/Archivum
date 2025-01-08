using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Archivum;

public partial class AppShell : Shell
{
    public AppShell(IMauiContext? context) {
        _context = context;
        InitializeComponent();
        BindingContext = this;
    }

    [RelayCommand]
    async Task OpenSettings() {
        var settingsPage = _context?.Services.GetService<Pages.SettingsPage>();
        await Navigation.PushModalAsync(settingsPage);
    }

    readonly IMauiContext? _context;
}
