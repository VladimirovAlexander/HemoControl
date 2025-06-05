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
                .Include(t => t.Reception)
                .Include(t => t.CbcDetails)
                .Include(t => t.CoagulogramDetails)
                .Include(t => t.FactorAndVwfDetails)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Test?> GetTestByIdAsync(Guid id)
        {
            return await _dbContext.Tests
                .Include(t => t.Patient)
                .Include(t => t.Reception)
                .Include(t => t.CbcDetails)
                .Include(t => t.CoagulogramDetails)
                .Include(t => t.FactorAndVwfDetails)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Test> CreateTestAsync(CreateTestDto testDto)
        {
            var patient = await _dbContext.Patients.FindAsync(testDto.PatientId);
            if (patient == null)
                throw new Exception("Пациент не найден");

            var testId = Guid.NewGuid();

            var test = new Test
            {
                Id = testId,
                PatientId = testDto.PatientId,
                ReceptionId = testDto.ReceptionId,
                TestName = testDto.TestName,
                TestType = testDto.TestType,
                Status = testDto.Status,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Tests.AddAsync(test);

            switch (testDto.TestType.ToLower())
            {
                case "cbc":
                    var cbc = new CompleteBloodCountTest
                    {
                        TestId = testId,
                        Hemoglobin = testDto.Parameters.GetValueOrDefault("Hemoglobin", 0),
                        Hematocrit = testDto.Parameters.GetValueOrDefault("Hematocrit", 0),
                        WhiteBloodCells = testDto.Parameters.GetValueOrDefault("WBC", 0),
                        RedBloodCells = testDto.Parameters.GetValueOrDefault("RBC", 0),
                        Platelets = testDto.Parameters.GetValueOrDefault("Platelets", 0),
                        MCH = testDto.Parameters.GetValueOrDefault("MCH", 0),
                        MCV = testDto.Parameters.GetValueOrDefault("MCV", 0)
                    };
                    await _dbContext.CompleteBloodCountTests.AddAsync(cbc);
                    break;

                case "coagulogram":
                    var coagulogram = new CoagulogramTest
                    {
                        TestId = testId,
                        PT = testDto.Parameters.GetValueOrDefault("PT", 0),
                        INR = testDto.Parameters.GetValueOrDefault("INR", 0),
                        APTT = testDto.Parameters.GetValueOrDefault("APTT", 0),
                        Fibrinogen = testDto.Parameters.GetValueOrDefault("Fibrinogen", 0)
                    };
                    await _dbContext.CoagulogramTests.AddAsync(coagulogram);
                    break;

                case "factorandvwf":
                    var factor = new FactorAndVWFTest
                    {
                        TestId = testId,
                        FactorVIII = testDto.Parameters.GetValueOrDefault("FactorVIII", 0),
                        FactorIX = testDto.Parameters.GetValueOrDefault("FactorIX", 0),
                        VWFActivity = testDto.Parameters.GetValueOrDefault("VWFActivity", 0)
                    };
                    await _dbContext.FactorAndVWFTests.AddAsync(factor);
                    break;

                default:
                    throw new ArgumentException("Неподдерживаемый тип анализа");
            }

            await _dbContext.SaveChangesAsync();

            return test;
        }
        public async Task UpdateTestAsync(Test test)
        {
            var existingTest = await _dbContext.Tests
                .Include(t => t.CbcDetails)
                .Include(t => t.CoagulogramDetails)
                .Include(t => t.FactorAndVwfDetails)
                .FirstOrDefaultAsync(t => t.Id == test.Id);

            if (existingTest == null)
                throw new ArgumentException("Test not found");

            // Обновляем основные свойства
            existingTest.Status = test.Status;
            existingTest.Result = test.Result;
            existingTest.CompletedAt = test.CompletedAt;

            // Обновляем детали анализов
            if (test.CbcDetails != null)
            {
                existingTest.CbcDetails ??= new CompleteBloodCountTest { TestId = test.Id };
                existingTest.CbcDetails.Hemoglobin = test.CbcDetails.Hemoglobin;
                existingTest.CbcDetails.Hematocrit = test.CbcDetails.Hematocrit;
                existingTest.CbcDetails.WhiteBloodCells = test.CbcDetails.WhiteBloodCells;
                existingTest.CbcDetails.RedBloodCells = test.CbcDetails.RedBloodCells;
                existingTest.CbcDetails.Platelets = test.CbcDetails.Platelets;
                existingTest.CbcDetails.MCH = test.CbcDetails.MCH;
                existingTest.CbcDetails.MCV = test.CbcDetails.MCV;
            }

            if (test.CoagulogramDetails != null)
            {
                existingTest.CoagulogramDetails ??= new CoagulogramTest { TestId = test.Id };
                existingTest.CoagulogramDetails.PT = test.CoagulogramDetails.PT;
                existingTest.CoagulogramDetails.INR = test.CoagulogramDetails.INR;
                existingTest.CoagulogramDetails.APTT = test.CoagulogramDetails.APTT;
                existingTest.CoagulogramDetails.Fibrinogen = test.CoagulogramDetails.Fibrinogen;
            }

            if (test.FactorAndVwfDetails != null)
            {
                existingTest.FactorAndVwfDetails ??= new FactorAndVWFTest { TestId = test.Id };
                existingTest.FactorAndVwfDetails.FactorVIII = test.FactorAndVwfDetails.FactorVIII;
                existingTest.FactorAndVwfDetails.FactorIX = test.FactorAndVwfDetails.FactorIX;
                existingTest.FactorAndVwfDetails.VWFActivity = test.FactorAndVwfDetails.VWFActivity;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteTestAsync(Guid id)
        {
            var test = await _dbContext.Tests.FindAsync(id);
            if (test == null) return false;

            _dbContext.Tests.Remove(test);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<Test?> GetTestByReceptionIdAndTypeAsync(Guid receptionId, string testType)
        {
            return await _dbContext.Tests
                .FirstOrDefaultAsync(t => t.ReceptionId == receptionId && t.TestType == testType);
        }

        public async Task DeleteAsync(Test test)
        {
            _dbContext.Tests.Remove(test);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<Test>> EnrichTestResults(List<Test> tests)
        {
            foreach (var test in tests)
            {
                switch (test.TestType.ToLower())
                {
                    case "cbc":
                        var cbc = await _dbContext.CompleteBloodCountTests
                            .FirstOrDefaultAsync(d => d.TestId == test.Id);
                        test.Parameters = new Dictionary<string, double>
                        {
                            ["Hemoglobin"] = cbc?.Hemoglobin ?? 0,
                            ["Hematocrit"] = cbc?.Hematocrit ?? 0,
                            ["WBC"] = cbc?.WhiteBloodCells ?? 0,
                            ["RBC"] = cbc?.RedBloodCells ?? 0,
                            ["Platelets"] = cbc?.Platelets ?? 0,
                            ["MCH"] = cbc?.MCH ?? 0,
                            ["MCV"] = cbc?.MCV ?? 0
                        };
                        break;

                    case "coagulogram":
                        var coag = await _dbContext.CoagulogramTests
                            .FirstOrDefaultAsync(d => d.TestId == test.Id);
                        test.Parameters = new Dictionary<string, double>
                        {
                            ["PT"] = coag?.PT ?? 0,
                            ["INR"] = coag?.INR ?? 0,
                            ["APTT"] = coag?.APTT ?? 0,
                            ["Fibrinogen"] = coag?.Fibrinogen ?? 0
                        };
                        break;

                    case "factorandvwf":
                        var factor = await _dbContext.FactorAndVWFTests
                            .FirstOrDefaultAsync(d => d.TestId == test.Id);
                        test.Parameters = new Dictionary<string, double>
                        {
                            ["FactorVIII"] = factor?.FactorVIII ?? 0,
                            ["FactorIX"] = factor?.FactorIX ?? 0,
                            ["VWFActivity"] = factor?.VWFActivity ?? 0
                        };
                        break;
                }
            }

            return tests;
        }
        public async Task<List<Test>?> GetTestsByReceptionIdAsync(Guid receptionId)
        {
            return await _dbContext.Tests
                .Where(t => t.ReceptionId == receptionId)
                .ToListAsync();
        }

        public async Task<List<Reception>> GetReceptionsByPatientIdAsync(Guid patientId)
        {
            return await _dbContext.Receptions
                .Where(r => r.PatientId == patientId)
                .Include(r => r.Slot)
                .ToListAsync();
        }
    }
}
