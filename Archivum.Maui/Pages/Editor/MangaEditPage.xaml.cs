using System.Threading.Tasks;
using Archivum.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace Archivum.Pages.Editor;

public partial class MangaEditPage : ContentPage
{
    public MangaViewModel Model { get; }

    public MangaEditPage(MangaViewModel model) {
        Model = model;
        InitializeComponent();
        BindingContext = this;
    }

    [RelayCommand]
    async Task SaveAsync() {
        Model.ApplyEdit();
        await Navigation.PopModalAsync();
    }

    [RelayCommand]
    async Task CancelAsync() {
        Model.CancelEdit();
        await Navigation.PopModalAsync();
    }
}
