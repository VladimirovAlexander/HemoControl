using HemoCRM.Dtos.PatirntDtos;
using HemoCRM.Models;

namespace HemoCRM.Interfaces
{
    public interface IPatientRepository
    {
        Task<List<Patient>> GetPatientsAsync();

        Task<Patient> CreatePatientAsync(CreatePatientDto patientDto);

        Task<Patient?> GetPatientByIdAsync(Guid id);

        Task<Patient?> UpdatePatientDataAsync(UpdatePatientDataDto updatePatientDataDto, Guid id);

        Task<bool> CheckUserRegistration(Guid id);

    }
}
