namespace HemoCRM.Models
{
    public class Diagnosis
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public int DiagnosisCode { get; set; }
        public string DiagnosisName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Patient Patient { get; set; }
    }
}
