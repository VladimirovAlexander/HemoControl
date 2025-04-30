using Microsoft.AspNetCore.Mvc;
using HemoCRM.Web.Dtos.PatirntDtos;
using HemoCRM.Web.Interfaces;
using System.Text.Json;
using System.Text;
using HemoCRM.Web.Dtos.AccountDtos;
using HemoCRM.Web.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace HemoCRM.Web.Controllers
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

        [Authorize]
        [HttpPost("sync-user-with-patient")]
        public async Task<IActionResult> SyncUserWithPatient()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("UserId not found in token.");
            var userId = Guid.Parse(userIdClaim);

            // Получить данные пользователя из AccountService (например, по Policy)
            var userResponse = await _client.GetAsync($"https://localhost:7000/api/account/info");

            if (!userResponse.IsSuccessStatusCode)
                return StatusCode((int)userResponse.StatusCode, "Не удалось получить данные пользователя");

            var userInfo = JsonSerializer.Deserialize<UserInfoDto>(await userResponse.Content.ReadAsStringAsync());

            if (userInfo == null || string.IsNullOrEmpty(userInfo.PolicyNumber))
                return BadRequest("Недостаточно информации для синхронизации");

            // Найти пациента по полису и без привязки к пользователю
            var patient = await _repo.FindPatientByPolicyAndNoUserAsync(userInfo.PolicyNumber);
            if (patient != null)
            {
                var updateDto = new UpdatePatientDataDto { UserId = userId };
                await _repo.UpdatePatientDataAsync(updateDto, patient.Id);
                return Ok("Пациент успешно привязан к пользователю.");
            }

            // Если пациента нет — создать нового
            var newPatientDto = new CreatePatientDto
            {
                Policy = userInfo.PolicyNumber,
                Name = userInfo.Name,
                Surname = userInfo.Surname,
                DateOfBirth = userInfo.DateOfBirth,
                UserId = userId
            };

            var created = await _repo.CreatePatientAsync(newPatientDto);
            return Ok($"Создан новый пациент и привязан к пользователю: {created.Id}");
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

        [Authorize]
        [HttpPost("auto-bind")]
        public async Task<IActionResult> AutoBindPatient()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("UserId not found in token.");

            var userId = Guid.Parse(userIdClaim);

            // Пример: допустим, PolicyNumber передаётся как query
            string policy = Request.Query["policy"];
            if (string.IsNullOrWhiteSpace(policy)) return BadRequest("Policy number is required");

            var patient = await _repo.FindPatientByPolicyAndNoUserAsync(policy);
            if (patient != null)
            {
                var updateDto = new UpdatePatientDataDto
                {
                    UserId = userId,
                    Name = patient.Name,
                    Surname = patient.Surname,
                    Patronymic = patient.Patronymic,
                    DateOfBirth = patient.DateOfBirth,
                    Policy = patient.Policy,
                    Country = patient.Country,
                    Region = patient.Region,
                    City = patient.City,
                    Street = patient.Street,
                    HouseNumber = patient.HouseNumber,
                    AppartmentNumber = patient.AppartmentNumber
                };

                await _repo.UpdatePatientDataAsync(updateDto, patient.Id);
                return Ok("Пациент привязан к пользователю.");
            }

            // Если не найден — можно создать нового
            // или вернуть сообщение
            return NotFound("Подходящий пациент не найден.");
        }

    }
}
