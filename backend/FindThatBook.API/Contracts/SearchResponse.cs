namespace FindThatBook.API.Contracts
{
    public sealed record SearchResponse(
    string Query,
    IReadOnlyList<BookCandidate> Results
);

    public sealed record BookCandidate(
        string Title,
        IReadOnlyList<string> PrimaryAuthors,
        int? FirstPublishYear,
        OpenLibraryMetadata OpenLibrary,
        string Explanation
    );

    public sealed record OpenLibraryMetadata(
        //string WorkId,
        string WorkUrl,
        string? CoverImageUrl
    );

}
