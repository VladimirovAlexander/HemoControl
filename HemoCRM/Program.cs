using HemoCRM.Web.Data;
using HemoCRM.Web.Interfaces;
using HemoCRM.Web.Repository;
using Microsoft.EntityFrameworkCore;

namespace HemoCRM.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<HemoCrmDbContext>(option =>
            {
                option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IPatientRepository, PatientRepository>();
            builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
            builder.Services.AddScoped<IReceptionRepository, ReceptionRepository>();
            builder.Services.AddScoped<IInjectionRepository, InjectionRepository>();
            builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
            builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
            builder.Services.AddScoped<ITestRepository, TestRepository>();
            
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();

            builder.Services.AddHttpClient();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:7001")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            app.MapControllers();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowFrontend");

            app.MapGet("/", () => Results.Redirect("/swagger"));

            app.Run();
        }
    }
}
