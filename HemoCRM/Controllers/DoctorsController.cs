﻿using HemoCRM.Web.Dtos.DoctorDtos;
using HemoCRM.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HemoCRM.Web.Controllers
{
    [Route("api/doctor")]
    [ApiController]
    public class DoctorsController : Controller
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorsController(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(Guid id)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(id);
            if (doctor == null)
            {
                return NotFound("Доктор не найден");
            }
            return Ok(doctor);
        }

        [Authorize]
        [HttpGet("GetDoctors")]
        public async Task<IActionResult> GetDoctors()
        {
            var doctorList = await _doctorRepository.GetDoctorsAsync();
            if (doctorList == null)
            {
                return NotFound("Доктора не найденыв");
            }
            return Ok(doctorList);
        }

        [HttpPost("CreateDoctor")]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto createDoctorDto)
        {
            var doctor = await _doctorRepository.CreateDoctorAsync(createDoctorDto);
            if (doctor == null)
            {
                return BadRequest("Пользователь не создан");
            }
            return CreatedAtAction(nameof(GetDoctorById), new { id = doctor.Id }, doctor);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctorData(UpdateDoctorDataDto updateDoctorDataDto, Guid id)
        {
            var doctor = await _doctorRepository.UpdateDoctorAsync(updateDoctorDataDto, id);
            if (doctor == null)
            {
                return BadRequest("Изменения не сохранены");
            }
            return Ok(doctor);
        }

        [Authorize]
        [HttpGet("getDoctorByUserId/{userId}")]
        public async Task<IActionResult> GetDoctorByUserId(Guid userId)
        {
            var doctor = await _doctorRepository.GetDoctorByUserIdAsync(userId);
            if (doctor == null)
                return NotFound("Доктор не найден");
            return Ok(doctor);
        }
    }
}
