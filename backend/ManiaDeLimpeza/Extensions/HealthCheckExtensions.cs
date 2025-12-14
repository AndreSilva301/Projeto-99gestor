using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

namespace ManiaDeLimpeza.Api.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IEndpointRouteBuilder MapConfiguredHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description,
                            duration = e.Value.Duration.TotalMilliseconds,
                            exception = e.Value.Exception?.Message,
                            data = e.Value.Data
                        }),
                        totalDuration = report.TotalDuration.TotalMilliseconds
                    }, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    await context.Response.WriteAsync(result);
                }
            });

            return endpoints;
        }
    }
}
