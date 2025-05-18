using Microsoft.AspNetCore.Mvc;
using HemoCRM.Web.Dtos.PatirntDtos;
using HemoCRM.Web.Interfaces;
using System.Text.Json;
using HemoCRM.Web.Dtos.AccountDtos;
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
        [Authorize]
        [HttpPost("CreatePatient")]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDto patientDto)
        {
            if (patientDto == null)
            {
                return BadRequest("Некоректные данные пациента.");
            }
            var patien = await _repo.CreatePatientAsync(patientDto);

            return CreatedAtAction(nameof(GetPatientById), new { id = patien.Id }, patien);
        }

        [Authorize]
        [HttpGet("sync-user-with-patient")]
        public async Task<IActionResult> SyncUserWithPatient()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("UserId not found in token.");

            var userId = Guid.Parse(userIdClaim);
            var accessToken = HttpContext.Request.Headers["Authorization"].ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7000/api/account/info");
            request.Headers.Add("Authorization", accessToken);

            var userResponse = await _client.SendAsync(request);
            if (!userResponse.IsSuccessStatusCode)
                return StatusCode((int)userResponse.StatusCode, "Не удалось получить данные пользователя");

            var content = await userResponse.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<UserInfoDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (userInfo == null)
                return BadRequest("Ответ от сервиса пользователей пуст.");

            if (string.IsNullOrWhiteSpace(userInfo.PolicyNumber))
                return BadRequest("Полис обязателен для синхронизации.");

            var existingPatient = await _repo.FindPatientByPolicyAndNoUserAsync(userInfo.PolicyNumber, userId);
            if (existingPatient != null)
            {
                return Ok("Пациент успешно привязан к пользователю.");
            }

            var draftPatient = new CreatePatientDto
            {
                UserId = userId,
                Name = userInfo.Name,
                Policy = userInfo.PolicyNumber
            };

            var createdPatient = await _repo.CreatePatientAsync(draftPatient);

            return Ok(new { message = "Синхронизирован", patientId = createdPatient.Id });
        }


        [Authorize]
        [HttpGet("is-synced/{userId}")]
        public async Task<IActionResult> IsSyncUserWithPatient(Guid userId)
        {
            var patientId = await _repo.GetPatientIdByUserIdAsync(userId);

            if (patientId == Guid.Empty || patientId == null)
            {
                return NotFound("Пользователь не синхронизирован с пациентом.");
            }

            return Ok(new { patientId });
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPatientById(Guid id)
        {
            var patient = await _repo.GetPatientByIdAsync(id);

            if(patient == null)
            {
                return NotFound("Пациент не найден");
            }
            return Ok(patient);
        }

        [Authorize]
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

        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePatientData(Guid id, UpdatePatientDataDto updatePatientDataDto)
        {
            if (updatePatientDataDto == null)
            {
                return BadRequest("Некоректные данные");
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
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("Не найден Id пользователя");

            var userId = Guid.Parse(userIdClaim);

            string policy = Request.Query["policy"];
            if (string.IsNullOrWhiteSpace(policy)) return BadRequest("Номер полиса обязательный");

            var patient = await _repo.FindPatientByPolicyAndNoUserAsync(policy, userId);
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
            return NotFound("Подходящий пациент не найден.");
        }

    }
}
