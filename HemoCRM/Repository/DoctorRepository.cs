using HemoCRM.Web.Constants;
using HemoCRM.Web.Data;
using HemoCRM.Web.Dtos.DoctorDtos;
using HemoCRM.Web.Interfaces;
using HemoCRM.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System;

namespace HemoCRM.Web.Repository
{
    public class DoctorRepository: IDoctorRepository
    {
        private readonly HemoCrmDbContext _context;
        public DoctorRepository(HemoCrmDbContext context)
        {
            _context = context; 
        }

        public async Task<Doctor> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
        {
            var doctor = new Doctor()
            {
                FirstName = createDoctorDto.FirstName,
                LastName = createDoctorDto.LastName,
                PhoneNumber = createDoctorDto.PhoneNumber,
                Email = createDoctorDto.Email,
                Specialty = createDoctorDto.Specialty
            };
            await _context.Doctors.AddAsync(doctor);    
            await _context.SaveChangesAsync();
            return doctor;
        }

        public async Task<Doctor> GetDoctorByIdAsync(Guid id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if(doctor == null)
            {
                return null;
            }
            return doctor;
        }

        public async Task<List<Doctor>> GetDoctorsAsync()
        {
            var doctorList = await _context.Doctors.ToListAsync();
            return doctorList;
        }

        public async Task<List<Doctor>> GetDoctorsBySpecialtyAsync(string specialties)
        {
            var doctorList = await _context.Doctors.Where(x => x.Specialty == specialties).ToListAsync();
            return doctorList;
        }

        public async Task<Doctor> UpdateDoctorAsync(UpdateDoctorDataDto updateDoctorDataDto, Guid id)
        {
            var doctor = await GetDoctorByIdAsync(id);

            if(doctor == null)
            {
                return null;
            }
            doctor.FirstName = updateDoctorDataDto.FirstName;
            doctor.LastName = updateDoctorDataDto.LastName;
            doctor.PhoneNumber = updateDoctorDataDto.PhoneNumber;
            doctor.Email = updateDoctorDataDto.Email;
            doctor.Specialty = updateDoctorDataDto.Specialty;
            doctor.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return doctor;
        }
    }
}
