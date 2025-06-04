using HemoCRM.Web.Models;

namespace HemoCRM.Web.Dtos.TestDtos
{
    public class CreateTestDto
    {
        public Guid PatientId { get; set; }
        public Guid ReceptionId { get; set; }
        public string TestType { get; set; } = string.Empty; // cbc, coagulogram, factorandvwf
        public string TestName { get; set; } = string.Empty;
        public TestStatus Status { get; set; } = TestStatus.Pending;
        public Dictionary<string, double> Parameters { get; set; } = new();
    }
}
