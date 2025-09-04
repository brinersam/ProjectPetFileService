using Hangfire;
using ProjectPet.FileService.Extensions;
using ProjectPet.FileService.Infrastructure.AmazonS3;
using ProjectPet.FileService.Infrastructure.Hangfire;
using ProjectPet.FileService.Infrastructure.MongoDb;
using ProjectPet.FileService.Infrastructure.RabbitMQ;
using ProjectPet.FileService.Infrastructure.Redis;
using ProjectPet.FileService.Options;
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

        builder.Services.Configure<TimeoutStuckUploadsJobOptions>(
            builder.Configuration.GetSection(TimeoutStuckUploadsJobOptions.SECTION));

        builder.AddRabbitMQ();

        builder.AddAmazonS3();

        builder.AddMongoDb();

        builder.AddHangfire();

        builder.AddRedis();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {

            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.MapEndpoints();
        app.UseAuthlessHangfireDashboard("/jobs");

        app.EnqueueStartupJobs();

        app.Run();
    }
}
