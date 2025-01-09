using System.Collections.Generic;
using System.Threading.Tasks;

namespace Archivum.Contracts.Repositories;

public interface IMangaRepository
{
    Task<IEnumerable<Models.Manga>> GetMangasAsync();
    Task<IEnumerable<Models.Author>> GetAuthorsAsync();
    Task<IEnumerable<Models.Title>> GetTitlesAsync();
}
