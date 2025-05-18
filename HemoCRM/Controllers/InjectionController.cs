using HemoCRM.Web.Dtos.InjectionDtos;
using HemoCRM.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HemoCRM.Web.Controllers
{
    [Route("api/injection")]
    [ApiController]
    public class InjectionController : ControllerBase
    {
        private readonly IInjectionRepository _injectionRepository;

        public InjectionController(IInjectionRepository injectionRepository)
        {
            _injectionRepository = injectionRepository;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateInjection([FromBody] CreateInjectionDto dto)
        {
            var injection = await _injectionRepository.CreateInjectionAsync(dto);
            return CreatedAtAction(nameof(GetInjectionById), new { id = injection.Id }, injection);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInjectionById(Guid id)
        {
            var injection = await _injectionRepository.GetInjectionByIdAsync(id);
            if (injection == null)
                return NotFound();

            return Ok(injection);
        }

        [Authorize]
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetPatientInjections(Guid patientId)
        {
            var list = await _injectionRepository.GetPatientInjectionsAsync(patientId);
            return Ok(list);
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllInjections()
        {
            var list = await _injectionRepository.GetAllInjectionsAsync();
            return Ok(list);
        }
    }
}
