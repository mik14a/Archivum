using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Archivum.ViewModels;

public partial class TitlesViewModel : ObservableObject
{
    public ObservableCollection<TitleViewModel> Titles { get; } = [];

    public TitlesViewModel(IMangaRepository repository) {
        _repository = repository;
    }

    public async Task LoadAsync() {
        var titles = await _repository.GetTitlesAsync();
        foreach (var title in titles) {
            var existing = Titles.FirstOrDefault(t => t.Name == title.Name);
            if (existing != null) {
                existing.Author = title.Author;
                existing.Count = title.Count;
                existing.LastModified = title.LastModified;
            } else {
                Titles.Add(new TitleViewModel(title));
            }
        }
    }

    readonly IMangaRepository _repository;
}
