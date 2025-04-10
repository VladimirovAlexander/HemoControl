using HemoCRM.Web.Data;
using HemoCRM.Web.Dtos.PatirntDtos;
using HemoCRM.Web.Interfaces;
using HemoCRM.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HemoCRM.Web.Repository
{
    public class PatientRepository : IPatientRepository
    {   
        private readonly HemoCrmDbContext _dbContext;
        public PatientRepository(HemoCrmDbContext dbContext) { 
        
            _dbContext = dbContext;
        }
        public async Task<List<Patient>?> GetPatientsAsync()
        {
            var patientModel = await _dbContext.Patients.ToListAsync();    
            if (patientModel.Count() == 0)
            {

                return null;
                
            }
            return patientModel;
        }
        public async Task<Patient> CreatePatientAsync(CreatePatientDto patientDto)
        {
            var patient = new Patient()
            {
                Name = patientDto.Name,
                Surname = patientDto.Surname,
                Patronymic = patientDto.Patronymic,
                DateOfBirth = patientDto.DateOfBirth,
                Policy = patientDto.Policy,
                Country = patientDto.Country,
                Region = patientDto.Region,
                City = patientDto.City,
                Street = patientDto.Street,
                HouseNumber = patientDto.HouseNumber,
                AppartmentNumber = patientDto.AppartmentNumber
            };

            await _dbContext.Patients.AddAsync(patient);
            await _dbContext.SaveChangesAsync();
            return patient;
        }
        public async Task<Patient?> GetPatientByIdAsync(Guid id)
        {
            var patient = await _dbContext.Patients.FirstOrDefaultAsync(patient => patient.Id == id);
            return patient;
        }
        public async Task<Patient> UpdatePatientDataAsync(UpdatePatientDataDto updatePatientDataDto,Guid id)
        {
            var patient = await GetPatientByIdAsync(id);

            if(patient == null)
            {
                return null;
            }
            patient.Name = updatePatientDataDto.Name;
            patient.UserId = updatePatientDataDto.UserId;
            patient.Surname = updatePatientDataDto.Surname;
            patient.Patronymic = updatePatientDataDto.Patronymic;
            patient.DateOfBirth = updatePatientDataDto.DateOfBirth;
            patient.Policy = updatePatientDataDto.Policy;
            patient.Country = updatePatientDataDto.Country;
            patient.Region = updatePatientDataDto.Region;
            patient.City = updatePatientDataDto.City;
            patient.Street = updatePatientDataDto.Street;
            patient.HouseNumber = updatePatientDataDto.HouseNumber;
            patient.AppartmentNumber = updatePatientDataDto.AppartmentNumber;

            _dbContext.Patients.Update(patient); 
            await _dbContext.SaveChangesAsync(); 

            return patient;
        }
        public async Task<bool> CheckUserRegistration(Guid id)
        {
            var patient = await GetPatientByIdAsync(id);

            if (patient == null)
            {
                throw new Exception("Пользователь не найден");
            }
            if (patient.UserId == Guid.Empty)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
