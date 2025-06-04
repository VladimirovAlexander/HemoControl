using HemoCRM.Web.Data;
using HemoCRM.Web.Dtos.ReceptionDtos;
using HemoCRM.Web.Dtos.TestDtos;
using HemoCRM.Web.Interfaces;
using HemoCRM.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HemoCRM.Web.Repository
{
    public class ReceptionRepository : IReceptionRepository
    {

        private readonly HemoCrmDbContext _dbContext;
        public ReceptionRepository(HemoCrmDbContext hemoCrmDbContext) 
        {
            _dbContext = hemoCrmDbContext;
        }
        public async Task<Reception?> GetReceptionByIdAsync(Guid id)
        {
            var reception = await _dbContext.Receptions.FindAsync(id);

            if (reception == null)
            {
                return null;
            }
            return reception;
        }

        public async Task<bool> DeleteReceptionAsync(Guid receptionId)
        {
            var reception = await _dbContext.Receptions.FindAsync(receptionId);

            if (reception != null)
            {
                reception.Status = ReceptionsStatus.Canceled;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Reception>?> GetDoctorReceptionsAsync(Guid doctorId)
        {
            var receptions = await _dbContext.Receptions
                .Where(x => x.DoctorId == doctorId)
                .Include(r => r.Patient)
                .Include(r => r.Slot)
                .Include(r => r.Notes)
                .Include(r => r.Tests)
                .AsSplitQuery()
                .ToListAsync();

            if (!receptions.Any()) return null;

            foreach (var reception in receptions)
            {
                foreach (var test in reception.Tests)
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
            }

            return receptions;
        }

        public async Task<List<Reception>?> GetReceptionsAsync()
        {
            var receptions = await _dbContext.Receptions.ToListAsync();
            if (receptions.Count == 0)
            {
                return receptions;
            }
            return receptions;
        }

        public async Task<List<Reception>?> GetUserReceptionsAsync(Guid userId)
        {
            var receptions = await _dbContext.Receptions.Where(x => x.PatientId == userId).ToListAsync();
            if (receptions.Count == 0)
            {
                return null;
            }
            return receptions;
        }

        public async Task<CreateReceptionResult> CreateReceptionAsync(CreateReceptionDto dto)
        {
            var slot = await _dbContext.DoctorAppointmentSlots
                .FirstOrDefaultAsync(s => s.Id == dto.SlotId && s.DoctorId == dto.DoctorId);

            if (slot == null)
                return CreateReceptionResult.Fail("Слот не найден.");

            var existing = await _dbContext.Receptions
                .AnyAsync(r => r.SlotId == dto.SlotId);

            if (existing)
                return CreateReceptionResult.Fail("Слот уже занят.");

            var reception = new Reception
            {
                Id = Guid.NewGuid(),
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                SlotId = dto.SlotId,

                Notes = new Notes
                {
                    Anamnesis = string.Empty,
                    Complaints = string.Empty,
                    GeneralConditions = string.Empty,
                    Physique = string.Empty,
                    State = string.Empty,
                    Skin = string.Empty,
                    Examination = string.Empty,
                    Treatment = string.Empty,
                    Recommendations = string.Empty,
                    Turnout = string.Empty
                },
                Status = "Запланировано",
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Receptions.Add(reception);
            await _dbContext.SaveChangesAsync();

            return CreateReceptionResult.Ok(reception.Id);
        }

        public async Task<bool> AssignTestsToReceptionAsync(Guid receptionId, AssignTestsDto dto)
        {
            var reception = await _dbContext.Receptions.FindAsync(receptionId);
            var patient = await _dbContext.Patients.FindAsync(dto.PatientId);

            if (reception == null || patient == null)
                return false;

            var existingTestTypes = await _dbContext.Tests
                .Where(t => t.PatientId == dto.PatientId && t.ReceptionId == receptionId)
                .Select(t => t.TestType)
                .ToListAsync();

            bool anyAdded = false;

            foreach (var testDto in dto.Tests)
            {
                if (existingTestTypes.Contains(testDto.TestType))
                    continue;

                var test = new Test
                {
                    Id = Guid.NewGuid(),
                    PatientId = dto.PatientId,
                    ReceptionId = receptionId,
                    TestName = testDto.TestName,
                    TestType = testDto.TestType,
                    Status = TestStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _dbContext.Tests.AddAsync(test);

                // Добавим детали в соответствующую таблицу
                switch (testDto.TestType)
                {
                    case "CBC":
                        await _dbContext.CompleteBloodCountTests.AddAsync(new CompleteBloodCountTest
                        {
                            TestId = test.Id
                        });
                        break;

                    case "Coagulogram":
                        await _dbContext.CoagulogramTests.AddAsync(new CoagulogramTest
                        {
                            TestId = test.Id
                        });
                        break;

                    case "FactorAndVWF":
                        await _dbContext.FactorAndVWFTests.AddAsync(new FactorAndVWFTest
                        {
                            TestId = test.Id
                        });
                        break;

                    default:
                        throw new ArgumentException("Неверный тип анализа: " + testDto.TestType);
                }

                anyAdded = true;
            }

            if (anyAdded)
                await _dbContext.SaveChangesAsync();

            return anyAdded;
        }
    }
}
