using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;

namespace Archivum.ViewModels;

public partial class AuthorsViewModel : ObservableObject
{
    public ObservableCollection<AuthorViewModel> Authors { get; } = [];

    public AuthorsViewModel(IMangaRepository repository, IOptions<Models.Settings> settings) {
        _repository = repository;
        _settings = settings.Value!;
        Authors.CollectionChanged += AuthorsCollectionChanged;
    }

    public async Task SyncAsync(bool sortItems = false) {
        var authors = await _repository.GetAuthorsAsync();

        var removed = Authors.Where(author => !authors.Any(a => a.Name == author.Name)).ToArray();
        foreach (var author in removed) {
            Authors.Remove(author);
        }

        var duplicates = Authors.GroupBy(t => t.Name).Where(g => 1 < g.Count()).SelectMany(g => g.Skip(1)).ToArray();
        foreach (var author in duplicates) {
            Authors.Remove(author);
        }

        foreach (var author in authors) {
            var existing = Authors.FirstOrDefault(a => a.Name == author.Name);
            if (existing != null) {
                existing.Count = author.Count;
                existing.LastModified = author.LastModified;
            } else {
                Authors.Add(new(author, _repository,  _settings));
            }
        }

        if (sortItems) {
            for (var i = 0; i < Authors.Count - 1; i++) {
                for (var j = 0; j < Authors.Count - i - 1; j++) {
                    var compareAuthor = string.Compare(Authors[j].Name, Authors[j + 1].Name);
                    if (0 < compareAuthor) {
                        (Authors[j + 1], Authors[j]) = (Authors[j], Authors[j + 1]);
                    }
                }
            }
        }
    }

    void AuthorsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add) {
            foreach (var author in e.NewItems!.OfType<AuthorViewModel>()) {
                author.LoadCoverAsync();
            }
        }
    }

    readonly IMangaRepository _repository;
    readonly Models.Settings _settings;
}
