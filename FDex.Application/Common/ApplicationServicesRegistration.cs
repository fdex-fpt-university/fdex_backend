using System;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FDex.Application.Contracts.Persistence;
using AutoMapper;
using FDex.Application.Services;
using Hangfire;
using Microsoft.Extensions.Configuration;

namespace FDex.Application.Common
{
	public static class ApplicationServicesRegistration
	{
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(config => config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            services.AddScoped<EventDispatcherService>();
            services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("FDexDB")));
            services.AddHangfireServer();
            return services;
        }
    }
}

