using Archivum.Controls;
using Archivum.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Archivum;

/// <summary>
/// A AppShell page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AppShell : Shell
{
    public UIElement AppTitleBar => _AppTitleBar;
    public NavigationView NavigationView => _NavigationView;
    public Frame ContentFrame => _ContentFrame;

    public AppShell() {
        InitializeComponent();
    }

    void NavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
        if (args.IsSettingsSelected) {
            _ContentFrame.Navigate(typeof(SettingsPage), args.RecommendedNavigationTransitionInfo);
        } else {
            var frameNavigationOptions = new FrameNavigationOptions {
                TransitionInfoOverride = args.RecommendedNavigationTransitionInfo,
                IsNavigationStackEnabled = false
            };
            var selectedItem = (NavigationViewItem)args.SelectedItem;
            if ((string)selectedItem.Tag == "HomePage") _ContentFrame.NavigateToType(typeof(HomePage), null, frameNavigationOptions);
        }
    }
}
