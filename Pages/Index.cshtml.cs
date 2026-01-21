using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PortfolioMichael.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public string Email { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;
        public string LinkedIn { get; private set; } = string.Empty;
        public string Portfolio { get; private set; } = string.Empty;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            Email = _configuration["Contact:Email"] ?? string.Empty;
            Phone = _configuration["Contact:Phone"] ?? string.Empty;
            LinkedIn = _configuration["Social:LinkedIn"] ?? string.Empty;
            Portfolio = _configuration["Social:Portfolio"] ?? string.Empty;
        }
    }
}