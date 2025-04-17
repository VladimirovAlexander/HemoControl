using System.ComponentModel.DataAnnotations;

namespace HemoCRM.Web.Dtos.MedicationDtos
{
    public class CreateMedicationDto
    {
        [Required]
        public Guid PatientId { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Instructions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public double Quantity { get; set; }
    }
}
