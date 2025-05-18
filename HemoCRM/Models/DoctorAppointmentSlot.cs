namespace HemoCRM.Web.Models
{
    public class DoctorAppointmentSlot
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public Reception Reception { get; set; }
    }
}
