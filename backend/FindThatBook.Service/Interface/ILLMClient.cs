using FindThatBook.Service.Models;

namespace FindThatBook.Service.Interface
{
    public interface ILLMClient
    {
        Task<string> GenerateAsync(string prompt, CancellationToken ct);
        Task<string> ExplainAsyncBatch(string work, string query, CancellationToken ct);
    }
}
