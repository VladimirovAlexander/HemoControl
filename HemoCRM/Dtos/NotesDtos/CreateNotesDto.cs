namespace HemoCRM.Web.Dtos.NotesDtos
{
    public class CreateNotesDto
    {
        public Guid ReceptionId { get; set; }
        public string Anamnesis { get; set; }
        public string Complaints { get; set; }

        #region Объективный статус
        public string GeneralConditions { get; set; } 
        public string Physique { get; set; }
        public double Weight { get; set; }  
        public double Height { get; set; }
        public byte BloodPressureSystolic { get; set; }    
        public byte BloodPressureDiastolic { get; set; }
        public double Temperature { get; set; }
        public string State { get; set; } 
        public string Skin { get; set; }
        #endregion

        public string Examination { get; set; } 
        public string Treatment { get; set; } 
        public string Recommendations { get; set; } 
        public string Turnout { get; set; } 
    }
}
