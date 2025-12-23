using FindThatBook.Api.Contracts;
using FluentValidation;

public sealed class SearchRequestValidator : AbstractValidator<SearchRequest>
{
    public SearchRequestValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty()
            .WithMessage("Query is required.")
            .MinimumLength(2)
            .WithMessage("Query must contain at least 2 characters.")
            .MaximumLength(300)
            .WithMessage("Query must not exceed 300 characters.");
    }
}
