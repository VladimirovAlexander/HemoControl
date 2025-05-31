using System.Numerics;

namespace HemoCRM.Web.Models
{
    public class Reception
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid SlotId { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Patient Patient { get; set; } 
        public Doctor Doctor { get; set; }
        public DoctorAppointmentSlot Slot { get; set; }
        public Notes Notes { get; set; }
    }
}
