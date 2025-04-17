using HemoCRM.Web.Dtos.MedicationDtos;
using HemoCRM.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HemoCRM.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationController : ControllerBase
    {
        private readonly IMedicationRepository _medicationRepository;

        public MedicationController(IMedicationRepository medicationRepository)
        {
            _medicationRepository = medicationRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var meds = await _medicationRepository.GetAllAsync();
            return Ok(meds);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var med = await _medicationRepository.GetByIdAsync(id);
            if (med == null)
                return NotFound();

            return Ok(med);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMedicationDto dto)
        {
            var med = await _medicationRepository.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = med.Id }, med);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMedicationDto dto)
        {
            var med = await _medicationRepository.UpdateAsync(id, dto);
            if (med == null)
                return NotFound();

            return Ok(med);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _medicationRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("remaining")]
        public async Task<IActionResult> GetRemainingQuantity(Guid patientId, string medicationName)
        {
            var quantity = await _medicationRepository.GetRemainingQuantityAsync(patientId, medicationName);
            if (quantity == null)
                return NotFound("Лекарство не найдено");

            return Ok(new { MedicationName = medicationName, Quantity = quantity });
        }

        [HttpPost("prescribe")]
        public async Task<IActionResult> PrescribeMedication([FromBody] PrescribeMedicationDto dto)
        {
            try
            {
                var medication = await _medicationRepository.PrescribeMedicationAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = medication.Id }, medication);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
