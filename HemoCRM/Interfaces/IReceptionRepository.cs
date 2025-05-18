using HemoCRM.Web.Dtos.ReceptionDtos;
using HemoCRM.Web.Models;

namespace HemoCRM.Web.Interfaces
{
    public interface IReceptionRepository
    {
        Task<Reception> GetReceptionByIdAsync(Guid receptionId);

        Task<List<Reception>?> GetReceptionsAsync();

        Task<List<Reception>?> GetUserReceptionsAsync(Guid userId);

        Task<List<Reception>?> GetDoctorReceptionsAsync(Guid doctorId);

        Task<bool> DeleteReceptionAsync(Guid receptionId);

        Task<CreateReceptionResult> CreateReceptionAsync(CreateReceptionDto dto);
    }
}
