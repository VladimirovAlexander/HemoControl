using HemoCRM.Web.Dtos.NotesDtos;
using HemoCRM.Web.Models;

namespace HemoCRM.Web.Interfaces
{
    public interface INotesRepository
    {
        Task CreateNotesAsync(CreateNotesDto createNotesDto);
        Task<List<Notes>> GetNotesListAsync();
        Task<Notes> GetNotesByIdAsync(Guid id);
        Task<Notes> UpdateNotesAsync(Guid id, UpdateNotesDto dto);

    }
}
