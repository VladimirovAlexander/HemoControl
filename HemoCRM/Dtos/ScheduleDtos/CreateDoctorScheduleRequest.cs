using HemoCRM.Web.Models;

namespace HemoCRM.Web.Dtos.ScheduleDtos
{
    public class CreateDoctorScheduleRequest
    {
        public Guid DoctorId { get; set; }
        public DateTime StartDateTime { get; set; }
        public int NumberOfAppointments {  get; set; }
    }
}
