using HemoCRM.Web.Data;
using HemoCRM.Web.Dtos.InjectionDtos;
using HemoCRM.Web.Interfaces;
using HemoCRM.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HemoCRM.Web.Repository
{
    public class InjectionRepository : IInjectionRepository
    {
        private readonly HemoCrmDbContext _dbContext;

        public InjectionRepository(HemoCrmDbContext context)
        {
            _dbContext = context;
        }

        public async Task<Injection> CreateInjectionAsync(CreateInjectionDto dto)
        {
            var patient = await _dbContext.Patients.FindAsync(dto.PatientId);
            var medication = await _dbContext.Medications.FindAsync(dto.MedicationId);
            var doctor = dto.DoctorId.HasValue ? await _dbContext.Doctors.FindAsync(dto.DoctorId) : null;

            if (patient == null || medication == null)
                throw new Exception("Пациент или медикаменты не найдены");

            var injection = new Injection
            {
                Patient = patient,
                PatientId = patient.Id,
                Medication = medication,
                MedicationId = medication.Id,
                Doctor = doctor,
                DoctorId = doctor?.Id,
                InjectedAt = dto.InjectedAt,
                DoseIU = dto.DoseIU,
                Notes = dto.Notes,
                InjectionSite = dto.InjectionSite,
            };

            await _dbContext.Injections.AddAsync(injection);
            await _dbContext.SaveChangesAsync();

            return injection;
        }

        public async Task<Injection?> GetInjectionByIdAsync(Guid id)
        {
            var injection = await _dbContext.Injections.FindAsync(id);
            return injection;
        }

        public async Task<List<Injection>> GetPatientInjectionsAsync(Guid patientId)
        {
            return await _dbContext.Injections
                .Where(i => i.PatientId == patientId)
                .OrderByDescending(i => i.InjectedAt)
                .ToListAsync();
        }

        public async Task<List<Injection>> GetAllInjectionsAsync()
        {
            return await _dbContext.Injections.ToListAsync();
        }
    }
}
