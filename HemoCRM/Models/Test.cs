using System.Text.Json.Serialization;

namespace HemoCRM.Web.Models
{
    public class Test
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public string TestName { get; set; } = string.Empty;
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TestStatus Status { get; set; } = TestStatus.Pending;
        public string? Result { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public Patient Patient { get; set; } = null!;
    }
    public enum TestStatus
    {
        Pending,
        InProgress,
        Completed,
        Canceled
    }
}




