using FindThatBook.Service.Models;

namespace FindThatBook.Service.Interface
{
    public interface IOpenLibraryClient
    {
        Task<OpenLibraryWork[]> SearchWorksAsync(
        string? title,
        string? author,
        string[] keywords,
        CancellationToken cancellationToken);

        Task<OpenLibraryWork?> GetWorkAsync(string workId, CancellationToken cancellationToken);

        Task<OpenLibraryAuthor?> GetAuthorAsync(string authorId, CancellationToken cancellationToken);
    }
}



