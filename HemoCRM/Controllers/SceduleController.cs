using HemoCRM.Web.Dtos.ScheduleDtos;
using HemoCRM.Web.Interfaces;
using HemoCRM.Web.Models;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// Получить доступные дни для записи к врачу
        /// </summary>
        [HttpGet("available-days/{doctorId}")]
        public async Task<IActionResult> GetAvailableDays(Guid doctorId)
        {
            var availableDays = await _scheduleRepository.GetAvailableDaysAsync(doctorId);

            if (!availableDays.Any())
            {
                return NotFound("Нет доступных дней для записи");
            }

            return Ok(availableDays.Select(d => d.ToString("yyyy-MM-dd")));
        }

        /// <summary>
        /// Получить доступное время на выбранный день
        /// </summary>
        [HttpGet("available-times/{doctorId}")]
        public async Task<IActionResult> GetAvailableTimes(Guid doctorId, [FromQuery] DateTime date)
        {
            date = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

            var availableTimes = await _scheduleRepository.GetAvailableTimesAsync(doctorId, date);

            if (!availableTimes.Any())
            {
                return NotFound("Нет доступного времени на выбранный день");
            }

            return Ok(availableTimes.Select(t => t.ToString(@"hh\:mm")));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateDoctorSchedule([FromBody] CreateDoctorScheduleRequest request)
        {
            if (request.Days == null || !request.Days.Any())
            {
                return BadRequest("Не указаны дни расписания.");
            }

            var schedules = request.Days.Select(day => new DoctorSchedule
            {
                Id = Guid.NewGuid(),
                DoctorId = request.DoctorId,
                DayOfWeek = day.DayOfWeek,
                StartTime = day.StartTime,
                EndTime = day.EndTime,
                AppointmentDuration = day.AppointmentDuration,
                BreakBetweenAppointments = day.BreakBetweenAppointments,
                LunchStart = day.LunchStart,
                LunchEnd = day.LunchEnd
            }).ToList();

            await _scheduleRepository.AddDoctorSchedulesAsync(schedules);

            return Ok(new { message = "Расписание врача успешно создано.", count = schedules.Count });
        }

        [HttpGet("{doctorId}")]
        public async Task<IActionResult> GetDoctorSchedule(Guid doctorId)
        {
            var schedules = await _scheduleRepository.GetDoctorSchedulesAsync(doctorId);
            return Ok(schedules);
        }

        [HttpDelete("{doctorId}")]
        public async Task<IActionResult> DeleteDoctorSchedule(Guid doctorId)
        {
            await _scheduleRepository.DeleteSchedulesByDoctorAsync(doctorId);
            return Ok(new { message = "Расписание врача удалено." });
        }
    }
}
