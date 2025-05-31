namespace HemoCRM.Web.Models
{
    public class Doctor
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Surname { get; set; }
        public string Specialty { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Reception> Appointments { get; set; } = new List<Reception>();

        public ICollection<DoctorAppointmentSlot> AppointmentSlots { get; set; }

    }
}
