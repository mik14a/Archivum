using CommunityToolkit.Mvvm.ComponentModel;

namespace Archivum.ViewModels;

public partial class AuthorViewModel : ObservableObject
{
    public string Name { get; }
    public int Count { get; }

    public AuthorViewModel(string name, int count) {
        Name = name;
        Count = count;
    }
}
