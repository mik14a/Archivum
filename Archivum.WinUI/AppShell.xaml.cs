using System;
using Archivum.Contracts.Repositories;
using Archivum.Controls;
using Archivum.Pages;
using Archivum.Repositories;
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
        Loaded += AppShellLoaded;
        Unloaded += AppShellUnloaded;
        _repository = App.GetService<IMangaRepository>() as LocalMangaRepository;
        InitializeComponent();
    }

    void AppShellLoaded(object sender, RoutedEventArgs e) {
        try {
            _repository?.LoadLibraryAsync().ConfigureAwait(false);
        } catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    void AppShellUnloaded(object sender, RoutedEventArgs e) {
        try {
            _repository?.SaveLibraryAsync().ConfigureAwait(false);
        } catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine(ex);
        }
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
            else if ((string)selectedItem.Tag == "MangasPage") _ContentFrame.NavigateToType(typeof(MangasPage), null, frameNavigationOptions);
            else throw new NotImplementedException((string)selectedItem.Tag);
        }
    }

    readonly LocalMangaRepository? _repository;
}
