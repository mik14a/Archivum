using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;

namespace Archivum.ViewModels;

public partial class AuthorViewModel : ObservableObject
{
    [ObservableProperty]
    public partial ImageSource? Image { get; set; }
    [ObservableProperty]
    public partial string Name { get; set; }
    [ObservableProperty]
    public partial int Count { get; set; }
    [ObservableProperty]
    public partial DateTime LastModified { get; set; }

    public AuthorViewModel(Models.Author author) {
        Name = author.Name;
        Count = author.Count;
        LastModified = author.LastModified;
    }
}
