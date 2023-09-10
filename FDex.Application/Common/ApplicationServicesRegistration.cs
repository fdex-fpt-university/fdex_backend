using System;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FDex.Application.Common
{
	public static class ApplicationServicesRegistration
	{
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(config => config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}

