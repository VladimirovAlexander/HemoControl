namespace HemoCRM.Web.Dtos.AccountDtos
{
    public class UserInfoDto
    {
        public Guid UserId { get; set; }
        public string PolicyNumber { get; set; }
        public string Name { get; set; }
        public string? Surname { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
