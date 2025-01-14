using System;

namespace Archivum.Contracts.Services;

public interface INavigationService
{
    public void NavigateTo(string tag);
    public bool Navigate(Type sourcePageType, object parameter);
}
