using HemoCRM.Web.Dtos.ReceptionDtos;
using HemoCRM.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HemoCRM.Web.Controllers
{

    [Route("api/reception")]
    [ApiController]
    public class ReceptionController: Controller
    {
        private readonly IReceptionRepository _receptionRepository;

        public ReceptionController(IReceptionRepository receptionRepository)
        {
            _receptionRepository = receptionRepository;
        }


        [HttpGet("reception/{id}")]
        public async Task<IActionResult> GetReceptionById(Guid id)
        {
            var reception = await _receptionRepository.GetReceptionByIdAsync(id);

            return Ok(reception);
        }

        [HttpGet("reception")]
        public async Task<IActionResult> GetReceptions()
        {
            var receptions = await _receptionRepository.GetReceptionsAsync();
            return Ok(receptions);
        }

        [HttpGet("user-receptions/{userId}")]
        public async Task<IActionResult> GetUsersReceptions(Guid iduserId)
        {
            var receptions = await _receptionRepository.GetUserReceptionsAsync(iduserId);
            return Ok(receptions);
        }

        [HttpGet("doctor-receptions/{doctorId}")]
        public async Task<IActionResult> GetDoctorsReceptions(Guid doctorId)
        {
            var receptions = await _receptionRepository.GetDoctorReceptionsAsync(doctorId);
            return Ok(receptions);
        }

        [HttpPut("deleteReception/{id}")]
        public async Task<IActionResult> DeleteReceptionAsync(Guid id)
        {
            bool receptionIsDeleted = await _receptionRepository.DeleteReceptionAsync(id);
            if (receptionIsDeleted)
            {
                return Ok("Запись отменена");
            }
            else
            {
                return BadRequest("Произошла ошибка");
            }
        }

        [HttpPost("createReception")]
        public async Task<IActionResult> CreateReception([FromBody] CreateReceptionDto createReceptionDto)
        {
            var newReception = await _receptionRepository.CreateReceptionAsync(createReceptionDto);

            if (newReception == null) {

                return BadRequest("Запись не создана");
            }
            return CreatedAtAction(
                nameof(GetReceptionById),
                new { id = newReception.Id },
                newReception  
            );
        }

        [HttpPut("updateReception")]
        public async Task<IActionResult> UpdateReception([FromBody] UpdateReceptionDto updateReceptionDto, Guid id)
        {
            var updateReception = await _receptionRepository.UpdateReceptionAsync(updateReceptionDto, id);
            if (updateReception == null) {

                return BadRequest("Изменения не приняты");
            }
            else
            {
                return Ok(updateReception);
            }
        }
    }
}
