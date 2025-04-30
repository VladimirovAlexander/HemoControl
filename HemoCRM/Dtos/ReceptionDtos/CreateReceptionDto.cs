using HemoCRM.Web.Models;

namespace HemoCRM.Web.Dtos.ReceptionDtos
{
    public class CreateReceptionDto
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string? Notes { get; set; }
    }
}
