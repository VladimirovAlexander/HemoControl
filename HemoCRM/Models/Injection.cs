namespace HemoCRM.Web.Models
{
    public class Injection
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }
        public Guid MedicationId { get; set; }
        public Medication Medication { get; set; }
        public DateTime InjectedAt { get; set; } = DateTime.UtcNow;
        public int DoseIU { get; set; }
        public Guid? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
        public string? Notes { get; set; }
        public string? InjectionSite { get; set; }
    }
}
