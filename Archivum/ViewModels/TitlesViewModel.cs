using System;
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
        var mangas = await _repository.GetAllAsync();
        var titles = mangas
            .GroupBy(m => m.Title)
            .Select(g => new TitleViewModel(g.Key, g.Count()))
            .OrderBy(t => t.Name);

        Titles.Clear();
        foreach (var title in titles) {
            Titles.Add(title);
        }
    }

    readonly IMangaRepository _repository;
}
