using ManiaDeLimpeza.Persistence;
using Microsoft.EntityFrameworkCore;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using AutoMapper;
using ManiaDeLimpeza.Application.Common;
using ManiaDeLimpeza.Api.Auth;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Persistence.Repositories;
using Microsoft.OpenApi.Models;

namespace ManiaDeLimpeza;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Register services (DI)
        builder.Services.AddMarkedDependencies();

        // Manually registered dependencies (DI)
        //builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        // Include Automapper
        builder.Services.AddAutoMapper(typeof(DefaultMapperProfile).Assembly);

        // Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "ManiaDeLimpeza API", Version = "v1" });

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

        //Configure the database
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


        //Add Authorization
        builder.Services.Configure<AuthOptions>(
            builder.Configuration.GetSection(AuthOptions.SECTION));

        var authOptions = builder.Configuration
            .GetSection(AuthOptions.SECTION)
            .Get<AuthOptions>();

        builder.Services.AddJwtAuthentication(authOptions.JwtSecret);
        builder.Services.AddAuthorization();

        //Build App
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}