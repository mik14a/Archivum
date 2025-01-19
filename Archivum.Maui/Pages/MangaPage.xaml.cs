using System.Threading.Tasks;
using Archivum.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

#if WINDOWS
using Microsoft.UI.Xaml;
#endif

namespace Archivum.Pages;

public partial class MangaPage : ContentPage
{
    public MangaViewModel Model => _model;

    public bool FullFlame { get; set; } = true;

    public MangaPage(MangaViewModel viewModel) {
        _model = viewModel;
        InitializeComponent();
        BindingContext = this;

        _model.LoadAsync();
    }

    protected override void OnAppearing() {
        base.OnAppearing();
    }

    protected override void OnDisappearing() {
        _model.UpdateLastRead();
        base.OnDisappearing();
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
    async Task OpenPropertiesAsync() {
        await Navigation.PushModalAsync(new Editor.MangaEditPage(_model));
    }

    [RelayCommand]
    async Task CloseAsync() {
        await Navigation.PopAsync();
    }

    [RelayCommand]
    void ToggleViewFrame() {
        FullFlame = !FullFlame;
        OnPropertyChanged(nameof(FullFlame));
    }

    readonly MangaViewModel _model;
}
