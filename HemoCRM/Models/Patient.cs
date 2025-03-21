using System.ComponentModel.DataAnnotations;

namespace HemoCRM.Models
{
    public class Patient
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
       
        public string Policy { get; set; } = string.Empty;
        public string Passport {  get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public int HouseNumber { get; set; }
        public int AppartmentNumber { get; set; }
        
        public ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
        public ICollection<Reception> Receptions { get; set; } = new List<Reception>();
        public ICollection<Test> Tests { get; set; } = new List<Test>();
        public ICollection<Medication> Medications { get; set; } = new List<Medication>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();



    }
}
