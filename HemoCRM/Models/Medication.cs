namespace HemoCRM.Web.Models
{
    public class Medication
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Instructions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Patient Patient { get; set; }
    }
}
