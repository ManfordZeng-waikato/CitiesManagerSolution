using Asp.Versioning;
using CitiesManager.Core.DTO;
using CitiesManager.Core.Enums;
using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CitiesManager.WebAPI.Controllers.v1
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    public class AccountController : CustomControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
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
                PhoneNumber = registerDTO.PhoneNumber,
            };
            IdentityResult result =
            await _userManager.CreateAsync(user, registerDTO.Password);

            if (result.Succeeded)
            {
                // Assign Role
                if (registerDTO.UserType == UserTypeOptions.Admin)
                {
                    if (await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new()
                        {
                            Name = UserTypeOptions.Admin.ToString()
                        };
                        await _roleManager.CreateAsync(applicationRole);

                        await _userManager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());
                    }
                }
                else
                {
                    if (await _roleManager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new()
                        {
                            Name = UserTypeOptions.User.ToString()
                        };
                        await _roleManager.CreateAsync(applicationRole);

                        await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());
                    }
                }
                await _signInManager.SignInAsync(user, isPersistent: false);
                AuthenticationResponse authenticationResponse = _jwtService.CreateJwtToken(user);

                user.RefreshToken = authenticationResponse.RefreshToken;
                user.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpiration;
                await _userManager.UpdateAsync(user);

                return Ok(authenticationResponse);
            }
            else
            {
                string errorMessages = string.Join("|", result.Errors.Select(e => e.Description));
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
                await _signInManager.SignInAsync(user, isPersistent: false);
                AuthenticationResponse authenticationResponse = _jwtService.CreateJwtToken(user);

                user.RefreshToken = authenticationResponse.RefreshToken;
                user.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpiration;
                await _userManager.UpdateAsync(user);
                return Ok(authenticationResponse);
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

        [HttpPost("generate-new-jwt-token")]
        public async Task<ActionResult> PostGenerateNewJwtToken(TokenModel tokenModel)
        {
            if (tokenModel == null)
            {
                return Problem("Invalid client request");
            }
            ClaimsPrincipal? principal = _jwtService.GetPrincinpalFromJwtToken(tokenModel.Token);
            if (principal == null)
            {
                return Problem("Invalid JWT token");
            }

            string? email = principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser? applicationUser = await _userManager.FindByEmailAsync(email);
            if (applicationUser == null || applicationUser.RefreshToken != tokenModel.RefreshToken || applicationUser.RefreshTokenExpirationDateTime <= DateTime.Now)
            {
                return Problem("Invalid refresh token");
            }
            var authenticationResponse = _jwtService.CreateJwtToken(applicationUser);
            applicationUser.RefreshToken = authenticationResponse.RefreshToken;
            applicationUser.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpiration;
            await _userManager.UpdateAsync(applicationUser);
            return Ok(authenticationResponse);
        }
    }
}
