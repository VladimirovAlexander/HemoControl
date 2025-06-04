using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HemoCRM.Web.Models
{
    public class Test
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid ReceptionId { get; set; }

        public string TestName { get; set; } = string.Empty;
        public string TestType { get; set; } = string.Empty;
        public TestStatus Status { get; set; } = TestStatus.Pending;
        public string? Result { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        public Patient Patient { get; set; } = null!;
        public Reception Reception { get; set; } = null!;

        public CompleteBloodCountTest? CbcDetails { get; set; }
        public CoagulogramTest? CoagulogramDetails { get; set; }
        public FactorAndVWFTest? FactorAndVwfDetails { get; set; }

        [NotMapped]
        public Dictionary<string, double>? Parameters { get; set; }
    }

    public class CompleteBloodCountTest
    {
        [Key]
        public Guid TestId { get; set; }  // FK к Test.Id

        public double Hemoglobin { get; set; }
        public double Hematocrit { get; set; }
        public double WhiteBloodCells { get; set; }
        public double RedBloodCells { get; set; }
        public double Platelets { get; set; }
        public double MCH { get; set; }
        public double MCV { get; set; }

        public Test Test { get; set; } = null!;
    }

    public class CoagulogramTest
    {
        [Key]
        public Guid TestId { get; set; }

        public double PT { get; set; }
        public double INR { get; set; }
        public double APTT { get; set; }
        public double Fibrinogen { get; set; }

        public Test Test { get; set; } = null!;
    }

    public class FactorAndVWFTest
    {
        [Key]
        public Guid TestId { get; set; }

        public double FactorVIII { get; set; }
        public double FactorIX { get; set; }
        public double VWFActivity { get; set; }

        public Test Test { get; set; } = null!;
    }

    public enum TestStatus
    {
        Pending,
        InProgress,
        Completed,
        Canceled
    }

}




