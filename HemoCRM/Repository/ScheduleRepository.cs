using HemoCRM.Web.Data;
using HemoCRM.Web.Dtos.ScheduleDtos;
using HemoCRM.Web.Interfaces;
using HemoCRM.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HemoCRM.Web.Repository
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly HemoCrmDbContext _context;

        public ScheduleRepository(HemoCrmDbContext context)
        {
            _context = context;
        }

        public async Task<List<DoctorAppointmentSlot>> CreateDoctorSlots(List<DoctorAppointmentSlot> slots)
        {
            if (slots == null || !slots.Any())
                throw new ArgumentException("Список слотов не может быть пустым.");
            await _context.DoctorAppointmentSlots.AddRangeAsync(slots);
            await _context.SaveChangesAsync();
            return slots;
        }

        public Task<List<DoctorAppointmentSlot>> GetDoctorSlots(Guid doctorId)
        {
            if (doctorId == null)
            {
                return null;
            }
            var slots = _context.DoctorAppointmentSlots
                .Where(x => x.DoctorId == doctorId)
                .ToListAsync();
            return slots;
        }
        public async Task<List<DateTime>> GetDoctorSlotDays(Guid doctorId)
        {
            return await _context.DoctorAppointmentSlots
                .Where(s => s.DoctorId == doctorId)
                .Select(s => s.StartDateTime.Date)
                .Distinct()
                .OrderBy(date => date)
                .ToListAsync();
        }

        public async Task<List<SlotDto>> GetDoctorSlotTimesOnDay(Guid doctorId, DateTime date)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);

            return await _context.DoctorAppointmentSlots
                .Where(s => s.DoctorId == doctorId && s.StartDateTime.Date == date)
                .OrderBy(s => s.StartDateTime)
                .Select(s => new SlotDto
                {
                    SlotId = s.Id,
                    Time = s.StartDateTime.TimeOfDay
                })
                .ToListAsync();
        }

        public async Task<DoctorAppointmentSlot> GetSlotById(Guid slotId)
        {
            return await _context.DoctorAppointmentSlots
               .FirstOrDefaultAsync(x => x.Id == slotId)
               ?? throw new InvalidOperationException("Слот с указанным идентификатором не найден.");
        }
    }
}
