using HemoCRM.Web.Dtos.MedicationDtos;
using HemoCRM.Web.Models;

namespace HemoCRM.Web.Interfaces
{
    public interface IMedicationRepository
    {
        Task<List<Medication>> GetAllAsync();
        Task<Medication?> GetByIdAsync(Guid id);
        Task<Medication> CreateAsync(CreateMedicationDto dto);
        Task<Medication?> UpdateAsync(Guid id, UpdateMedicationDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<double?> GetRemainingQuantityAsync(Guid patientId, string medicationName);
        Task<Medication> PrescribeMedicationAsync(PrescribeMedicationDto dto);
    }
}
