using ManiaDeLimpeza.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ManiaDeLimpeza.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

            services.AddCors(options =>
            {
                options.AddPolicy("DefaultCorsPolicy", builder =>
                {
                    // Check if wildcard is configured (for development)
                    if (allowedOrigins.Contains("*"))
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                    else
                    {
                        builder
                            .WithOrigins(allowedOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddConfiguredHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>(
                    name: "database",
                    tags: new[] { "db", "sql", "sqlserver" });

            return services;
        }
    }
}
