using Account.Constants;
using Account.Dtos;
using Account.Interfaces;
using Account.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Account.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController:Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly  ITokenService _tokenService; 

        public AccountController(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;           
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
                    Email = registerModel.Email,
                    PolicyNumber = registerModel.PolicyNumber
                };

                var createdUser = await _userManager.CreateAsync(user,registerModel.Password);

                //TODO проверка по почте
                if (createdUser.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, UserRole.User);
                    return Ok(new UserResponseDto
                    {
                        UserId = Guid.Parse(user.Id),
                        Email = user.Email
                    });
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

        [Authorize(Roles = UserRole.Administrator)]
        [HttpPost("register-staff")]
        public async Task<IActionResult> RegisterStaff([FromBody] RegisterStaffDto registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid input data");
            }

            try
            {
                var validRoles = new List<string> { UserRole.Doctor, UserRole.Assistant, UserRole.Administrator };
                if (!validRoles.Contains(registerModel.Role))
                {
                    return BadRequest("Неверная роль");
                }

                var user = new User
                {
                    UserName = registerModel.UserName,
                    Email = registerModel.Email
                };

                var createdUser = await _userManager.CreateAsync(user, registerModel.Password);

                if (createdUser.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, registerModel.Role);
                    return StatusCode(201, $"{registerModel.Role} успешно зарегистрирован");
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

        [Authorize]
        [HttpGet("info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("Пользователь не найден");

            var dto = new UserInfoDto
            {
                UserId = Guid.Parse(user.Id),
                PolicyNumber = user.PolicyNumber,
                Name = user.UserName
            };

            return Ok(dto);
        }
    }
}
