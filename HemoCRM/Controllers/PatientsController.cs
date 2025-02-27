using Microsoft.AspNetCore.Mvc;
using HemoCRM.Models;
using HemoCRM.Repository;
using HemoCRM.Dtos.PatirntDtos;
using HemoCRM.Interfaces;
namespace HemoCRM.Controllers
{
    [Route("api/patient")]
    [ApiController]
    public class PatientsController : Controller
    {
        private readonly IPatientRepository _repo;
        public PatientsController(IPatientRepository patientRepository)
        {
            _repo = patientRepository;
        }

        [HttpPost("CreatePatient")]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDto patientDto)
        {
            if (patientDto == null)
            {
                return BadRequest("Некорректные данные пациента.");
            }
            var patien = await _repo.CreatePatientAsync(patientDto);

            return CreatedAtAction(nameof(GetPatientById), new { id = patien.Id }, patien);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(Guid id)
        {
            var patient = await _repo.GetPatientByIdAsync(id);

            if(patient == null)
            {
                return NotFound("Пациент не найден");
            }
            return Ok(patient);
        }

        [HttpGet("GetPatients")]
        public async Task<IActionResult> GetPatients()
        {

            var patients = await _repo.GetPatientsAsync();
            
            if(patients == null)
            {
                return NotFound();
            }

            return Ok(patients);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatientData(Guid id, UpdatePatientDataDto updatePatientDataDto)
        {
            if (updatePatientDataDto == null)
            {
                return BadRequest("Некторектные данные");
            }
            var patient = await _repo.UpdatePatientDataAsync(updatePatientDataDto, id);

            if(patient == null)
            {
                return NotFound("Пациент не найден");
            }
            return Ok(patient);
        }
    }
}
