using HemoCRM.Web.Models;

public class UpdateTestRequest
{
    public UpdateTestResultsDto Dto { get; set; }
}

public class UpdateTestResultsDto
{
    public Guid TestId { get; set; }
    public string TestType { get; set; } = string.Empty;
    public string Status { get; set; }
    public string? Result { get; set; }
    public TestResultsDto Results { get; set; } = new();
}

public class TestResultsDto
{
    // CBC
    public double Hemoglobin { get; set; }
    public double Hematocrit { get; set; }
    public double WhiteBloodCells { get; set; }
    public double RedBloodCells { get; set; }
    public double Platelets { get; set; }
    public double MCH { get; set; }
    public double MCV { get; set; }

    // Coagulogram
    public double PT { get; set; }
    public double INR { get; set; }
    public double APTT { get; set; }
    public double Fibrinogen { get; set; }

    // FactorAndVWF
    public double FactorVIII { get; set; }
    public double FactorIX { get; set; }
    public double VWFActivity { get; set; }
}