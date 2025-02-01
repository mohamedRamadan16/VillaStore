using MagicVilla;
using MagicVilla.Data;
using MagicVilla.Loggings;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("log/villalogs.txt", rollingInterval:RollingInterval.Day).CreateLogger();
            //builder.Host.UseSerilog();

            /// if you want to allow to return an xml format from the API
            //builder.Services.AddControllers(options =>
            //{
            //    options.ReturnHttpNotAcceptable = true;
            //}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();

            builder.Services.AddAutoMapper(typeof(MappingConfig));
            builder.Services.AddControllers().AddNewtonsoftJson();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });

            /// custom logging
            builder.Services.AddSingleton<ILogging, Logging>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
