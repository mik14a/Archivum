using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Archivum.ViewModels;

public partial class AuthorsViewModel : ObservableObject
{
    public ObservableCollection<AuthorViewModel> Authors { get; } = [];

    public AuthorsViewModel(IMangaRepository repository) {
        _repository = repository;
    }

    public async Task LoadAsync() {
        var authors = await _repository.GetAuthorsAsync();
        foreach (var author in authors) {
            var existing = Authors.FirstOrDefault(a => a.Name == author.Name);
            if (existing != null) {
                existing.Count = author.Count;
                existing.LastModified = author.LastModified;
            } else {
                Authors.Add(new AuthorViewModel(author));
            }
        }
    }

    readonly IMangaRepository _repository;
}
