using FindThatBook.Service;
using FindThatBook.Service.Interface;
using FindThatBook.Service.Models;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FindThatBook.Infrastructure.Service
{
    internal class GeminiClient : ILLMClient
    {
        private readonly Client _client;
        private readonly ILogger<GeminiClient> _logger;

        public GeminiClient(IConfiguration configuration, ILogger<GeminiClient> logger)
        {
            _logger = logger;

            var apiKey = configuration["Gemini:ApiKey"]
                ?? throw new InvalidOperationException("Gemini API key missing");

            _client = new Client(null, apiKey);
        }

        public async Task<string> GenerateAsync(string prompt, CancellationToken ct)
        {
            string systemPrompt = @"
You are a STRICT, NON-GENERATIVE text parser.

Your task is to COPY information from the user query.
You may extract partial words or fragments if they appear in the query.
DO NOT complete, infer, normalize, or invent anything.

Return JSON strictly matching this schema:
{
  ""title"": string | null,
  ""author"": string | null,
  ""keywords"": string[]
}

Rules:
- Extract only substrings that appear exactly in the query
- If a value is not explicitly present, return null
- keywords are leftover tokens literally present in the query
- If no leftover tokens exist, keywords must be an empty array
- Return valid JSON ONLY, no markdown, no explanations
";

            try
            {
                var contents = new List<Content>
                                {
                                    new Content
                                    {
                                        Role = "user",
                                        Parts = new List<Part>
                                        {
                                            new Part { Text = prompt }
                                        }
                                    }
                                };

                var systemConfig = new Content
                                    {
                                        Role = "system",
                                        Parts = new List<Part>
                                        {
                                            new Part { Text = systemPrompt }
                                        }
                                    };


                var response = await _client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash",
                    contents: contents,
                    config: new GenerateContentConfig { SystemInstruction = systemConfig }
                );

                if (response?.Candidates == null || response.Candidates.Count == 0)
                {
                    _logger.LogError("Gemini GenerateAsync returned no candidates for prompt: {Prompt}", prompt);
                    throw new InvalidOperationException("Gemini returned no candidates");
                }

                return response.Candidates[0].Content.Parts[0].Text;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating structured query from prompt: {Prompt}", prompt);
                throw new InvalidOperationException("Failed to generate structured query", ex);
            }
        }

        public async Task<string> ExplainAsyncBatch(string userQuery, string candidatesText, CancellationToken cancellationToken)
        {
            string systemPrompt = @"
You are explaining why each candidate book matches a user query.

Rules:
- Explain only using the information provided: title, primary authors, contributors, first publish year.
- Do NOT infer, normalize, or add facts not present in the input.
- Each explanation must be EXACTLY ONE sentence.
- Mention primary authors first; contributors only if relevant.
- Include publication year only if provided.
- Return a JSON array of objects, strictly matching this schema:
[
  { ""title"": string, ""explanation"": string }
]
- The array must contain exactly one item per candidate, in the same order as provided.
- Do NOT include markdown, extra text, or commentary.
";

            string userPrompt = $@"
User query: ""{userQuery}""

Candidate books:
{candidatesText}

Return JSON array of objects with:
- title: candidate's title
- explanation: one-sentence explanation of why it matches the user query
";

            try
            {
                var contents = new List<Content>
                {
                    new Content { Role = "model", Parts = new List<Part>{ new Part { Text = systemPrompt } } },
                    new Content { Role = "user", Parts = new List<Part>{ new Part { Text = userPrompt } } }
                };

                var response = await _client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash",
                    contents: contents
                );

                if (response?.Candidates == null || response.Candidates.Count == 0)
                {
                    _logger.LogError("Gemini ExplainAsyncBatch returned no candidates for query: {Query}", userQuery);
                    throw new InvalidOperationException("Gemini returned no candidates");
                }

                return response.Candidates[0].Content.Parts[0].Text;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating explanations for query: {Query}", userQuery);
                throw new InvalidOperationException("Failed to generate explanations", ex);
            }
        }
    }
}
