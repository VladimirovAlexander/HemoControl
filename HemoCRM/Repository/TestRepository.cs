using HemoCRM.Web.Data;
using HemoCRM.Web.Dtos.TestDtos;
using HemoCRM.Web.Interfaces;
using HemoCRM.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HemoCRM.Web.Repository
{
    public class TestRepository : ITestRepository
    {
        private readonly HemoCrmDbContext _dbContext;

        public TestRepository(HemoCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Test>> GetAllTestsAsync()
        {
            return await _dbContext.Tests
                .Include(t => t.Patient)
                .ToListAsync();
        }

        public async Task<Test?> GetTestByIdAsync(Guid id)
        {
            return await _dbContext.Tests
                .Include(t => t.Patient)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Test> CreateTestAsync(CreateTestDto testDto)
        {
            var patient = await _dbContext.Patients.FindAsync(testDto.PatientId);
            if (patient == null)
                throw new Exception("Пациент не найден");

            var test = new Test
            {
                Id = Guid.NewGuid(),
                PatientId = testDto.PatientId,
                TestName = testDto.TestName,
                Status = testDto.Status,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Tests.AddAsync(test);
            await _dbContext.SaveChangesAsync();

            return test;
        }

        public async Task<Test?> UpdateTestAsync(Guid id, UpdateTestDto updateDto)
        {
            var test = await _dbContext.Tests.FindAsync(id);

            if (test == null)
                return null;

            test.Status = updateDto.Status;

            if (!string.IsNullOrEmpty(updateDto.Result))
            {
                test.Result = updateDto.Result;
                test.CompletedAt = DateTime.UtcNow;
            }

            _dbContext.Tests.Update(test);
            await _dbContext.SaveChangesAsync();

            return test;
        }

        public async Task<bool> DeleteTestAsync(Guid id)
        {
            var test = await _dbContext.Tests.FindAsync(id);
            if (test == null) return false;

            _dbContext.Tests.Remove(test);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
