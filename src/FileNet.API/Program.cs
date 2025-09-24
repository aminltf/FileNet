using FileNet.Application.Extensions.DependencyInjection;
using FileNet.Infrastructure.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace FileNet.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddApplication();

        builder.Services.AddInfrastructurePersistence(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "FileNet System API v1");
                options.DocumentTitle = "FileNet System Docs";
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
