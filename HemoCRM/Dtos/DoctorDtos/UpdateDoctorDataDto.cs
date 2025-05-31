namespace HemoCRM.Web.Dtos.DoctorDtos
{
    public class UpdateDoctorDataDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Surname { get; set; }
        public string Specialty { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
