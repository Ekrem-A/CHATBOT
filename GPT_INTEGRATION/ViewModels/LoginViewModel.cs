using System.ComponentModel.DataAnnotations;

namespace GPT_INTEGRATION.ViewModels
{
    public class LoginViewModel
    {
        [EmailAddress]
        public string Email { get; set; } = null!;
        
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; } = true;
    }
}