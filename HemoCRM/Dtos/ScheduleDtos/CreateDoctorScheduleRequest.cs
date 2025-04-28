namespace HemoCRM.Web.Dtos.ScheduleDtos
{
    public class CreateDoctorScheduleRequest
    {
        public Guid DoctorId { get; set; }
        public List<DoctorScheduleDay> Days { get; set; } = new();
    }
    public class DoctorScheduleDay
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan AppointmentDuration { get; set; } = TimeSpan.FromMinutes(20);
        public TimeSpan BreakBetweenAppointments { get; set; } = TimeSpan.FromMinutes(10);
        public TimeSpan LunchStart { get; set; } = new TimeSpan(12, 0, 0);
        public TimeSpan LunchEnd { get; set; } = new TimeSpan(13, 0, 0);
    }
}
