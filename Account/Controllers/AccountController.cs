using Account.Dtos;
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
        //private readonly EmailService _emailService; 

        public AccountController(UserManager<User> userManager)
        {
            _userManager = userManager;
            //_emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult>Register([FromBody] RegisterDto registerModel)
        {
            if (ModelState.IsValid) 
            {
                try
                {
                    var user = new User 
                    { 
                        UserName = registerModel.UserName,
                        Email = registerModel.Email
                    };

                    //TODO проверка по почте
                    //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmEmail = Url.Action("ConfirmEmail", "Account", new { });

                    var createdUser = await _userManager.CreateAsync(user,registerModel.Password);

                    
                    //await _emailService.SendEmailAsync();
                }
                catch (Exception ex)
                {

                }
            }
            return Ok();

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginModel)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == loginModel.Email.ToLower());
            if (user == null)
            {
                return Unauthorized("Invalid email");
            }
            var result = await _userManager.CheckPasswordAsync(user, loginModel.Password);
            if (result)
            {
                //TODO Token 
                return Ok("Успешно авторизирован");
            }
            else
            {
                return BadRequest("Не верный логин или пароль");
            }
        }
        /*[HttpGet]
        public async Task<IActionResult> ConfirmEmail()
        {
            
            return Ok();
        }  */
    }
}
