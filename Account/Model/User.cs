using Microsoft.AspNetCore.Identity;

namespace Account.Model
{
    public class User : IdentityUser
    {
        public string UserSurname { get; set; } = string.Empty;
        public string UserPatronymic { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Policy { get; set; } = string.Empty;
        public string Country {  get; set; } = string.Empty;
        public string Region {  get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street {  get; set; } = string.Empty;
        public int HouseNumber {  get; set; }
        public int AppartmentNumber { get; set; }
        public int DiagnosisCode {  get; set; }
        public string NameOfDiagnosis { get; set; } = string.Empty;
    }
}
