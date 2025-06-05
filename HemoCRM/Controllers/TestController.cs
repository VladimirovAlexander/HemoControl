using HemoCRM.Web.Dtos.TestDtos;
using HemoCRM.Web.Interfaces;
using HemoCRM.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HemoCRM.Web.Controllers6
{
    [ApiController]
    [Route("api/tests")]
    public class TestController : ControllerBase
    {
        private readonly ITestRepository _testRepository;

        public TestController(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tests = await _testRepository.GetAllTestsAsync();
            return Ok(tests);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var test = await _testRepository.GetTestByIdAsync(id);
            return test == null ? NotFound() : Ok(test);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdTest = await _testRepository.CreateTestAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdTest.Id }, createdTest);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateTestResults([FromBody] UpdateTestRequest dto) // Измените параметр
        {
            if (dto == null) // Проверяем наличие DTO
            {
                return BadRequest("DTO is required");
            }

            var test = await _testRepository.GetTestByIdAsync(dto.Dto.TestId);
            if (test == null)
                return NotFound("Анализ не найден");

            test.Status = dto.Dto.Status switch
            {
                "Completed" => TestStatus.Completed,
                "Pending" => TestStatus.Pending,
                "InProgress" => TestStatus.InProgress,
                _ => test.Status 
            };

            test.Result = dto.Dto.Result;
            test.CompletedAt = dto.Dto.Status == "Completed" ? DateTime.UtcNow : null;

            // Обновляем детали в зависимости от типа теста
            switch (dto.Dto.TestType)
            {
                case "CBC":
                    test.CbcDetails ??= new CompleteBloodCountTest { TestId = test.Id };
                    test.CbcDetails.Hemoglobin = dto.Dto.Results.Hemoglobin;
                    test.CbcDetails.Hematocrit = dto.Dto.Results.Hematocrit;
                    test.CbcDetails.WhiteBloodCells = dto.Dto.Results.WhiteBloodCells;
                    test.CbcDetails.RedBloodCells = dto.Dto.Results.RedBloodCells;
                    test.CbcDetails.Platelets = dto.Dto.Results.Platelets;
                    test.CbcDetails.MCH = dto.Dto.Results.MCH;
                    test.CbcDetails.MCV = dto.Dto.Results.MCV;
                    break;

                case "Coagulogram":
                    test.CoagulogramDetails ??= new CoagulogramTest { TestId = test.Id };
                    test.CoagulogramDetails.PT = dto.Dto.Results.PT;
                    test.CoagulogramDetails.INR = dto.Dto.Results.INR;
                    test.CoagulogramDetails.APTT = dto.Dto.Results.APTT;
                    test.CoagulogramDetails.Fibrinogen = dto.Dto.Results.Fibrinogen;
                    break;

                case "FactorAndVWF":
                    test.FactorAndVwfDetails ??= new FactorAndVWFTest { TestId = test.Id };
                    test.FactorAndVwfDetails.FactorVIII = dto.Dto.Results.FactorVIII;
                    test.FactorAndVwfDetails.FactorIX = dto.Dto.Results.FactorIX;
                    test.FactorAndVwfDetails.VWFActivity = dto.Dto.Results.VWFActivity;
                    break;

                default:
                    return BadRequest("Неизвестный тип анализа");
            }

            await _testRepository.UpdateTestAsync(test);
            return Ok(test);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _testRepository.DeleteTestAsync(id);
            return deleted ? Ok() : NotFound();
        }

        [HttpDelete("remove-test/{receptionId}/{testType}")]
        public async Task<IActionResult> RemoveTest(Guid receptionId, string testType)
        {
            var test = await _testRepository.GetTestByReceptionIdAndTypeAsync(receptionId, testType);
            if (test == null)
                return NotFound("Анализ не найден");

            await _testRepository.DeleteAsync(test);

            return Ok("Анализ успешно удалён");
        }

        [Authorize]
        [HttpGet("reception/{receptionId}")]
        public async Task<IActionResult> GetTestsByReceptionId(Guid receptionId)
        {
            var tests = await _testRepository.GetTestsByReceptionIdAsync(receptionId);
            if (tests == null || !tests.Any())
            {
                return NotFound("Тесты для данного приема не найдены");
            }

            var enrichedTests = await _testRepository.EnrichTestResults(tests);

            return Ok(enrichedTests);
        }

        [Authorize]
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetPatientTests(Guid patientId)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                var receptions = await _testRepository.GetReceptionsByPatientIdAsync(patientId);

                var tests = new List<Test>();
                foreach (var reception in receptions)
                {
                    var receptionTests = await _testRepository.GetTestsByReceptionIdAsync(reception.Id);
                    if (receptionTests != null)
                    {
                        tests.AddRange(receptionTests);
                    }
                }

                if (!tests.Any())
                {
                    return NotFound("Анализы для данного пациента не найдены");
                }

                var enrichedTests = await _testRepository.EnrichTestResults(tests);

                var sortedTests = enrichedTests
                    .OrderByDescending(t => t.CreatedAt)
                    .ToList();

                return Ok(sortedTests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка при получении анализов пациента");
            }
        }


    }
}
