using System;
using FDex.Application.Contracts.Infrastructure;
using FDex.Infrastructure.Mail;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Infrastructure.Common
{
	public static class InfrastructureServicesRegistration
	{
        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services)
        {
            services.AddTransient<IEmailSender, EmailSender>();
            return services;
        }
    }
}

