using HemoCRM.Web.Dtos.NotesDtos;
using HemoCRM.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HemoCRM.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly INotesRepository _notesRepository;

        public NotesController(INotesRepository notesRepository)
        {
            _notesRepository = notesRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotes([FromBody] CreateNotesDto createNotesDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _notesRepository.CreateNotesAsync(createNotesDto);
            return Ok("Осмотр успешно добавлен.");
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetNotesById(Guid id)
        {
            var note = await _notesRepository.GetNotesByIdAsync(id);
            if (note == null)
                return NotFound("Осмотр не найден");

            return Ok(note);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            var notesList = await _notesRepository.GetNotesListAsync();
            return Ok(notesList);
        }
    }
}
