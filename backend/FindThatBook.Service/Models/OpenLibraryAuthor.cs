namespace FindThatBook.Service.Models
{
    public record OpenLibraryAuthor
    {
        public string Id { get; init; } = "";
        public string Name { get; init; } = "";
        public string? Bio { get; init; }
    }
}

