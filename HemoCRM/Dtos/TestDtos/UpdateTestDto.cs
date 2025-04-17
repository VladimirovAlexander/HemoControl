using HemoCRM.Web.Models;

namespace HemoCRM.Web.Dtos.TestDtos
{
    public class UpdateTestDto
    {
        public TestStatus Status { get; set; }
        public string? Result { get; set; }
    }
}
