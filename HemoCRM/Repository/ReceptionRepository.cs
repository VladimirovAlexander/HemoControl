using HemoCRM.Web.Data;
using HemoCRM.Web.Dtos.ReceptionDtos;
using HemoCRM.Web.Interfaces;
using HemoCRM.Web.Models;
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
            var receptions = await _dbContext.Receptions.Where(x => x.DoctorId == doctorId).ToListAsync();
            if (receptions.Count == 0)
            {
                return null;
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
                Notes = dto.Notes,
                Status = "Запланировано",
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Receptions.Add(reception);
            await _dbContext.SaveChangesAsync();

            return CreateReceptionResult.Ok(reception.Id);
        }
    }
}
