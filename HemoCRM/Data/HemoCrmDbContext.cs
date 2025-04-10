using HemoCRM.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace HemoCRM.Web.Data
{
    public class HemoCrmDbContext:DbContext
    {

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Reception> Receptions { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }

        public HemoCrmDbContext(DbContextOptions options):base(options) 
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
