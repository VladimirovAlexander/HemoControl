using HemoCRM.Web.Models;

namespace HemoCRM.Web.Interfaces
{
    public interface IScheduleRepository
    {
        Task<List<DateTime>> GetAvailableDaysAsync(Guid doctorId);
        Task<List<TimeSpan>> GetAvailableTimesAsync(Guid doctorId, DateTime date);
        Task AddDoctorSchedulesAsync(IEnumerable<DoctorSchedule> schedules);
        Task<IEnumerable<DoctorSchedule>> GetDoctorSchedulesAsync(Guid doctorId);
        Task DeleteSchedulesByDoctorAsync(Guid doctorId);
    }
}

