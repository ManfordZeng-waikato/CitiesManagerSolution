using Asp.Versioning;
using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CitiesManager.WebAPI.Controllers.v1
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    public class AccountController : CustomControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApplicationUser>> PostRegister(RegisterDTO registerDTO)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessages =
                string.Join("|", ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMessages);
            }

            ApplicationUser user = new()
            {
                PersonName = registerDTO.PersonName,
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber
            };
            IdentityResult result =
            await _userManager.CreateAsync(user, registerDTO.Password);

            if (result.Succeeded)
            {
                // Assign Role
                if (await _roleManager.RoleExistsAsync("User"))
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok(user);
            }
            else
            {
                string errorMessages =
                string.Join("|", result.Errors.Select(e => e.Description));
                return Problem(errorMessages);
            }
        }

        [HttpGet("IsEmailAlreadyRegister")]
        public async Task<IActionResult> IsEmailAlreadyRegister(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> PostLogin(LoginDTO loginDTO)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessages =
                string.Join("|", ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMessages);
            }
            var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(loginDTO.Email);
                return Ok(new { personName = user.PersonName, email = user.Email });
            }
            else
            {
                return Problem("Invalid login attempt");
            }
        }

        [HttpGet("logout")]
        public async Task<ActionResult> GetLogout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
    }
}
