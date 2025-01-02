using Archivum.ViewModels;
using Microsoft.Maui.Controls;

#if WINDOWS
using Windows.Foundation;
using System.Collections.Generic;
using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
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
                if (0 < mouseWheelDelta) _model.MoveToPreviousView();
                else if (mouseWheelDelta < 0) _model.MoveToNextView();
                e.Handled = mouseWheelDelta != 0;
            };
            AddKeyboardAccelerator(page.KeyboardAccelerators, VirtualKey.Left, VirtualKeyModifiers.Control, (s, e) => _model.MoveToNextFrame());
            AddKeyboardAccelerator(page.KeyboardAccelerators, VirtualKey.Right, VirtualKeyModifiers.Control, (s, e) => _model.MoveToNextFrame());
            AddKeyboardAccelerator(page.KeyboardAccelerators, VirtualKey.Left, VirtualKeyModifiers.None, (s, e) => _model.MoveToNextView());
            AddKeyboardAccelerator(page.KeyboardAccelerators, VirtualKey.Right, VirtualKeyModifiers.None, (s, e) => _model.MoveToPreviousView());
            AddKeyboardAccelerator(page.KeyboardAccelerators, VirtualKey.Escape, VirtualKeyModifiers.None, (s, e) => Navigation.PopAsync());
            AddKeyboardAccelerator(page.KeyboardAccelerators, VirtualKey.Number1, VirtualKeyModifiers.None, (s, e) => _model.SetSingleFrameView());
            AddKeyboardAccelerator(page.KeyboardAccelerators, VirtualKey.Number2, VirtualKeyModifiers.None, (s, e) => _model.SetSpreadFrameView());

            static void AddKeyboardAccelerator(
                IList<Microsoft.UI.Xaml.Input.KeyboardAccelerator> keyboardAccelerators,
                VirtualKey key,
                VirtualKeyModifiers modifiers,
                TypedEventHandler<Microsoft.UI.Xaml.Input.KeyboardAccelerator, KeyboardAcceleratorInvokedEventArgs> invoked) {
                var keyboardAccelerator = new Microsoft.UI.Xaml.Input.KeyboardAccelerator { Key = key, Modifiers = modifiers };
                keyboardAccelerator.Invoked += invoked;
                keyboardAccelerators.Add(keyboardAccelerator);
            }
        }
    }
#endif

    readonly MangaViewModel _model;
}
