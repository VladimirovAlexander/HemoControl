namespace Account.Dtos
{
    public class UserInfoDto
    {
        public Guid UserId { get; set; }
        public string PolicyNumber { get; set; }
        public string Name { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }
    }
}
