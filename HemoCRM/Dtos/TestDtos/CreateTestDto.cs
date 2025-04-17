using HemoCRM.Web.Models;

namespace HemoCRM.Web.Dtos.TestDtos
{
    public class CreateTestDto
    {
        public Guid PatientId { get; set; }
        public string TestName { get; set; }
        public TestStatus Status { get; set; } = TestStatus.Pending;
    }
}
