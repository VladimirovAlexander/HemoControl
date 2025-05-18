using HemoCRM.Web.Dtos.TestDtos;
using HemoCRM.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HemoCRM.Web.Controllers
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
            var createdTest = await _testRepository.CreateTestAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdTest.Id }, createdTest);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTestDto dto)
        {
            var updatedTest = await _testRepository.UpdateTestAsync(id, dto);
            return updatedTest == null ? NotFound() : Ok(updatedTest);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _testRepository.DeleteTestAsync(id);
            return deleted ? Ok() : NotFound();
        }
    }
}
