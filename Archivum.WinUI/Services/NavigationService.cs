using System;
using System.Linq;
using Archivum.Contracts.Services;
using Microsoft.UI.Xaml.Controls;

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

    public bool Navigate(Type sourcePageType, object parameter) {
        if (_navigationView == null) throw new InvalidOperationException();
        if (_contentFrame == null) throw new InvalidOperationException();
        _navigationView.SelectedItem = null;
        return _contentFrame.Navigate(sourcePageType, parameter);
    }

    NavigationView? _navigationView;
    Frame? _contentFrame;
}
