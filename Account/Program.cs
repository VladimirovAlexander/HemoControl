using Account.Data;
using Microsoft.EntityFrameworkCore;

namespace Account
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AccountDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddControllers();
            
            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.Run();
        }
    }
}
