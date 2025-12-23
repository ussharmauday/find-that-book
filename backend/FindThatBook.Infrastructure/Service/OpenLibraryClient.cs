using FindThatBook.Infrastructure.Models;
using FindThatBook.Service.Interface;
using FindThatBook.Service.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace FindThatBook.Infrastructure.Service
{
    public class OpenLibraryClient : IOpenLibraryClient
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<OpenLibraryClient> _logger;

        public OpenLibraryClient(HttpClient httpClient, ILogger<OpenLibraryClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<OpenLibraryWork[]> SearchWorksAsync(
            string? title,
            string? author,
            string[] keywords,
            CancellationToken cancellationToken)
        {
            var queryParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(title))
                queryParts.Add(title);

            if (!string.IsNullOrWhiteSpace(author))
                queryParts.Add(author);

            if (keywords?.Length > 0)
                queryParts.AddRange(keywords);

            if (queryParts.Count == 0)
                return Array.Empty<OpenLibraryWork>();

            var q = Uri.EscapeDataString(string.Join(" ", queryParts));
            var url = $"https://openlibrary.org/search.json?q={q}&limit=10";
            try
            {

                var response = await _httpClient.GetFromJsonAsync<OpenLibrarySearchResponse>(
                    url,
                    cancellationToken);

                if (response?.Docs == null || response.Docs.Length == 0)
                    return [];

                var workIds = response.Docs
                    .Select(d => d.Key)
                    .Where(k => !string.IsNullOrWhiteSpace(k) && k!.StartsWith("/works/"))
                    .Select(k => k!.Replace("/works/", ""))
                    .Distinct()
                    .Take(5)
                    .ToArray();

                var works = new List<OpenLibraryWork>();

                foreach (var workId in workIds)
                {
                    var work = await GetWorkAsync(workId, cancellationToken);

                    if (work != null && work.Authors?.Length > 0)
                    {
                        var authorNames = new List<string>();

                        foreach (var authorId in work.Authors)
                        {
                            var authorDetails = await GetAuthorAsync(authorId, cancellationToken);
                            if (authorDetails != null && !string.IsNullOrWhiteSpace(authorDetails.Name))
                                authorNames.Add(authorDetails.Name);
                        }

                        work.Authors = authorNames.ToArray();
                        work.FirstPublishYear = response.Docs.FirstOrDefault(x => x.Key.Replace("/works/", "") == work.Id).FirstPublishedYear;
                    }

                    if (work != null)
                        works.Add(work);
                }
                return works.ToArray();
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }

            
        

        public async Task<OpenLibraryWork?> GetWorkAsync(
            string workId,
            CancellationToken cancellationToken)
        {
            var url = $"https://openlibrary.org/works/{workId}.json";

            OpenLibraryWorkDto? dto;

            try
            {
                dto = await _httpClient.GetFromJsonAsync<OpenLibraryWorkDto>(
                    url,
                    cancellationToken);
            }
            catch
            {
                return null;
            }

            if (dto == null)
                return null;

            return new OpenLibraryWork
            {
                Id = workId,
                Title = dto.Title ?? string.Empty,
                Authors = dto.Authors?
                    .Select(a => a.Author?.Key)
                    .Where(k => !string.IsNullOrWhiteSpace(k))
                    .Select(k => k!.Replace("/authors/", ""))
                    .ToArray()
                    ?? Array.Empty<string>(),
                Contributors = dto.Contributors?
                    .Select(c => c.Name)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .ToArray()
                    ?? Array.Empty<string>(),
                FirstPublishYear = ParseYear(dto.FirstPublishDate),
                CoverImageUrl = dto.Covers?.FirstOrDefault() is int coverId
                    ? $"https://covers.openlibrary.org/b/id/{coverId}-M.jpg"
                    : null
            };
        }

        public async Task<OpenLibraryAuthor?> GetAuthorAsync(
            string authorId,
            CancellationToken cancellationToken)
        {
            var url = $"https://openlibrary.org/authors/{authorId}.json";

            OpenLibraryAuthorDto? dto;

            try
            {
                dto = await _httpClient.GetFromJsonAsync<OpenLibraryAuthorDto>(
                    url,
                    cancellationToken);
            }
            catch
            {
                return null;
            }

            if (dto == null)
                return null;

            return new OpenLibraryAuthor
            {
                Id = authorId,
                Name = dto.name ?? string.Empty,
                //Bio = dto.Bio
            };
        }

        private static int? ParseYear(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (value.Length >= 4 &&
                int.TryParse(value.AsSpan(0, 4), out var year))
                return year;

            return null;
        }            

        
    }
}
