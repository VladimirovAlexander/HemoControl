using HemoCRM.Data;
using HemoCRM.Repository;
using Microsoft.EntityFrameworkCore;

namespace HemoCRM
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

            builder.Services.AddScoped<PatientRepository, PatientRepository>();

            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();
            var app = builder.Build();

            app.MapControllers();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.Run();
        }
    }
}
