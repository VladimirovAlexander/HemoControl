﻿namespace HemoCRM.Web.Dtos.DoctorDtos
{
    public class CreateDoctorDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Surname { get; set; }
        public string Specialty { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public Guid UserId { get; set; }
    }
}
