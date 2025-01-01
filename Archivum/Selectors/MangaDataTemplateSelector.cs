using Archivum.ViewModels;

namespace Archivum.Selectors;

class MangaDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? DirectoryItemTemplate { get; set; }
    public DataTemplate? FileItemTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container) {
        var mangaViewModel = (MangaViewModel)item;
        return mangaViewModel.IsDirectory ? DirectoryItemTemplate : FileItemTemplate;
    }
}
