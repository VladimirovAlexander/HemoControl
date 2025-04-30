using HemoCRM.Web.Data;
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

        public async Task<List<DateTime>> GetAvailableDaysAsync(Guid doctorId)
        {
            var days = new List<DateTime>();
            var today = DateTime.UtcNow.Date;
            var schedule = await _context.DoctorSchedules.FirstOrDefaultAsync(s => s.DoctorId == doctorId);

            if (schedule == null) return days;

            for (int i = 0; i < 30; i++)
            {
                var date = today.AddDays(i);
                var times = await GetAvailableTimesAsync(doctorId, date);

                if (times.Any())
                {
                    days.Add(date);
                }
            }

            return days;
        }


        public async Task<List<TimeSpan>> GetAvailableTimesAsync(Guid doctorId, DateTime date)
        {
            date = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

            var schedule = await _context.DoctorSchedules.FirstOrDefaultAsync(s => s.DoctorId == doctorId);
            if (schedule == null) return new List<TimeSpan>();

            var allAppointments = await _context.Receptions
                .Where(r => r.DoctorId == doctorId && r.AppointmentDate.HasValue && r.AppointmentDate.Value.Date == date.Date)
                .ToListAsync();

            List<TimeSpan> availableTimes = new();

            TimeSpan time = schedule.StartTime;
            while (time + schedule.AppointmentDuration <= schedule.EndTime)
            {
                bool inLunchBreak = time >= schedule.LunchStart && time < schedule.LunchEnd;
                bool isBooked = allAppointments.Any(r =>
                    r.AppointmentDate.Value.TimeOfDay >= time &&
                    r.AppointmentDate.Value.TimeOfDay < time + schedule.AppointmentDuration
                );

                if (!inLunchBreak && !isBooked)
                {
                    availableTimes.Add(time);
                }

                time += schedule.AppointmentDuration + schedule.BreakBetweenAppointments;
            }

            return availableTimes;
        }

        public async Task AddDoctorSchedulesAsync(IEnumerable<DoctorSchedule> schedules)
        {
            await _context.DoctorSchedules.AddRangeAsync(schedules);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DoctorSchedule>> GetDoctorSchedulesAsync(Guid doctorId)
        {
            return await _context.DoctorSchedules
                .Where(s => s.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task DeleteSchedulesByDoctorAsync(Guid doctorId)
        {
            var existing = await _context.DoctorSchedules
                .Where(x => x.DoctorId == doctorId)
                .ToListAsync();

            _context.DoctorSchedules.RemoveRange(existing);
            await _context.SaveChangesAsync();
        }
    }
}
