using HemoCRM.Web.Dtos.ScheduleDtos;
using HemoCRM.Web.Interfaces;
using HemoCRM.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HemoCRM.Web.Controllers
{
    [Route("api/schedule")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleController(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        [Authorize]
        [HttpPost("create-slots")]
        public async Task<IActionResult> CreateDoctorSlots([FromBody] CreateDoctorScheduleRequest request)
        {
            if (request.NumberOfAppointments < 1 || request.NumberOfAppointments > 10)
                return BadRequest("Количество сеансов должно быть от 1 до 10.");

            var slots = new List<DoctorAppointmentSlot>();
            var currentStart = request.StartDateTime;
            var appointmentDuration = TimeSpan.FromMinutes(20);
            var breakBetween = TimeSpan.FromMinutes(10);

            for (int i = 0; i < request.NumberOfAppointments; i++)
            {
                var startDateTime = currentStart;
                var endDateTime = startDateTime + appointmentDuration;

                slots.Add(new DoctorAppointmentSlot
                {
                    Id = Guid.NewGuid(),
                    DoctorId = request.DoctorId,
                    StartDateTime = startDateTime,
                    EndDateTime = endDateTime,
                });

                currentStart += appointmentDuration + breakBetween;
            }

            await _scheduleRepository.CreateDoctorSlots(slots);
            return Ok(new { message = "Сеансы созданы", count = slots.Count });
        }

        [Authorize]
        [HttpGet("get-doctor-slots")]
        public async Task<IActionResult> GetDoctorSlots([FromQuery] Guid doctorId)
        {
            var slots = await _scheduleRepository.GetDoctorSlots(doctorId);
            if (slots == null || !slots.Any())
                return NotFound("Нет доступных сеансов для данного врача.");
            return Ok(slots);
        }

        [Authorize]
        [HttpGet("get-doctor-slot-days")]
        public async Task<IActionResult> GetDoctorSlotsOnDay([FromQuery] Guid doctorId)
        {
            var slotDates = await _scheduleRepository.GetDoctorSlotDays(doctorId);

            if (slotDates == null || !slotDates.Any())
                return NotFound("У врача нет запланированных слотов.");

            return Ok(slotDates);
        }

        [Authorize]
        [HttpGet("get-doctor-slot-times")]
        public async Task<IActionResult> GetDoctorSlotTimesOnDay([FromQuery] Guid doctorId, [FromQuery] DateTime date)
        {
            var utcDate = DateTime.SpecifyKind(date, DateTimeKind.Utc);

            var slotTimes = await _scheduleRepository.GetDoctorSlotTimesOnDay(doctorId, date.Date);

            if (slotTimes == null || !slotTimes.Any())
                return NotFound("На выбранный день нет слотов у врача.");

            return Ok(slotTimes);
        }
    }
}
