using IPCountryBlocker.Abstraction.IRepositories;
using IPCountryBlocker.Abstraction.IServices;
using IPCountryBlocker.Persistence.Repositories;
using IPCountryBlocker.Presentation;
using IPCountryBlocker.Presentation.Controllers;
using IPCountryBlocker.Service.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace IPCountryBlocker.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            #region Services
            builder.Services.AddControllers()
                            .AddApplicationPart(typeof(CountriesController).Assembly);

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
           
            builder.Services.AddSingleton<ICountryRepository, CountryRepository>();
            builder.Services.AddSingleton<ILogRepository, LogRepository>();

            // Register Services
            builder.Services.AddScoped<CountryService>();
            builder.Services.AddHttpClient<IIPGeolocationService, IPGeolocationService>();

            // Register BackgroundService
            builder.Services.AddHostedService<TemporalBlockBackgroundService>();
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}