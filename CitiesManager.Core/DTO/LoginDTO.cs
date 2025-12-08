using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.DTO
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress(ErrorMessage = "Email should be in a proper email address format")]
        public string? Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password cant be blank")]
        public string? Password { get; set; } = string.Empty;
    }
}
