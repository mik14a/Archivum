using System;
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
        var mangas = await _repository.GetAllAsync();
        var authors = mangas
            .GroupBy(m => m.Author)
            .Select(g => new AuthorViewModel(g.Key, g.Count()))
            .OrderBy(a => a.Name);

        Authors.Clear();
        foreach (var author in authors) {
            Authors.Add(author);
        }
    }

    readonly IMangaRepository _repository;
}
