using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortfolioMichael.Services;
using System.ComponentModel.DataAnnotations;

namespace PortfolioMichael.Pages
{
    public class ContactModel : PageModel
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ContactModel> _logger;

        public ContactModel(IEmailService emailService, IConfiguration configuration, ILogger<ContactModel> logger)
        {
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public ContactFormModel ContactForm { get; set; } = new();

        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string LinkedIn { get; set; } = string.Empty;
        public string Portfolio { get; set; } = string.Empty;

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public void OnGet() => LoadContactInfo();

        public async Task<IActionResult> OnPostAsync()
        {
            LoadContactInfo();

            if (!ModelState.IsValid)
            {
                return Page(); // Returns with validation errors visible
            }

            try
            {
                var result = await _emailService.SendContactFormAsync(
                    ContactForm.Name!,
                    ContactForm.Email!,
                    ContactForm.Subject!,
                    ContactForm.Message!
                );

                if (result)
                {
                    SuccessMessage = "Thank you! Your message has been sent successfully.";
                    return RedirectToPage(); // PRG Pattern (Post-Redirect-Get) to prevent double-submit
                }

                ErrorMessage = "Could not send email. Please try again later.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending contact form email.");
                ErrorMessage = "An unexpected error occurred. Please contact me directly at skillopp05@gmail.com";
            }

            return Page();
        }

        private void LoadContactInfo()
        {
            Email = _configuration["Contact:Email"] ?? "skillopp05@gmail.com";
            Phone = _configuration["Contact:Phone"] ?? "+27 69 262 9422";
            LinkedIn = _configuration["Social:LinkedIn"] ?? "https://linkedin.com";
            Portfolio = _configuration["Social:Portfolio"] ?? "https://macfish.co.za";
        }
    }

    public class ContactFormModel
    {
        [Required(ErrorMessage = "Please provide your name.")]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "A valid email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter a subject.")]
        [StringLength(200)]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Message cannot be empty.")]
        [StringLength(5000, ErrorMessage = "Message is too long.")]
        public string? Message { get; set; }
    }
}
