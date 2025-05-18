using HemoCRM.Web.Models;

namespace HemoCRM.Web.Interfaces
{
    public interface IScheduleRepository
    {
        Task<List<DoctorAppointmentSlot>> CreateDoctorSlots(List<DoctorAppointmentSlot> slots);
        Task<List<DoctorAppointmentSlot>> GetDoctorSlots(Guid doctorId);
        Task<List<DateTime>> GetDoctorSlotDays(Guid doctorId);
        Task<List<TimeSpan>> GetDoctorSlotTimesOnDay(Guid doctorId, DateTime date);
    }
}

