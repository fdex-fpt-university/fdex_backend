using FDex.Infrastructure.Common;
using FDex.Application.Common;
using FDex.Persistence.Common;
using AutoMapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using FDex.Application.Services;
using FDex.Api.Middleware;
using FDex.Application.Extensions;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddHealthChecks();
        builder.Services.AddHttpContextAccessor();
        builder.Services.ConfigurePersistenceServices(builder.Configuration);
        builder.Services.ConfigureApplicationServices();
        builder.Services.ConfigureInfrastructureServices();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHostedService<EventDispatcherService>();
        builder.Services.AddHostedService<EventDataSeedService>();
        builder.Services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new DoubleInfinityConverter())
        );
        builder.Services.Configure<HostOptions>(options =>
        {
            options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
        });
        builder.Services.AddCors(c =>
        {
            c.AddPolicy("CorsPolicy",
            builder => builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseRouting();

        app.UseAuthorization();

        app.UseCors("CorsPolicy");

        app.MapControllers();

        app.MapHealthChecks("/healthz");

        app.Run();
    }
}