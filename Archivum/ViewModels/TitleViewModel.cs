using CommunityToolkit.Mvvm.ComponentModel;

namespace Archivum.ViewModels;

public partial class TitleViewModel : ObservableObject
{
    public string Name { get; }
    public int Count { get; }

    public TitleViewModel(string name, int count) {
        Name = name;
        Count = count;
    }
}
