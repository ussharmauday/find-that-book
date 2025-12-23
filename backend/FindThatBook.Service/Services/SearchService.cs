using FindThatBook.Service.Interface;
using FindThatBook.Service.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FindThatBook.Service
{
    public class SearchService : ISearchService
    {
        private readonly IOpenLibraryClient _openLibraryClient;
        private readonly ILLMClient _llmClient;
        private readonly ILogger<SearchService> _logger;

        public SearchService(IOpenLibraryClient openLibraryClient, ILLMClient llmClient, ILogger<SearchService> logger)
        {
            _openLibraryClient = openLibraryClient;
            _llmClient = llmClient;
            _logger = logger;
        }

        public async Task<SearchResult[]> SearchAsync(string query, CancellationToken cancellationToken)
        {
            ExtractedQuery extracted;

            try
            {
                // 1. Extract structured query using Gemini
                var response = await _llmClient.GenerateAsync(query, cancellationToken);
                response = response
                    .Replace("“", "\"")
                    .Replace("”", "\"")
                    .Replace("\u200b", "");

                extracted = JsonSerializer.Deserialize<ExtractedQuery>(
                    response,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new ExtractedQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extract structured query from input: {Query}", query);
                return Array.Empty<SearchResult>();
            }

            OpenLibraryWork[] candidates;
            try
            {
                // 2. Search Open Library using extracted fragments
                candidates = await _openLibraryClient.SearchWorksAsync(
                    extracted.Title,
                    extracted.Author,
                    extracted.Keywords.ToArray(),
                    cancellationToken
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search Open Library with extracted query: {@Extracted}", extracted);
                return Array.Empty<SearchResult>();
            }

            if (candidates.Length == 0)
                return Array.Empty<SearchResult>();

            // 3. Rank candidates
            var ranked = candidates
                .Select(w => new
                {
                    Work = w,
                    Score = ComputeScore(w, extracted)
                })
                .OrderByDescending(x => x.Score)
                .Take(5)
                .ToArray();

            // 4. Prepare candidate info for batch explanation
            var candidatesText = string.Join("\n", ranked.Select(r =>
                $@"Title: {r.Work.Title}
Primary authors: {string.Join(", ", r.Work.Authors)}
Contributors: {string.Join(", ", r.Work.Contributors)}
First publish year: {r.Work.FirstPublishYear?.ToString() ?? "null"}"
            ));

            ExplanationResult[] explanations;
            try
            {
                // 5. Call Gemini batch explanation
                var explanationJson = await _llmClient.ExplainAsyncBatch(query, candidatesText, cancellationToken);

                // 6. Deserialize explanations
                explanations = JsonSerializer.Deserialize<ExplanationResult[]>(
                    explanationJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? Array.Empty<ExplanationResult>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to generate explanations for candidates. Returning empty explanations.");
                explanations = Array.Empty<ExplanationResult>();
            }

            // 7. Build final SearchResult list
            var results = new List<SearchResult>();
            for (int i = 0; i < ranked.Length; i++)
            {
                var work = ranked[i].Work;
                var explanation = explanations.ElementAtOrDefault(i)?.Explanation ?? string.Empty;

                results.Add(new SearchResult
                {
                    Title = work.Title,
                    Authors = work.Authors,
                    Contributors = work.Contributors,
                    FirstPublishYear = work.FirstPublishYear,
                    CoverImageUrl = work.CoverImageUrl,
                    Explanation = explanation
                });
            }

            return results.ToArray();
        }

        private int ComputeScore(OpenLibraryWork work, ExtractedQuery extracted)
        {
            int score = 0;

            bool titleMatch = !string.IsNullOrEmpty(extracted.Title) &&
                              work.Title.Contains(extracted.Title, StringComparison.OrdinalIgnoreCase);

            bool authorMatch = !string.IsNullOrEmpty(extracted.Author) &&
                               work.Authors.Any(a => a.Contains(extracted.Author, StringComparison.OrdinalIgnoreCase));

            bool contributorOnly = !authorMatch &&
                                   !string.IsNullOrEmpty(extracted.Author) &&
                                   work.Contributors.Any(c => c.Contains(extracted.Author, StringComparison.OrdinalIgnoreCase));

            if (titleMatch && authorMatch) score += 100;
            else if (titleMatch && contributorOnly) score += 80;
            else if (!string.IsNullOrEmpty(extracted.Title) && titleMatch) score += 50;
            else if (authorMatch) score += 30;

            return score;
        }

        private class ExplanationResult
        {
            public string Title { get; set; } = string.Empty;
            public string Explanation { get; set; } = string.Empty;
        }
    }
}
