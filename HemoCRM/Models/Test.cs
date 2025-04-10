namespace HemoCRM.Web.Models
{
    public class Test
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string TestName { get; set; }
        public string Status { get; set; }
        public string Result { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }

        public Patient Patient { get; set; }
    }
}
