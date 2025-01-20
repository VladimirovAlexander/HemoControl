using Microsoft.AspNetCore.Mvc;

namespace Account.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController:Controller
    {
        [HttpPost("register")]

        public async Task<IActionResult> Register()
        {

        }
    }
}
