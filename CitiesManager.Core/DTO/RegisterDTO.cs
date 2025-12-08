using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Person name cant be blank")]
        public string? PersonName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email cant be blank")]
        [EmailAddress(ErrorMessage = "Email should be in a proper email address format")]
        [Remote(action: "IsEmailAlreadyRegister", controller: "Account", ErrorMessage = "Email is already used")]
        public string? Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number cant be blank")]
        [Phone]
        public string? PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password cant be blank")]
        public string? Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password cant be blank")]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match")]
        public string? ConfirmPassword { get; set; } = string.Empty;
    }
}
