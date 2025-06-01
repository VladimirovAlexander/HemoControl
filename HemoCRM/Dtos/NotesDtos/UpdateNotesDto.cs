namespace HemoCRM.Web.Dtos.NotesDtos
{
    public class UpdateNotesDto
    {
        public string Anamnesis { get; set; }
        public string Complaints { get; set; }

        #region Объективный статус
        public string GeneralConditions { get; set; } // объективный статус
        public string Physique { get; set; } // Телосложение
        public double Weight { get; set; } // Вес  
        public double Height { get; set; } // Рост
        public byte BloodPressureSystolic { get; set; } // Верхнее артериально давление     
        public byte BloodPressureDiastolic { get; set; } // Нижнее артериально давление
        public double Temperature { get; set; } // Температура
        public string State { get; set; } // Состояние
        public string Skin { get; set; } // Кожа
        #endregion

        public string Examination { get; set; } // Обследование
        public string Treatment { get; set; } // Лечение
        public string Recommendations { get; set; } // Рекомендации
        public string Turnout { get; set; } // Явка
    }
}
