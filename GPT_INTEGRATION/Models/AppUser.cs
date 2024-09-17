using Microsoft.AspNetCore.Identity;

namespace GPT_INTEGRATION.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}