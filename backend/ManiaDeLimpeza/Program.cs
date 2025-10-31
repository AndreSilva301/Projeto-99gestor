using ManiaDeLimpeza.Api.Auth;
using ManiaDeLimpeza.Api.Extensions;
using ManiaDeLimpeza.Api.Middleware;
using ManiaDeLimpeza.Application.Common;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using ManiaDeLimpeza.Infrastructure.Repositories;
using ManiaDeLimpeza.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace ManiaDeLimpeza;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Register repositories and services
        builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
        builder.Services.AddMarkedDependencies();

        // Configure AutoMapper
        builder.Services.AddAutoMapper(typeof(DefaultMapperProfile).Assembly);

        // Configure CORS
        builder.Services.AddConfiguredCors(builder.Configuration);

        // Add Controllers
        builder.Services.AddControllers();

        // Configure Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ManiaDeLimpeza API",
                Version = "v1"
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' followed by a space and your JWT token."
            };

            options.AddSecurityDefinition("Bearer", securityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
        });
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.Configure<ResetPasswordOptions>(
            builder.Configuration.GetSection(ResetPasswordOptions.SECTION));

        builder.Services.Configure<AuthOptions>(
            builder.Configuration.GetSection(AuthOptions.SECTION));

        var authOptions = builder.Configuration
            .GetSection(AuthOptions.SECTION)
            .Get<AuthOptions>();

        builder.Services.AddJwtAuthentication(authOptions.JwtSecret);
        builder.Services.AddAuthorization();

        //  Build the app
        var app = builder.Build();

        app.UseCors("DefaultCorsPolicy");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseMiddleware<ErrorHandlerMiddleware>();
        app.UseAuthentication();
        app.UseMiddleware<UserFetchMiddleware>();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
