using Microsoft.AspNetCore.Mvc;
using HemoCRM.Models;
using HemoCRM.Repository;
using HemoCRM.Dtos.PatirntDtos;
using HemoCRM.Interfaces;
using Account.Dtos;
using NuGet.Protocol;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text;
namespace HemoCRM.Controllers
{
    [Route("api/patient")]
    [ApiController]
    public class PatientsController : Controller
    {
        private readonly IPatientRepository _repo;
        private readonly HttpClient _client;
        public PatientsController(IPatientRepository patientRepository, IHttpClientFactory httpClientFactory)
        {
            _repo = patientRepository;
            _client = httpClientFactory.CreateClient();
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

        [HttpPost("RegisterPatient/{id}")]
        public async Task<IActionResult> RegisterPatient(Guid id,[FromBody] RegisterDto registerDto)
        {
            if(id == Guid.Empty)
            {
                return BadRequest("Пользователь не найден");
            }
            if (await _repo.CheckUserRegistration(id) == true)
            {
                return Ok("Пользователь уже зарегистрирован");
            }

            var patient = await _repo.GetPatientByIdAsync(id);

            if(patient == null)
            {
                return NotFound("Пациент не найден");
            }

            var json = JsonSerializer.Serialize(registerDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("https://localhost:7000/api/account/register",content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var createdUser = JsonSerializer.Deserialize<UserResponseDto>(responseBody);
                if (createdUser != null)
                {
                    UpdatePatientDataDto dto = new UpdatePatientDataDto() { UserId = createdUser.UserId};

                    await _repo.UpdatePatientDataAsync(dto, patient.Id);
                }
                return Ok("Пациент успешно зарегистрирован и связан с учетной записью.");
            }
            return RedirectToAction();

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
