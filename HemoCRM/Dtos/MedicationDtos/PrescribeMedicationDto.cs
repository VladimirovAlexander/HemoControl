namespace HemoCRM.Web.Dtos.MedicationDtos
{
    public class PrescribeMedicationDto
    {
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Instructions { get; set; }
        public double Quantity { get; set; }
    }
}
