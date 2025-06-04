namespace HemoCRM.Web.Dtos.TestDtos
{
    public class AssignTestsDto
    {
        public Guid PatientId { get; set; }
        public List<TestCreationDto> Tests { get; set; } = new();
    }

    public class TestCreationDto
    {
        public string TestType { get; set; } = string.Empty; // CBC, Coagulogram, FactorAndVWF
        public string TestName { get; set; } = string.Empty;
    }
}
