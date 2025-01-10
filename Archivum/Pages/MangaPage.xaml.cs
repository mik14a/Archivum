using Archivum.ViewModels;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

#if WINDOWS
using Microsoft.UI.Xaml;
#endif

namespace Archivum.Pages;

public partial class MangaPage : ContentPage
{
    public MangaViewModel Model => _model;

    public MangaPage(MangaViewModel viewModel) {
        _model = viewModel;
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing() {
        await _model.LoadAsync();
        base.OnAppearing();
    }

#if WINDOWS
    protected override void OnHandlerChanged() {
        base.OnHandlerChanged();
        if (_Page.Handler?.PlatformView is UIElement page) {
            page.PointerWheelChanged += (s, e) => {
                var mouseWheelDelta = e.GetCurrentPoint((UIElement)s).Properties.MouseWheelDelta;
                if (0 < mouseWheelDelta) _model.MoveToPreviousViewCommand.Execute(null);
                else if (mouseWheelDelta < 0) _model.MoveToNextViewCommand.Execute(null);
                e.Handled = mouseWheelDelta != 0;
            };
        }
    }
#endif

    [RelayCommand]
    async Task CloseAsync() {
        await Navigation.PopAsync();
    }

    readonly MangaViewModel _model;
}
