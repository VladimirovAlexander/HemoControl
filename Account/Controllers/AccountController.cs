using Account.Dtos;
using Account.Interfaces;
using Account.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Account.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController:Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly  ITokenService _tokenService;
        //private readonly EmailService _emailService; 

        public AccountController(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            //_emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult>Register([FromBody] RegisterDto registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid input data");
            }
            try
            {
                var user = new User 
                { 
                    UserName = registerModel.UserName,
                    Email = registerModel.Email
                };

                    
                var confirmEmail = Url.Action("ConfirmEmail", "Account", new { });
                var createdUser = await _userManager.CreateAsync(user,registerModel.Password);

                //TODO проверка по почте
                if (createdUser.Succeeded)
                {
                    //await _emailService.SendEmailAsync();
                    return StatusCode(201, "Registration successful");
                }
                else
                {
                    return BadRequest(string.Join("; ", createdUser.Errors.Select(e => e.Description)));
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginModel)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == loginModel.Email.ToLower());
            if (user == null)
            {
                return NotFound($"Unable not user with email {loginModel.Email}");
            }
            var result = await _userManager.CheckPasswordAsync(user, loginModel.Password);
            if (result)
            {
                var token = _tokenService.GenerateToken(user);
                return Ok( new { token });
            }
            else
            {
                return NotFound("Не верный логин или пароль");
            }
        }
        
    }
}
