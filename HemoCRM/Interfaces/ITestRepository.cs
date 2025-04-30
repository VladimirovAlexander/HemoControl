using HemoCRM.Web.Dtos.TestDtos;
using HemoCRM.Web.Models;

namespace HemoCRM.Web.Interfaces
{
    public interface ITestRepository
    {
        Task<List<Test>> GetAllTestsAsync();

        Task<Test?> GetTestByIdAsync(Guid id);

        Task<Test> CreateTestAsync(CreateTestDto testDto);

        Task<Test?> UpdateTestAsync(Guid id, UpdateTestDto updateDto);

        Task<bool> DeleteTestAsync(Guid id);
    }
}
