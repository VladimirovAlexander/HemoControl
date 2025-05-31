namespace HemoCRM.Web.Dtos.ReceptionDtos
{
    public class ReceptionDto
    {
        public Guid Id { get; set; }
        public string PatientFullName { get; set; }
        public DateTime SlotTime { get; set; }
        public string? Notes { get; set; }
    }
}
