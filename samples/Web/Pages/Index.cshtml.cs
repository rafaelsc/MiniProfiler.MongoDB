using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Models;
using Web.Services;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly MflixService _mflixService;

        public IList<Movie> Movies = new List<Movie>();

        public IndexModel(ILogger<IndexModel> logger, MflixService mflixService)
        {
            _logger = logger;
            _mflixService = mflixService;
        }

        public void OnGet()
        {
            this.Movies = this._mflixService.ListAll().ToList();
        }
    }
}