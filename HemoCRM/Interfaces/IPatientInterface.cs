using HemoCRM.Dtos;
using HemoCRM.Models;

namespace HemoCRM.Interfaces
{
    public interface IPatientInterface
    {
        Task<List<Patient>> GetPatientsAsync();

        Task<Patient> CreatePatientAsync(CreatePatientDto patientDto);

        Task<Patient?> GetPatientByIdAsync(Guid id);

        Task<Patient?> UpdatePatientDataAsync(UpdatePatientDataDto updatePatientDataDto, Guid id);
    }
}
