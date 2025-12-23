using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindThatBook.Infrastructure.Models
{
    internal record OpenLibraryWorkDto
    {
        public string? Title { get; init; }
        public string? Subtitle { get; init; }
        public string? FirstPublishDate { get; init; }
        public int[]? Covers { get; init; }
        public OpenLibraryWorkAuthorDto[]? Authors { get; init; }
        public OpenLibraryWorkContributorDto[]? Contributors { get; init; }
    }

    internal record OpenLibraryWorkAuthorDto
    {
        public OpenLibraryAuthorRef? Author { get; init; }
    }

    internal record OpenLibraryAuthorRef
    {
        public string? Key { get; init; }
    }

    internal record OpenLibraryWorkContributorDto
    {
        public string? Name { get; init; }
        public string? Role { get; init; }
    }

    internal record OpenLibraryAuthorDto
    {
        [JsonProperty("name")]
        public string? name { get; init; }
    }
}
