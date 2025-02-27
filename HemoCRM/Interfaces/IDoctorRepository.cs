using HemoCRM.Dtos.DoctorDtos;
using HemoCRM.Models;

namespace HemoCRM.Interfaces
{
    public interface IDoctorRepository
    {
        Task<List<Doctor>> GetDoctorsAsync();

        Task<Doctor> GetDoctorByIdAsync(Guid id);

        Task<Doctor> CreateDoctorAsync(CreateDoctorDto createDoctorDto);

        Task<Doctor> UpdateDoctorAsync(UpdateDoctorDataDto updateDoctorDataDto, Guid id);
    }
}
