using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FindThatBook.Infrastructure.Models
{
    internal record OpenLibrarySearchResponse
    {
        public OpenLibraryDoc[]? Docs { get; init; }
    }
    internal record OpenLibraryDoc
    {
        public string? Key { get; init; }
        [JsonPropertyName("first_publish_year")]
        public int? FirstPublishedYear { get; set; }
    }
}
