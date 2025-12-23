using FindThatBook.Api.Contracts;
using FindThatBook.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FindThatBook.API.Controllers
{
    [ApiController]
    [Route("/api/book")]
    public class BookController : ControllerBase
    {
       private readonly ILogger<BookController> _logger;
        private readonly ISearchService _searchService;

        public BookController(ILogger<BookController> logger, ISearchService searchService)
        {
            _logger = logger;
            _searchService = searchService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var result = await _searchService.SearchAsync(
                request.Query,
                cancellationToken);

            return Ok(result);
        }
    }
}
