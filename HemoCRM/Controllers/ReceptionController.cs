using HemoCRM.Web.Dtos.ReceptionDtos;
using HemoCRM.Web.Interfaces;
using HemoCRM.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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


        [Authorize]
        [HttpGet("reception/{id}")]
        public async Task<IActionResult> GetReceptionById(Guid id)
        {
            var reception = await _receptionRepository.GetReceptionByIdAsync(id);

            return Ok(reception);
        }

        [Authorize]
        [HttpGet("reception")]
        public async Task<IActionResult> GetReceptions()
        {
            var receptions = await _receptionRepository.GetReceptionsAsync();
            if (receptions == null)
            {
                return NotFound("Записи не найдены");
            }
            return Ok(receptions);
        }

        [Authorize]
        [HttpGet("user-receptions/{patientId}")]
        public async Task<IActionResult> GetUsersReceptions([FromRoute] Guid patientId)
        {
            var receptions = await _receptionRepository.GetUserReceptionsAsync(patientId);
            if (receptions == null)
            {
                return NotFound("Записи не найдены");
            }
            return Ok(receptions);
        }

        [Authorize]
        [HttpGet("doctor-receptions/{doctorId}")]
        public async Task<IActionResult> GetDoctorsReceptions(Guid doctorId)
        {
            var receptions = await _receptionRepository.GetDoctorReceptionsAsync(doctorId);
            if (receptions == null)
            {
                return NotFound("Записи не найдены");
            }
            return Ok(receptions);
        }
        
        [Authorize]
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

        [Authorize]
        [HttpPost("create-reception")]
        public async Task<IActionResult> CreateReception([FromBody] CreateReceptionDto createReceptionDto)
        {
            var result = await _receptionRepository.CreateReceptionAsync(createReceptionDto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { message = "Запись успешно создана", receptionId = result.ReceptionId });
        }
    }
}
