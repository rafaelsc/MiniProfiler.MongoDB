using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Models;
using Web.Services;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly MflixService _mflixService;

        public IEnumerable<Movie> Movies;
        public IAsyncEnumerable<Movie> MoviesAsync;

        public IndexModel(ILogger<IndexModel> logger, MflixService mflixService)
        {
            _logger = logger;
            _mflixService = mflixService;
        }

        public void OnGet()
        {
            this.Movies = this._mflixService.ListAll();
            this.MoviesAsync = this._mflixService.ListAllAsync();
        }
    }
}