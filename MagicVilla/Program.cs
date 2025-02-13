using MagicVilla;
using MagicVilla.Data;
using MagicVilla.Loggings;
using MagicVilla.Repos;
using MagicVilla.Repos.IRepo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            builder.Services.AddApiVersioning(options => {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true; // To send supported version in the response headers
            });
            builder.Services.AddControllers().AddNewtonsoftJson();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddScoped<IVillaRepository, VillaRepository>();
            builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });

            builder.Services.AddAuthentication(options =>
            {
                // Check With JWT Token in header
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // [Authorize]
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // return unauthorized instead of notfound
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>   // Verified key
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    //ValidIssuer = builder.Configuration["JWT:IssuerURL"],
                    //ValidAudience = builder.Configuration["JWT:AudienceURL"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ApiSettings:SecretKey"]))
                };

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

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
