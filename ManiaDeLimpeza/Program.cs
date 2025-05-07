using ManiaDeLimpeza.Persistence;
using Microsoft.EntityFrameworkCore;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using AutoMapper;
using ManiaDeLimpeza.Application.Common;

namespace ManiaDeLimpeza;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        // Register services (DI)
        builder.Services.AddMarkedDependencies();

        // Include Automapper
        builder.Services.AddAutoMapper(typeof(DefaultMapperProfile).Assembly);

        // Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //Configure the database
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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