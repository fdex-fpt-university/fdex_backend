using FDex.Infrastructure.Common;
using FDex.Application.Common;
using FDex.Persistence.Common;
using AutoMapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using FDex.Application.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddHttpContextAccessor();
        builder.Services.ConfigurePersistenceServices(builder.Configuration);
        builder.Services.ConfigureApplicationServices();
        builder.Services.ConfigureInfrastructureServices();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        using (ServiceProvider serviceProvider = builder.Services.BuildServiceProvider())
        {
            EventDispatcherService dispatcher = serviceProvider.GetRequiredService<EventDispatcherService>();
            dispatcher.GetLogsTokenSwapObservableSubscription();
        }
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

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseRouting();

        app.UseAuthorization();

        app.UseCors("CorsPolicy");

        app.MapControllers();

        app.Run();
    }
}