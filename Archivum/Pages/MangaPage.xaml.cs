using Archivum.ViewModels;

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
        _model.Index = 0;
        base.OnAppearing();
    }

    protected override void OnHandlerChanged() {
        base.OnHandlerChanged();

#if WINDOWS
        var view = _Page.Handler?.PlatformView as UIElement;
        if (view != null) {
            view.PointerWheelChanged += (s, e) => {
                var mouseWheelDelta = e.GetCurrentPoint(view).Properties.MouseWheelDelta;
                var p = e.GetCurrentPoint(view).Position;
                if (mouseWheelDelta != 0) {
                    _model.Index += 0 < mouseWheelDelta ? -1 : 1;
                    e.Handled = true;
                }
            };
        }
#endif
    }

    readonly MangaViewModel _model;
}
