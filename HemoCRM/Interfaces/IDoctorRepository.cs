using HemoCRM.Web.Constants;
using HemoCRM.Web.Dtos.DoctorDtos;
using HemoCRM.Web.Models;

namespace HemoCRM.Web.Interfaces
{
    public interface IDoctorRepository
    {
        Task<List<Doctor>> GetDoctorsAsync();

        Task<Doctor> GetDoctorByIdAsync(Guid id);

        Task<Doctor> CreateDoctorAsync(CreateDoctorDto createDoctorDto);

        Task<Doctor> UpdateDoctorAsync(UpdateDoctorDataDto updateDoctorDataDto, Guid id);

        Task<List<Doctor>> GetDoctorsBySpecialtyAsync(string specialties);
    }
}
