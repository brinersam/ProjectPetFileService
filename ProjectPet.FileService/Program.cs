using ProjectPet.FileService.Extensions;
using Scalar.AspNetCore;

namespace ProjectPet.FileService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddEndpoints();

        // Amazon s3
        builder.AddS3();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {

            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseAuthorization();

        app.MapEndpoints();

        app.Run();
    }
}
