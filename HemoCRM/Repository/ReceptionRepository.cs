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

        public async Task<Reception?> CreateReceptionAsync(CreateReceptionDto dto)
        {
            var isTaken = await _dbContext.Receptions.AnyAsync(r =>
                r.DoctorId == dto.DoctorId &&
                r.AppointmentDate == dto.AppointmentDateTime);

            if (isTaken)
            {
                return null;
            }

            var reception = new Reception
            {
                Id = Guid.NewGuid(),
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                AppointmentDate = dto.AppointmentDateTime.ToUniversalTime(),
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Receptions.Add(reception);
            await _dbContext.SaveChangesAsync();

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

        public async Task<Reception> UpdateReceptionAsync(UpdateReceptionDto receptionDto, Guid receptionId)
        {
            var reception = await _dbContext.Receptions.FindAsync(receptionId);

            if (reception == null)
                throw new Exception("Reception not found");

            var doctor = await _dbContext.Doctors.FindAsync(receptionDto.DoctorId);

            if (doctor == null)
                throw new Exception("Doctor not found");

            reception.Doctor = doctor;
            reception.DoctorId = receptionDto.DoctorId;
            reception.Notes = receptionDto.Notes;
            reception.Status = receptionDto.Status;
            switch (receptionDto.Status)
            {
                case ReceptionsStatus.MovedBack:
                        
                    reception.AppointmentDate = receptionDto.AppointmentDate;
                    break;
                case ReceptionsStatus.Completed:
                    reception.CompletedAt = DateTime.UtcNow;
                    break;
                default:
                    break;
            }
            
            _dbContext.Receptions.Update(reception);
            await _dbContext.SaveChangesAsync();

            return reception;
        }

        public async Task<List<DateTime>> GetAvailableSlotsAsync(Guid doctorId, DateTime date)
        {
            var schedule = await _dbContext.DoctorSchedules
                .FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.DayOfWeek == date.DayOfWeek);
            if (schedule == null) return new List<DateTime>();

            var start = schedule.StartTime;
            var end = schedule.EndTime;

            var slots = new List<DateTime>();
            var current = start;

            while (current + schedule.AppointmentDuration <= end)
            {
                if (current >= schedule.LunchStart && current < schedule.LunchEnd)
                {
                    current = schedule.LunchEnd;
                    continue;
                }

                slots.Add(date.Date + current);
                current += schedule.AppointmentDuration + schedule.BreakBetweenAppointments;
            }

            var takenTimes = await _dbContext.Receptions
                .Where(r => r.DoctorId == doctorId && r.AppointmentDate.Value.Date == date.Date)
                .Select(r => r.AppointmentDate.Value)
                .ToListAsync();

            return slots.Where(s => !takenTimes.Contains(s)).ToList();
        }
    }
}
