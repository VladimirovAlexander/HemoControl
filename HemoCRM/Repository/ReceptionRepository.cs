using HemoCRM.Web.Data;
using HemoCRM.Web.Dtos.ReceptionDtos;
using HemoCRM.Web.Interfaces;
using HemoCRM.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace HemoCRM.Web.Repository
{
    public class ReceptionRepository : IReceptionRepository
    {

        private readonly HemoCrmDbContext _dbContext;
        public ReceptionRepository(HemoCrmDbContext hemoCrmDbContext) 
        {
            _dbContext = hemoCrmDbContext;
        }
        public async Task<Reception?> GetReceptionByIdAsync(Guid id)
        {
            var reception = await _dbContext.Receptions.FindAsync(id);

            if (reception == null)
            {
                return null;
            }
            return reception;
        }

        public async Task<Reception> CreateReceptionAsync(CreateReceptionDto receptionDto)
        {

            var doctor = await _dbContext.Doctors.FindAsync(receptionDto.DoctorId);

            if(doctor == null)
            {
                throw new Exception("Doctor not found");
            }

            var reception = new Reception()
            {
                AppointmentDate = receptionDto.AppointmentDate,
                CreatedAt = DateTime.UtcNow,
                Status = receptionDto.Status,
                Doctor = doctor,
                DoctorId = receptionDto.DoctorId,
                Notes = receptionDto.Notes
            };

            await _dbContext.Receptions.AddAsync(reception);
            await _dbContext.SaveChangesAsync();
            return reception;
        }

        public async Task<bool> DeleteReceptionAsync(Guid receptionId)
        {
            var reception = await _dbContext.Receptions.FindAsync(receptionId);

            if (reception != null)
            {
                reception.Status = ReceptionsStatus.Canceled;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Reception>?> GetDoctorReceptionsAsync(Guid doctorId)
        {
            var receptions = await _dbContext.Receptions.Where(x => x.DoctorId == doctorId).ToListAsync();
            if (receptions.Count == 0)
            {
                return null;
            }
            return receptions;
        }

        public async Task<List<Reception>?> GetReceptionsAsync()
        {
            var receptions = await _dbContext.Receptions.ToListAsync();
            if (receptions.Count == 0)
            {
                return receptions;
            }
            return receptions;
        }

        public async Task<List<Reception>?> GetUserReceptionsAsync(Guid userId)
        {
            var receptions = await _dbContext.Receptions.Where(x => x.PatientId == userId).ToListAsync();
            if (receptions.Count == 0)
            {
                return null;
            }
            return receptions;
        }

        public async Task<Reception> UpdateReceptionAsync(UpdateReceptionDto receptionDto, Guid receptionId)
        {
            var reception = await _dbContext.Receptions.FindAsync(receptionId);

            if (reception == null)
                throw new Exception("Reception not found");

            var doctor = await _dbContext.Doctors.FindAsync(receptionDto.DoctorId);

            if (doctor == null)
                throw new Exception("Doctor not found");

            reception.Doctor = doctor;
            reception.DoctorId = receptionDto.DoctorId;
            reception.Notes = receptionDto.Notes;
            reception.Status = receptionDto.Status;
            switch (receptionDto.Status)
            {
                case ReceptionsStatus.MovedBack:
                        
                    reception.AppointmentDate = receptionDto.AppointmentDate;
                    break;

                case ReceptionsStatus.Canceled:
                        
                    reception.AppointmentDate = null;
                    break;

                case ReceptionsStatus.Completed:
                    reception.CompletedAt = DateTime.UtcNow;
                    break;

                default:
                    break;
            }
            
            _dbContext.Receptions.Update(reception);
            await _dbContext.SaveChangesAsync();

            return reception;
        }
    }
}
