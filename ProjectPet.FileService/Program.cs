using Hangfire;
using ProjectPet.FileService.Extensions;
using ProjectPet.FileService.Infrastructure.AmazonS3;
using ProjectPet.FileService.Infrastructure.Hangfire;
using ProjectPet.FileService.Infrastructure.MongoDb;
using Scalar.AspNetCore;

namespace ProjectPet.FileService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddEndpoints();

        builder.AddAmazonS3();

        builder.AddMongoDb();

        builder.AddHangfire();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {

            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.MapEndpoints();
        app.UseAuthlessHangfireDashboard("/jobs");

        using (var scope = app.Services.CreateScope())
        {
            var backgroundJobs = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();
            backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
        }

        app.Run();
    }
}
