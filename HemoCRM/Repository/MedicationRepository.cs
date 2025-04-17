using HemoCRM.Web.Data;
using HemoCRM.Web.Dtos.MedicationDtos;
using HemoCRM.Web.Interfaces;
using HemoCRM.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HemoCRM.Web.Repository
{
    public class MedicationRepository : IMedicationRepository
    {
        private readonly HemoCrmDbContext _dbContext;

        public MedicationRepository(HemoCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Medication>> GetAllAsync()
        {
            return await _dbContext.Medications
                .Include(m => m.Patient)
                .ToListAsync();
        }

        public async Task<Medication?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Medications
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Medication> CreateAsync(CreateMedicationDto dto)
        {
            var patient = await _dbContext.Patients.FindAsync(dto.PatientId);
            if (patient == null)
                throw new Exception("Пациент не найден");

            var medication = new Medication
            {
                Id = Guid.NewGuid(),
                PatientId = dto.PatientId,
                MedicationName = dto.MedicationName,
                Dosage = dto.Dosage,
                Instructions = dto.Instructions,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Medications.AddAsync(medication);
            await _dbContext.SaveChangesAsync();

            return medication;
        }

        public async Task<Medication?> UpdateAsync(Guid id, UpdateMedicationDto dto)
        {
            var medication = await _dbContext.Medications.FindAsync(id);
            if (medication == null)
                return null;

            if (!string.IsNullOrWhiteSpace(dto.MedicationName))
                medication.MedicationName = dto.MedicationName;

            if (!string.IsNullOrWhiteSpace(dto.Dosage))
                medication.Dosage = dto.Dosage;

            if (!string.IsNullOrWhiteSpace(dto.Instructions))
                medication.Instructions = dto.Instructions;

            if (dto.StartDate.HasValue)
                medication.StartDate = dto.StartDate.Value;

            medication.EndDate = dto.EndDate;

            _dbContext.Medications.Update(medication);
            await _dbContext.SaveChangesAsync();

            return medication;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var medication = await _dbContext.Medications.FindAsync(id);
            if (medication == null)
                return false;

            _dbContext.Medications.Remove(medication);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<double?> GetRemainingQuantityAsync(Guid patientId, string medicationName)
        {
            var med = await _dbContext.Medications
                .Where(m => m.PatientId == patientId && m.MedicationName == medicationName)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync();

            return med?.Quantity;
        }

        public async Task<Medication> PrescribeMedicationAsync(PrescribeMedicationDto dto)
        {
            var patient = await _dbContext.Patients.FindAsync(dto.PatientId);
            if (patient == null)
                throw new Exception("Пациент не найден");

            var medication = new Medication
            {
                Id = Guid.NewGuid(),
                PatientId = dto.PatientId,
                MedicationName = dto.MedicationName,
                Dosage = dto.Dosage,
                Instructions = dto.Instructions,
                StartDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Quantity = dto.Quantity
            };

            await _dbContext.Medications.AddAsync(medication);
            await _dbContext.SaveChangesAsync();

            return medication;
        }

    }
}
