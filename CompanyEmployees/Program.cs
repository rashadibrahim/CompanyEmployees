using CompanyEmployees.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using static System.Net.WebRequestMethods;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Microsoft.EntityFrameworkCore;
using CompanyEmployees.Repository;
using Contracts;
using AspNetCoreRateLimit;

namespace CompanyEmployees
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(),"/nlog.config"));

            // Add services to the container.
            builder.Services.ConfigureCors();
            builder.Services.ConfigureIISIntegration();
            builder.Services.ConfigureLoggerService();
            builder.Services.ConfigureeRepositoryManager();
            builder.Services.ConfigureServiceManager();
            builder.Services.ConfigureSqlContext(builder.Configuration);
            builder.Services.ConfigureResponseCaching();
            builder.Services.ConfigureRateLimitingOptions();
            builder.Services.AddHttpContextAccessor();
            builder.Services.ConfigureIdentity();
            builder.Services.ConfigureJWT(builder.Configuration);

            builder.Services.AddAuthentication();
            builder.Services.AddMemoryCache();
            builder.Services.AddControllers(); //method registers only the controllers in IServiceCollection and not Views or Pages because they are not required in the Web API project
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            /*builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();*/

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            var logger = app.Services.GetRequiredService<ILoggerManager>();
            app.ConfigureExceptionHandler(logger);
            if (app.Environment.IsProduction())
                app.UseHsts(); //HTTP Strict Transport Security is a security mechanism that tells the browser to only communicate with your website using HTTPS
            
            app.UseHttpsRedirection();
            app.UseStaticFiles(); 
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                //used to configure the application to trust and process forwarded headers. These headers are typically added by proxies or load balancers, providing information about the original client request.
                ForwardedHeaders = ForwardedHeaders.All
            });
            
            app.UseIpRateLimiting();

            app.UseCors("CorsPolicy");
            app.UseResponseCaching();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers(); // adds the endpoints from controller actions to the IEndpointRouteBuilder(used to add endpoints in our app)

            app.Run();
        }
    }
}