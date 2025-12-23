using System.Text.Json.Serialization;

namespace FindThatBook.Service.Models
{
    //public sealed record SearchResult(
    //    string Query,
    //    IReadOnlyList<BookCandidate> Results
    //);

    //public sealed record BookCandidate(
    //    string Title,
    //    IReadOnlyList<string> PrimaryAuthors,
    //    int? FirstPublishYear,
    //    OpenLibraryMetadata OpenLibrary,
    //    string Explanation
    //);

    //public sealed record OpenLibraryMetadata(
    //    string WorkId,
    //    string WorkUrl,
    //    string? CoverImageUrl
    //);
    // File: LibraryDiscovery.Application/Models/SearchResult.cs
    public record SearchResult
    {
        public string Title { get; init; } = "";
        public string[] Authors { get; init; } = Array.Empty<string>();
        public string[] Contributors { get; init; } = Array.Empty<string>();
        [JsonPropertyName("first_publish_year")]
        public int? FirstPublishYear { get; init; }
        public string? CoverImageUrl { get; init; }
        public string Explanation { get; init; } = "";
    }

}
