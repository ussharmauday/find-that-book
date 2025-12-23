using FindThatBook.Service.Models;

namespace FindThatBook.Service.Interface
{
    public interface ISearchService
    {
        Task<SearchResult[]> SearchAsync(string query, CancellationToken cancellationToken);
    }
}
