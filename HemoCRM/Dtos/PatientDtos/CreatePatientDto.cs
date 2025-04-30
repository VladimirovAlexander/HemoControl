namespace HemoCRM.Web.Dtos.PatirntDtos
{
    public class CreatePatientDto
    {
        public string Name { get; set; }
        public string? Surname { get; set; }
        public string? Patronymic { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Policy { get; set; } = string.Empty;
        public string? Passport { get; set; } = string.Empty;
        public string? Country { get; set; } = string.Empty;
        public string? Region { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? Street { get; set; } = string.Empty;
        public int? HouseNumber { get; set; }
        public int? AppartmentNumber { get; set; }

        public Guid? UserId { get; set; }
    }
}
