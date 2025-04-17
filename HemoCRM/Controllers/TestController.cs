using HemoCRM.Web.Dtos.TestDtos;
using HemoCRM.Web.Interfaces;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tests = await _testRepository.GetAllTestsAsync();
            return Ok(tests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var test = await _testRepository.GetTestByIdAsync(id);
            return test == null ? NotFound() : Ok(test);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTestDto dto)
        {
            var createdTest = await _testRepository.CreateTestAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdTest.Id }, createdTest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTestDto dto)
        {
            var updatedTest = await _testRepository.UpdateTestAsync(id, dto);
            return updatedTest == null ? NotFound() : Ok(updatedTest);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _testRepository.DeleteTestAsync(id);
            return deleted ? Ok() : NotFound();
        }
    }
}
