using AspNetCoreRateLimit;
using CompanyEmployees.Repository;
using Contracts;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Service;
using Service.Contracts;
using System.Data;

using System.Diagnostics.Metrics;

namespace CompanyEmployees.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyHeader() // or WithHeaders("accept", "content-type") 
                    .AllowAnyMethod() // or WithMethods("POST", "GET") 
                    .AllowAnyOrigin() // or WithOrigins("https://example.com")
                    .WithExposedHeaders("X-Pagination");
                });
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {

            });
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureeRepositoryManager(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        }

        public static void ConfigureServiceManager(this IServiceCollection services)
        { 
            services.AddScoped<IServiceManager, ServiceManager>();
        }

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        { 
            services.AddDbContext<RepositoryContext>(opts => opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
        }

        public static void ConfigureResponseCaching(this IServiceCollection services) { 
            services.AddResponseCaching();
        }


        public static void ConfigureRateLimitingOptions(this IServiceCollection services)
        {
            var rateLimitRules = new List<RateLimitRule>
            {
            new RateLimitRule //the rule states that three requests are allowed in a five-minute period for any endpoint in our API.
            {
                Endpoint = "*",
                Limit = 3,
            Period = "5m"
            }
            };

            //configure IpRateLimitOptions to add the created rule
            services.Configure<IpRateLimitOptions>(opt => { opt.GeneralRules = rateLimitRules; });

            /*
             register rate limit stores, configuration, and processing strategy as a singleton.
            They serve the purpose of storing rate limit counters and policies as well as adding configuration
             */
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }


    }
}
