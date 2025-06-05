using HemoCRM.Web.Dtos.TestDtos;
using HemoCRM.Web.Models;

namespace HemoCRM.Web.Interfaces
{
    public interface ITestRepository
    {
        Task<List<Test>> GetAllTestsAsync();

        Task<Test?> GetTestByIdAsync(Guid id);

        Task<Test> CreateTestAsync(CreateTestDto testDto);

        Task UpdateTestAsync(Test test);

        Task<bool> DeleteTestAsync(Guid id);
        Task<Test?> GetTestByReceptionIdAndTypeAsync(Guid receptionId, string testType);
        Task DeleteAsync(Test test);
        Task<List<Test>> EnrichTestResults(List<Test> tests);
        Task<List<Test>?> GetTestsByReceptionIdAsync(Guid receptionId);
        Task<List<Reception>> GetReceptionsByPatientIdAsync(Guid patientId);
    }
}
