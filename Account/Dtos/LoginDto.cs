using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Account.Dtos
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}
