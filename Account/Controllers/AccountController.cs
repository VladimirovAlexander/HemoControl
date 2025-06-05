using Account.Constants;
using Account.Dtos;
using Account.Interfaces;
using Account.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Security.Claims;

namespace Account.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController:Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly  ITokenService _tokenService;
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(UserManager<User> userManager, ITokenService tokenService, IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _httpClientFactory = httpClientFactory;
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

        [HttpPost("register-staff")]
        public async Task<IActionResult> RegisterStaff([FromBody] RegisterStaffDto registerModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input data");

            try
            {
                var validRoles = new List<string> { UserRole.Doctor, UserRole.Assistant, UserRole.Administrator, UserRole.Administrator };
                if (!validRoles.Contains(registerModel.Role))
                    return BadRequest("Неверная роль");

                var user = new User
                {
                    UserName = registerModel.Login,
                    Email = registerModel.Email
                };

                var createUserResult = await _userManager.CreateAsync(user, registerModel.Password);
                if (!createUserResult.Succeeded)
                    return BadRequest(string.Join("; ", createUserResult.Errors.Select(e => e.Description)));

                await _userManager.AddToRoleAsync(user, registerModel.Role);

                if (registerModel.Role == UserRole.Doctor)
                {
                    var httpClient = _httpClientFactory.CreateClient();

                    var createDoctorDto = new
                    {
                        FirstName = registerModel.FirstName,
                        LastName = registerModel.LastName,
                        Surname = registerModel.Surname,
                        Specialty = registerModel.Specialty,
                        Email = registerModel.Email,
                        PhoneNumber = registerModel.PhoneNumber,
                        UserId = user.Id
                    };

                    var response = await httpClient.PostAsJsonAsync("https://localhost:7098/api/doctor/CreateDoctor", createDoctorDto);

                    if (!response.IsSuccessStatusCode)
                    {
                        await _userManager.DeleteAsync(user);
                        return StatusCode(500, "Не удалось создать профиль доктора");
                    }
                }

                return StatusCode(201, $"{registerModel.Role} успешно зарегистрирован");
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
                return NotFound("Неверный логин или пароль");
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("Invalid user ID in token");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("Пользователь не найден");

            var roles = await _userManager.GetRolesAsync(user);

            var dto = new UserInfoDto
            {
                UserId = Guid.Parse(user.Id),
                PolicyNumber = user.PolicyNumber,
                Name = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Role = roles.FirstOrDefault(),

            };
            return Ok(dto);
        }
    }
}
