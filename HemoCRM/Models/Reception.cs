using System.Numerics;

namespace HemoCRM.Web.Models
{
    public class Reception
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
        public Patient Patient { get; set; } 
        public Doctor Doctor { get; set; } 
    }
}
