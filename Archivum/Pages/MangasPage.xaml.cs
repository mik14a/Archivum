using Archivum.ViewModels;

namespace Archivum.Pages;

public partial class MangasPage : ContentPage
{

    public MangasPage(MangasViewModel viewModel) {
        _viewModel = viewModel;
        InitializeComponent();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing() {
        await _viewModel.LoadAsync();
        base.OnAppearing();
    }

    protected override void OnDisappearing() {
        base.OnDisappearing();
    }

    private readonly MangasViewModel _viewModel;
}
