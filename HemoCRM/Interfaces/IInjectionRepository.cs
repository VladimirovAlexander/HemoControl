using HemoCRM.Web.Dtos.InjectionDtos;
using HemoCRM.Web.Models;

namespace HemoCRM.Web.Interfaces
{
    public interface IInjectionRepository
    {
        Task<Injection> CreateInjectionAsync(CreateInjectionDto dto);

        Task<Injection?> GetInjectionByIdAsync(Guid id);

        Task<List<Injection>> GetPatientInjectionsAsync(Guid patientId);

        Task<List<Injection>> GetAllInjectionsAsync();
    }
}
