namespace FindThatBook.Service.Models
{
    public class OpenLibraryWork
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string[] Authors { get; set; } = Array.Empty<string>();
        public string[] Contributors { get; set; } = Array.Empty<string>();
        public int? FirstPublishYear { get; set; }
        public string? CoverImageUrl { get; set; }
    }
}

