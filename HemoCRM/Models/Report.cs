namespace HemoCRM.Models
{
    public class Report
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string ReportType { get; set; }
        
        //JSON для передачи в форму
        public string Data { get; set; }  
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        
        public Patient Patient { get; set; } 
    }
}
