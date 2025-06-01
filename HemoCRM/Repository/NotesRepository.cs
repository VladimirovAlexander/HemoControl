using HemoCRM.Web.Data;
using HemoCRM.Web.Dtos.NotesDtos;
using HemoCRM.Web.Dtos.PatirntDtos;
using HemoCRM.Web.Interfaces;
using HemoCRM.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HemoCRM.Web.Repository
{
    public class NotesRepository : INotesRepository
    {
        private readonly HemoCrmDbContext _dbContext;

        public NotesRepository(HemoCrmDbContext dbContext) 
        { 
            _dbContext = dbContext;
        }

        public async Task CreateNotesAsync(CreateNotesDto createNotesDto)
        {
            var notes = new Notes()
            {
                ReceptionId = createNotesDto.ReceptionId,
                Anamnesis = createNotesDto.Anamnesis,
                Complaints = createNotesDto.Complaints,
                GeneralConditions = createNotesDto.GeneralConditions,
                Physique = createNotesDto.Physique,
                Weight = createNotesDto.Weight,
                Height = createNotesDto.Height,
                BloodPressureSystolic = createNotesDto.BloodPressureSystolic,
                BloodPressureDiastolic = createNotesDto.BloodPressureDiastolic,
                Temperature = createNotesDto.Temperature,
                State = createNotesDto.State,
                Skin = createNotesDto.Skin,
                Examination = createNotesDto.Examination,
                Treatment = createNotesDto.Treatment,
                Recommendations = createNotesDto.Recommendations,
                Turnout = createNotesDto.Turnout
            };

            await _dbContext.Notes.AddAsync(notes);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Notes> GetNotesByIdAsync(Guid id)
        {
            var notes = await _dbContext.Notes.FirstOrDefaultAsync(x => x.Id == id);
            if (notes == null)
            {
                return null;
            }
            return notes;
        }

        public async Task<List<Notes>> GetNotesListAsync()
        {
            var notesList = await _dbContext.Notes.ToListAsync();
            return notesList;
        }

        public async Task<Notes> UpdateNotesAsync(Guid id, UpdateNotesDto dto)
        {
            var existingNote = await _dbContext.Notes.FindAsync(id);
            if (existingNote == null)
            {
                return null;
            }

            // Обновляем поля
            existingNote.Anamnesis = dto.Anamnesis;
            existingNote.Complaints = dto.Complaints;
            existingNote.GeneralConditions = dto.GeneralConditions;
            existingNote.Physique = dto.Physique;
            existingNote.Weight = dto.Weight;
            existingNote.Height = dto.Height;
            existingNote.BloodPressureSystolic = dto.BloodPressureSystolic;
            existingNote.BloodPressureDiastolic = dto.BloodPressureDiastolic;
            existingNote.Temperature = dto.Temperature;
            existingNote.State = dto.State;
            existingNote.Skin = dto.Skin;
            existingNote.Examination = dto.Examination;
            existingNote.Treatment = dto.Treatment;
            existingNote.Recommendations = dto.Recommendations;
            existingNote.Turnout = dto.Turnout;

            await _dbContext.SaveChangesAsync();
            return existingNote;
        }
    }
}
