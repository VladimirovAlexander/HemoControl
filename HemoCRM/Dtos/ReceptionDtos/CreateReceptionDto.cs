using HemoCRM.Web.Models;

namespace HemoCRM.Web.Dtos.ReceptionDtos
{
    public class CreateReceptionDto
    {
        public Guid DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}
