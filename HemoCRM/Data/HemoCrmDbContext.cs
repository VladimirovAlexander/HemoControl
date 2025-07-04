﻿using HemoCRM.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HemoCRM.Web.Data
{
    public class HemoCrmDbContext:DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Reception> Receptions { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<CompleteBloodCountTest> CompleteBloodCountTests { get; set; }
        public DbSet<CoagulogramTest> CoagulogramTests { get; set; }
        public DbSet<FactorAndVWFTest> FactorAndVWFTests { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<Injection> Injections { get; set; }
        public DbSet<DoctorAppointmentSlot> DoctorAppointmentSlots { get; set; }
        public DbSet<Notes> Notes { get; set; }

        public HemoCrmDbContext(DbContextOptions options):base(options) 
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Test>()
                .Property(t => t.Status)
                .HasConversion<string>();
            modelBuilder.Entity<Reception>()
                .HasOne(r => r.Notes)
                .WithOne(n => n.Reception)
                .HasForeignKey<Notes>(n => n.ReceptionId);

            modelBuilder.Entity<Test>()
                .HasOne(t => t.CbcDetails)
                .WithOne(d => d.Test)
                .HasForeignKey<CompleteBloodCountTest>(d => d.TestId);

            modelBuilder.Entity<Test>()
                .HasOne(t => t.CoagulogramDetails)
                .WithOne(d => d.Test)
                .HasForeignKey<CoagulogramTest>(d => d.TestId);

            modelBuilder.Entity<Test>()
                .HasOne(t => t.FactorAndVwfDetails)
                .WithOne(d => d.Test)
                .HasForeignKey<FactorAndVWFTest>(d => d.TestId);
        }
    }
}
