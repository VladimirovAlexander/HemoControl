namespace HemoCRM.Web.Models
{
    public class DoctorSchedule
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; } 
        public TimeSpan EndTime { get; set; }  
        public TimeSpan AppointmentDuration { get; set; } = TimeSpan.FromMinutes(20);
        public TimeSpan BreakBetweenAppointments { get; set; } = TimeSpan.FromMinutes(10);
        public TimeSpan LunchStart { get; set; } = new TimeSpan(12, 0, 0);
        public TimeSpan LunchEnd { get; set; } = new TimeSpan(13, 0, 0);

        public Doctor Doctor { get; set; } = null!;
    }
}
