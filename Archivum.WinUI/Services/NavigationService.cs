using System;
using System.Linq;
using System.Threading.Tasks;
using Archivum.Contracts.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Archivum.Services;

public class NavigationService : INavigationService
{
    public void Initialize(NavigationView navigationView, Frame contentFrame) {
        _navigationView = navigationView;
        _contentFrame = contentFrame;
    }

    public void NavigateTo(string tag) {
        if (_navigationView == null) throw new InvalidOperationException();
        var item = _navigationView.MenuItems
            .OfType<NavigationViewItem>()
            .SingleOrDefault(x => x.Tag.ToString() == tag);
        if (item != null) {
            _navigationView.SelectedItem = item;
        }
    }

    public async Task<bool> PushAsync(Type sourcePageType, object parameter) {
        if (_navigationView == null) throw new InvalidOperationException();
        if (_contentFrame == null) throw new InvalidOperationException();
        _navigationView.SelectedItem = null;
        var options = new FrameNavigationOptions {
            IsNavigationStackEnabled = true,
        };
        return await Task.FromResult(_contentFrame.NavigateToType(sourcePageType, parameter, options));
    }

    public async Task PopAsync() {
        if (_contentFrame == null) throw new InvalidOperationException();
        if (_contentFrame.CanGoBack) {
            _contentFrame.GoBack();
        }
        await Task.CompletedTask;
    }

    NavigationView? _navigationView;
    Frame? _contentFrame;
}
