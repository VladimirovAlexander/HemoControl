using System.ComponentModel.DataAnnotations;

namespace Account.Dtos
{
    public class RegisterStaffDto
    {
        [Required]
        public string Login { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Specialty {  get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
