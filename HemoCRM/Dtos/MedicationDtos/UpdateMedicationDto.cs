namespace HemoCRM.Web.Dtos.MedicationDtos
{
    public class UpdateMedicationDto
    {
        public string? MedicationName { get; set; }
        public string? Dosage { get; set; }
        public string? Instructions { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
