namespace HemoCRM.Web.Dtos.InjectionDtos
{
    public class CreateInjectionDto
    {
        public Guid PatientId { get; set; }
        public Guid MedicationId { get; set; }
        public Guid? DoctorId { get; set; }
        public DateTime InjectedAt { get; set; } = DateTime.UtcNow;
        public int DoseIU { get; set; }
        public string? Notes { get; set; }
        public string? InjectionSite { get; set; }

    }
}
