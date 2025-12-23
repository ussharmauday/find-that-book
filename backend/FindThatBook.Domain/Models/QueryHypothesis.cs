using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindThatBook.Domain.Models
{
    public record QueryHypothesis(
    string? Title,
    string? Author,
    IReadOnlyList<string> Keywords,
    string ConfidenceNote
);

    public record HypothesisExtractionResult(
        IReadOnlyList<QueryHypothesis> Hypotheses
    );


}
