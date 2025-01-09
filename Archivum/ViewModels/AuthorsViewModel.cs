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

    public async Task SyncAsync() {
        var authors = await _repository.GetAuthorsAsync();
        foreach (var author in authors) {
            var existing = Authors.FirstOrDefault(a => a.Name == author.Name);
            if (existing != null) {
                existing.Count = author.Count;
                existing.LastModified = author.LastModified;
            } else {
                Authors.Add(new(author, _repository, _settings));
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
