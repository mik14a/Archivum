using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Archivum.Contracts.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;

namespace Archivum.ViewModels;

public partial class TitlesViewModel : ObservableObject
{
    public ObservableCollection<TitleViewModel> Titles { get; } = [];

    public TitlesViewModel(IMangaRepository repository, IOptions<Models.Settings> settings) {
        _repository = repository;
        _settings = settings.Value!;
        Titles.CollectionChanged += TitlesCollectionChanged;
    }

    public async Task SyncAsync(bool sortItems = false) {
        var titles = await _repository.GetTitlesAsync();

        var removed = Titles.Where(title => !titles.Any(t => t.Name == title.Name)).ToArray();
        foreach (var title in removed) {
            Titles.Remove(title);
        }

        var duplicates = Titles.GroupBy(t => t.Name).Where(g => 1 < g.Count()).SelectMany(g => g.Skip(1)).ToArray();
        foreach (var title in duplicates) {
            Titles.Remove(title);
        }

        foreach (var title in titles) {
            var existing = Titles.FirstOrDefault(t => t.Name == title.Name);
            if (existing != null) {
                existing.Author = title.Author;
                existing.Count = title.Count;
                existing.LastModified = title.LastModified;
            } else {
                Titles.Add(new(title, _repository, _settings));
            }
        }

        if (sortItems) {
            for (var i = 0; i < Titles.Count - 1; i++) {
                for (var j = 0; j < Titles.Count - i - 1; j++) {
                    var compareTitle = string.Compare(Titles[j].Name, Titles[j + 1].Name);
                    if (0 < compareTitle) {
                        (Titles[j + 1], Titles[j]) = (Titles[j], Titles[j + 1]);
                    } else if (compareTitle == 0) {
                        var compareAuthor = string.Compare(Titles[j].Author, Titles[j + 1].Author);
                        if (0 < compareAuthor) {
                            (Titles[j + 1], Titles[j]) = (Titles[j], Titles[j + 1]);
                        }
                    }
                }
            }
        }
    }

    void TitlesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add) {
            foreach (var title in e.NewItems!.OfType<TitleViewModel>()) {
                title.LoadCoverAsync();
            }
        }
    }

    readonly IMangaRepository _repository;
    readonly Models.Settings _settings;
}
