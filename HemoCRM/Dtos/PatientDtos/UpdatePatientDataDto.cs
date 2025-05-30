﻿namespace HemoCRM.Web.Dtos.PatirntDtos
{
    public class UpdatePatientDataDto
    {
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public string? Surname { get; set; } = string.Empty;
        public string? Patronymic { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Policy { get; set; } = string.Empty;
        public string? Passport { get; set; } = string.Empty;
        public string? Country { get; set; } = string.Empty;
        public string? Region { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? Street { get; set; } = string.Empty;
        public int? HouseNumber { get; set; }
        public int? AppartmentNumber { get; set; }
    }
}
