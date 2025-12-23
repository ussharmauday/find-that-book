namespace FindThatBook.Service
{
    public class ExtractedQuery
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public List<string> Keywords { get; set; } = new();
    }
}
